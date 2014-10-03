using Autofac;
using Banzai.Factories;
using NUnit.Framework;
using Should;

namespace Banzai.Autofac.Test
{
    [TestFixture]
    public class WhenBuildingFlows
    {
        [Test]
        public void Simple_Flow_Is_Registered_With_Container()
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

            flow.ShouldNotBeNull();
            flow.IsFlow.ShouldBeTrue();
        }

        [Test]
        public void Simple_Flow_Is_Accessed_With_NodeFactory()
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

            var flow = factory.GetFlow("TestFlow1");

            flow.ShouldNotBeNull();
        }

        [Test]
        public void Simple_Flow_Contains_All_Nodes()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>()
                .AddChild<IPipelineNode<object>>()
                .ForChild<IPipelineNode<object>>()
                .AddChild<ITestNode4>()
                .AddChild<ITestNode3>()
                .AddChild<ITestNode2>();
                
            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flow = (IPipelineNode<object>)factory.GetFlow("TestFlow1");

            flow.ShouldBeType<PipelineNode<object>>();
            flow.Children.Count.ShouldEqual(2);
            var subflow = (IPipelineNode<object>)flow.Children[1];
            subflow.Children.Count.ShouldEqual(3);
            subflow.Children[1].ShouldBeType<TestNode3>();
        }

        [Test]
        public void Multiple_Flows_Can_Be_Registered()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

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

            var flow = factory.GetFlow("TestFlow1");
            flow.ShouldNotBeNull();

            flow = factory.GetFlow("TestFlow2");
            flow.ShouldNotBeNull();
        }

        [Test]
        public void Simple_Flow_Contains_Subflow()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow2")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode4>()
                .AddChild<ITestNode3>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>()
                .AddFlow("TestFlow2");
                
            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flow = (IPipelineNode<object>)factory.GetFlow("TestFlow1");

            flow.ShouldBeType<PipelineNode<object>>();
            flow.Children.Count.ShouldEqual(2);
            var subflow = (IPipelineNode<object>)flow.Children[1];
            subflow.Children.Count.ShouldEqual(3);
            subflow.Children[1].ShouldBeType<TestNode3>();
        }

    }
}