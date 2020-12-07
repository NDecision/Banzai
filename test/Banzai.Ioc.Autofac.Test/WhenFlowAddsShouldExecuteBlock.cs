﻿using Autofac;
using Banzai.Factories;
using Xunit;
using FluentAssertions;

namespace Banzai.Ioc.Autofac.Test
{
    
    public class WhenFlowAddsShouldExecuteBlock
    {
        [Fact]
        public void ShouldExecute_Is_Added_To_Root_Node()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetShouldExecuteBlock<ShouldNotExecuteTestBlock>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");
            flowRootNode.ShouldExecuteBlock.Should().NotBeNull();
            ((IShouldExecuteBlock<object>)flowRootNode.ShouldExecuteBlock).ShouldExecuteAsync(new ExecutionContext<object>(new object())).Result.Should().BeFalse();
        }

        [Fact]
        public void ShouldExecute_Is_Added_To_Child_Node()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>()
                .ForChild<ITestNode2>()
                .SetShouldExecuteBlock<ShouldNotExecuteTestBlock>();

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flow = (IPipelineNode<object>)factory.BuildFlow("TestFlow1");

            var subflow = flow.Children[0];
            subflow.ShouldExecuteBlock.Should().NotBeNull();
            ((IShouldExecuteBlock<object>)subflow.ShouldExecuteBlock).ShouldExecuteAsync(new ExecutionContext<object>(new object())).Result.Should().BeFalse();
        }

        [Fact]
        public void Adding_ShouldExecute_To_Subflow_Applies_To_Subflow_Root()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

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
                .AddFlow("TestFlow2")
                .ForChildFlow("TestFlow2").SetShouldExecuteBlock<ShouldNotExecuteTestBlock>();

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flow = (IPipelineNode<object>)factory.BuildFlow("TestFlow1");

            var subflowRoot = (IPipelineNode<object>)flow.Children[1];
            subflowRoot.ShouldExecuteBlock.Should().NotBeNull();
            ((IShouldExecuteBlock<object>)subflowRoot.ShouldExecuteBlock).ShouldExecuteAsync(new ExecutionContext<object>(new object())).Result.Should().BeFalse();
        }

    }
}