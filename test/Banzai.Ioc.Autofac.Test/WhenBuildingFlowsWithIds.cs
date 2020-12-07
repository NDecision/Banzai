using System;
using Autofac;
using Banzai.Factories;
using Banzai.Serialization;
using Banzai.Serialization.JsonNet;
using FluentAssertions;
using Xunit;

namespace Banzai.Ioc.Autofac.Test
{
    public class WhenBuildingFlowsWithIds
    {
        [Fact]
        public void Flow_Is_Registered_With_Container()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);
            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var container = containerBuilder.Build();
            var flow = container.ResolveNamed<FlowComponent<object>>("TestFlow1");

            flow.Should().NotBeNull();
            flow.IsFlow.Should().BeTrue();
            flow.Id.Should().Be("TestFlow1");
        }

        [Fact]
        public void Flow_With_Explicit_Id_Is_Registered_With_Container()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);
            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1", "MyTestFlow")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var container = containerBuilder.Build();
            var flow = container.ResolveNamed<FlowComponent<object>>("TestFlow1");

            flow.Should().NotBeNull();
            flow.IsFlow.Should().BeTrue();
            flow.Id.Should().Be("MyTestFlow");
        }

        [Fact]
        public void Flow_With_Children_With_Explicit_Ids_Are_Registered_With_Container()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);
            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>(id: "MyPipeline")
                .AddChild<ITestNode2>(id: "MyTestNode");

            flowBuilder.Register();

            var container = containerBuilder.Build();
            var flow = container.ResolveNamed<FlowComponent<object>>("TestFlow1");

            flow.Should().NotBeNull();
            flow.IsFlow.Should().BeTrue();
            flow.Id.Should().Be("TestFlow1");
            flow.Children[0].Id.Should().Be("MyPipeline");
            flow.Children[0].Children[0].Id.Should().Be("MyTestNode");
        }

        [Fact]
        public void Flow_Is_Built_With_NodeFactory()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);
            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var container = containerBuilder.Build();
            var factory = container.Resolve<INodeFactory<object>>();

            var flow = factory.BuildFlow("TestFlow1");
            flow.FlowId.Should().Be("TestFlow1");
            flow.Id.Should().Be("Banzai.Ioc.Autofac.Test.TestPipelineNode1");
            ((IMultiNode<object>) flow).Children[0].Id.Should().Be("Banzai.Ioc.Autofac.Test.TestNode2");
        }

        [Fact]
        public void Flow_Root_Is_Retrieved_With_NodeFactory()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);
            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var container = containerBuilder.Build();
            var factory = container.Resolve<INodeFactory<object>>();

            var flow = factory.GetFlowRoot("TestFlow1");
            flow.Should().NotBeNull();
            flow.Id.Should().Be("TestFlow1");
        }

        [Fact]
        public void Flow_Built_With_NodeFactory_Has_Custom_Ids()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);
            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1", "MyFlow")
                .AddRoot<IPipelineNode<object>>(id: "MyPipeline")
                .AddChild<ITestNode2>(id: "MyTestNode");

            flowBuilder.Register();

            var container = containerBuilder.Build();
            var factory = container.Resolve<INodeFactory<object>>();

            var flow = factory.BuildFlow("TestFlow1");
            flow.FlowId.Should().Be("MyFlow");
            flow.Id.Should().Be("MyPipeline");
            ((IMultiNode<object>) flow).Children[0].Id.Should().Be("MyTestNode");
        }

        [Fact]
        public void Flow_Built_From_FlowComponent_Contains_Default_Ids()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var factory = container.Resolve<INodeFactory<object>>();
            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var component = flowBuilder.RootComponent;

            var flow = factory.BuildFlow(component);
            flow.FlowId.Should().Be("TestFlow1");
            flow.Id.Should().Be("Banzai.Ioc.Autofac.Test.TestPipelineNode1");
            ((IMultiNode<object>) flow).Children[0].Id.Should().Be("Banzai.Ioc.Autofac.Test.TestNode2");
        }

        [Fact]
        public void Built_Pipeline_Contains_Custom_Flow_And_Pipeline_Ids()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var factory = container.Resolve<INodeFactory<object>>();
            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1", "MyCustomFlow")
                .AddRoot<IPipelineNode<object>>(id: "MyPipeline")
                .AddChild<ITestNode2>(id: "MyTestNode");

            var component = flowBuilder.RootComponent;

            var flow = factory.BuildFlow(component);
            flow.FlowId.Should().Be("MyCustomFlow");
            flow.Id.Should().Be("MyPipeline");
            ((IMultiNode<object>) flow).Children[0].Id.Should().Be("MyTestNode");
        }


        [Fact]
        public void Flow_Is_Built_With_NodeFactory_And_Json_Serializer()
        {
            Registrar.RegisterAsDefault();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var component = flowBuilder.RootComponent;

            var serialized = SerializerProvider.Serializer.Serialize(component);

            var flow = factory.BuildSerializedFlow(serialized);
            flow.FlowId.Should().Be("TestFlow1");
            flow.Id.Should().Be("Banzai.Ioc.Autofac.Test.TestPipelineNode1");
            ((IMultiNode<object>) flow).Children[0].Id.Should().Be("Banzai.Ioc.Autofac.Test.TestNode2");
        }


        [Fact]
        public void Flow_Builder_Is_Hydrated_And_Flow_Built_From_Json_Serializer()
        {
            Registrar.RegisterAsDefault();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var factory = container.Resolve<INodeFactory<object>>();
            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var serialized = flowBuilder.SerializeRootComponent();

            Console.WriteLine(serialized);


            var component = flowBuilder.DeserializeAndSetRootComponent(serialized);

            var flow = factory.BuildFlow(component);
            flow.FlowId.Should().Be("TestFlow1");
            flow.Id.Should().Be("Banzai.Ioc.Autofac.Test.TestPipelineNode1");
            ((IMultiNode<object>) flow).Children[0].Id.Should().Be("Banzai.Ioc.Autofac.Test.TestNode2");
        }
    }
}