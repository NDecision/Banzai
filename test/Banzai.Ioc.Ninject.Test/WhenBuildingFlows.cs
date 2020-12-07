﻿using System;
using Banzai.Factories;
using Banzai.Serialization.JsonNet;
using FluentAssertions;
using Ninject;
using Xunit;

namespace Banzai.Ioc.Ninject.Test
{
    public class WhenBuildingFlows
    {
        [Fact]
        public void Simple_Flow_Is_Registered_With_Container()
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
        }

        [Fact]
        public void Simple_Flow_Is_Accessed_With_NodeFactory()
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

            flow.Should().NotBeNull();
        }

        [Fact]
        public void Simple_Flow_Is_Built_With_NodeFactory()
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

            flow.Should().NotBeNull();
        }

        [Fact]
        public void Simple_Flow_Is_Built_With_NodeFactory_And_Json_Serializer()
        {
            Registrar.RegisterAsDefault();
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var serialized = flowBuilder.SerializeRootComponent();

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = factory.BuildSerializedFlow(serialized);

            flow.Should().NotBeNull();
        }

        [Fact]
        public void Flow_Builder_Is_Hydrated_And_Flow_Built_From_Json_Serializer()
        {
            Registrar.RegisterAsDefault();
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var serialized = flowBuilder.SerializeRootComponent();

            var component = flowBuilder.DeserializeAndSetRootComponent(serialized);

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = factory.BuildFlow(component);

            flow.Should().NotBeNull();
        }

        [Fact]
        public void Simple_Flow_Contains_All_Nodes()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

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

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = (IPipelineNode<object>) factory.BuildFlow("TestFlow1");

            flow.Should().BeOfType<PipelineNode<object>>();
            flow.Children.Count.Should().Be(3);
            var subflow = (IPipelineNode<object>) flow.Children[1];
            subflow.Children.Count.Should().Be(3);
            subflow.Children[1].Should().BeOfType<TestNode3>();
        }

        [Fact]
        public void Multiple_Flows_Can_Be_Registered()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

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

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = factory.BuildFlow("TestFlow1");
            flow.Should().NotBeNull();

            flow = factory.BuildFlow("TestFlow2");
            flow.Should().NotBeNull();
        }

        [Fact]
        public void Simple_Flow_Contains_Subflow()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

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

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = (IPipelineNode<object>) factory.BuildFlow("TestFlow1");

            flow.Should().BeOfType<PipelineNode<object>>();
            flow.Children.Count.Should().Be(2);
            var subflow = (IPipelineNode<object>) flow.Children[1];
            subflow.Children.Count.Should().Be(3);
            subflow.Children[1].Should().BeOfType<TestNode3>();
        }

        [Fact]
        public void Adding_Child_Node_To_Simple_Node_Errs()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            var componentBuilder = flowBuilder.CreateFlow("TestFlow")
                .AddRoot<ITestNode4>();

            Assert.Throws<InvalidOperationException>(() => componentBuilder.AddChild<ITestNode4>());
        }

        [Fact]
        public void Adding_Child_Flow_To_Simple_Node_Errs()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

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