using System;
using Banzai.Factories;
using Banzai.Serialization;
using Ninject;
using NUnit.Framework;
using FluentAssertions;

namespace Banzai.Ninject.Test
{
    [TestFixture]
    public class WhenBuildingFlowsWithIds
    {
        [Test]
        public void Flow_Is_Registered_With_Container()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var flow = kernel.Get<FlowComponent<object>>("TestFlow1");

            flow.Should().NotBeNull();
            flow.IsFlow.Should().BeTrue();
            flow.Id.Should().Be("TestFlow1");
        }

        [Test]
        public void Flow_With_Explicit_Id_Is_Registered_With_Container()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1", "MyTestFlow")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var flow = kernel.Get<FlowComponent<object>>("TestFlow1");

            flow.Should().NotBeNull();
            flow.IsFlow.Should().BeTrue();
            flow.Id.Should().Be("MyTestFlow");
        }

        [Test]
        public void Flow_With_Children_With_Explicit_Ids_Are_Registered_With_Container()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1", "MyTestFlow")
                .AddRoot<IPipelineNode<object>>(id:"MyPipeline")
                .AddChild<ITestNode2>(id:"MyTestNode");

            flowBuilder.Register();

            var flow = kernel.Get<FlowComponent<object>>("TestFlow1");

            flow.Should().NotBeNull();
            flow.IsFlow.Should().BeTrue();
            flow.Id.Should().Be("MyTestFlow");
            flow.Children[0].Id.Should().Be("MyPipeline");
            flow.Children[0].Children[0].Id.Should().Be("MyTestNode");
        }

        [Test]
        public void Flow_Is_Built_With_NodeFactory()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = factory.BuildFlow("TestFlow1");
            flow.FlowId.Should().Be("TestFlow1");
            flow.Id.Should().Be("Banzai.Ninject.Test.TestPipelineNode1");
            ((IMultiNode<object>)flow).Children[0].Id.Should().Be("Banzai.Ninject.Test.TestNode2");
        }

        [Test]
        public void Flow_Root_Is_Retrieved_With_NodeFactory()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = factory.GetFlowRoot("TestFlow1");
            flow.Id.Should().Be("TestFlow1");
        }

        [Test]
        public void Flow_Built_With_NodeFactory_Has_Custom_Ids()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1", "MyFlow")
                .AddRoot<IPipelineNode<object>>(id: "MyPipeline")
                .AddChild<ITestNode2>(id: "MyTestNode");

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = factory.BuildFlow("TestFlow1");
            flow.FlowId.Should().Be("MyFlow");
            flow.Id.Should().Be("MyPipeline");
            ((IMultiNode<object>)flow).Children[0].Id.Should().Be("MyTestNode");
        }

        [Test]
        public void Flow_Built_From_FlowComponent_Contains_Default_Ids()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var component = flowBuilder.RootComponent;
            var factory = kernel.Get<INodeFactory<object>>();

            var flow = factory.BuildFlow(component);
            flow.FlowId.Should().Be("TestFlow1");
            flow.Id.Should().Be("Banzai.Ninject.Test.TestPipelineNode1");
            ((IMultiNode<object>)flow).Children[0].Id.Should().Be("Banzai.Ninject.Test.TestNode2");
        }

        [Test]
        public void Built_Pipeline_Contains_Custom_Flow_And_Pipeline_Ids()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1", "MyCustomFlow")
                .AddRoot<IPipelineNode<object>>(id: "MyPipeline")
                .AddChild<ITestNode2>(id: "MyTestNode");

            var component = flowBuilder.RootComponent;
            var factory = kernel.Get<INodeFactory<object>>();

            var flow = factory.BuildFlow(component);
            flow.FlowId.Should().Be("MyCustomFlow");
            flow.Id.Should().Be("MyPipeline");
            ((IMultiNode<object>)flow).Children[0].Id.Should().Be("MyTestNode");
        }


        [Test]
        public void Flow_Is_Built_With_NodeFactory_And_Json_Serializer()
        {
            Json.Registrar.RegisterAsDefault();

            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var component = flowBuilder.RootComponent;
            var factory = kernel.Get<INodeFactory<object>>();

            var serialized = SerializerProvider.Serializer.Serialize(component);

            var flow = factory.BuildSerializedFlow(serialized);
            flow.FlowId.Should().Be("TestFlow1");
            flow.Id.Should().Be("Banzai.Ninject.Test.TestPipelineNode1");
            ((IMultiNode<object>)flow).Children[0].Id.Should().Be("Banzai.Ninject.Test.TestNode2");
        }


        [Test]
        public void Flow_Builder_Is_Hydrated_And_Flow_Built_From_Json_Serializer()
        {
            Json.Registrar.RegisterAsDefault();

            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var factory = kernel.Get<INodeFactory<object>>();

            var serialized = flowBuilder.SerializeRootComponent();

            Console.WriteLine(serialized);

            var component = flowBuilder.DeserializeAndSetRootComponent(serialized);

            var flow = factory.BuildFlow(component);
            flow.FlowId.Should().Be("TestFlow1");
            flow.Id.Should().Be("Banzai.Ninject.Test.TestPipelineNode1");
            ((IMultiNode<object>)flow).Children[0].Id.Should().Be("Banzai.Ninject.Test.TestNode2");
        }
  
    }
}