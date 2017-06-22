using System;
using Autofac;
using Banzai.Factories;
using Banzai.Serialization;
using FluentAssertions;
using NUnit.Framework;
using System.Reflection;

namespace Banzai.Autofac.Test
{
    [TestFixture]
    public class WhenBuildingFlows
    {
        [Test]
        public void Simple_Flow_Is_Registered_With_Container()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().GetTypeInfo().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var flow = container.ResolveNamed<FlowComponent<object>>("TestFlow1");

            flow.Should().NotBeNull();
            flow.IsFlow.Should().BeTrue();
        }

        [Test]
        public void Simple_Flow_Is_Accessed_With_NodeFactory()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().GetTypeInfo().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flow = factory.BuildFlow("TestFlow1");

            flow.Should().NotBeNull();
        }

        [Test]
        public void Simple_Flow_Is_Built_With_NodeFactory()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().GetTypeInfo().Assembly, true);

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var component = flowBuilder.RootComponent;

            var flow = factory.BuildFlow(component);

            flow.Should().NotBeNull();
        }

        [Test]
        public void Simple_Flow_Is_Built_With_NodeFactory_And_Json_Serializer()
        {
            Json.Registrar.RegisterAsDefault();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().GetTypeInfo().Assembly, true);

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var component = flowBuilder.RootComponent;

            var serialized = SerializerProvider.Serializer.Serialize(component);

            var flow = factory.BuildSerializedFlow(serialized);

            flow.Should().NotBeNull();
        }



        [Test]
        public void Flow_Builder_Is_Hydrated_And_Flow_Built_From_Json_Serializer()
        {
            Json.Registrar.RegisterAsDefault();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().GetTypeInfo().Assembly, true);

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var serialized = flowBuilder.SerializeRootComponent();

            var component = flowBuilder.DeserializeAndSetRootComponent(serialized);

            var flow = factory.BuildFlow(component);

            flow.Should().NotBeNull();
        }


        [Test]
        public void Simple_Flow_Contains_All_Nodes()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().GetTypeInfo().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<PipelineNode<object>>()
                .AddChild<ITestNode2>()
                .AddChild<IPipelineNode<object>>()
                    .ForLastChild()
                    .AddChild<ITestNode4>()
                    .AddChild<ITestNode3>()
                    .AddChild<ITestNode2>()
                .ForParent()
                .AddChild<ITestNode3>();
                
            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flow = (IPipelineNode<object>)factory.BuildFlow("TestFlow1");

            flow.Should().BeOfType<PipelineNode<object>>();
            flow.Children.Count.Should().Be(3);
            var subflow = (IPipelineNode<object>)flow.Children[1];
            subflow.Children.Count.Should().Be(3);
            subflow.Children[1].Should().BeOfType<TestNode3>();
        }

        [Test]
        public void Multiple_Flows_Can_Be_Registered()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().GetTypeInfo().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow2")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode4>()
                .AddChild<ITestNode3>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flow = factory.BuildFlow("TestFlow1");
            flow.Should().NotBeNull();

            flow = factory.BuildFlow("TestFlow2");
            flow.Should().NotBeNull();
        }

        [Test]
        public void Simple_Flow_Contains_Subflow()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().GetTypeInfo().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow2")
                .AddRoot<PipelineNode<object>>()
                .AddChild<ITestNode4>()
                .AddChild<ITestNode3>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<PipelineNode<object>>()
                .AddChild<ITestNode2>()
                .AddFlow("TestFlow2");
                
            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flow = (IPipelineNode<object>)factory.BuildFlow("TestFlow1");

            flow.Should().BeOfType<PipelineNode<object>>();
            flow.Children.Count.Should().Be(2);
            var subflow = (IPipelineNode<object>)flow.Children[1];
            subflow.Children.Count.Should().Be(3);
            subflow.Children[1].Should().BeOfType<TestNode3>();
        }

        [Test]
        public void Adding_Child_Node_To_Simple_Node_Errs()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().GetTypeInfo().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            var componentBuilder = flowBuilder.CreateFlow("TestFlow")
                .AddRoot<ITestNode4>();

            Assert.Throws<InvalidOperationException>(() => componentBuilder.AddChild<ITestNode4>());

        }

        [Test]
        public void Adding_Child_Flow_To_Simple_Node_Errs()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().GetTypeInfo().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow2")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode4>()
                .AddChild<ITestNode3>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var componentBuilder = flowBuilder.CreateFlow("TestFlow")
                .AddRoot<ITestNode4>();

            Assert.Throws<InvalidOperationException>(() => componentBuilder.AddFlow("TestFlow2"));

        }

    }
}