using System.Threading.Tasks;
using Ninject;
using Banzai.Factories;
using NUnit.Framework;
using FluentAssertions;

namespace Banzai.Ninject.Test
{
    [TestFixture]
    public class WhenFlowAddsShouldExecute
    {
        [Test]
        public void ShouldExecute_Is_Added_To_Root_Node()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetShouldExecute(ctxt => Task.FromResult(1 + 1 != 2))
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");
            flowRootNode.ShouldExecuteFunc.Should().NotBeNull();
            flowRootNode.ShouldExecuteFunc(new ExecutionContext<object>(new object())).Result.Should().BeFalse();
        }

        [Test]
        public void ShouldExecute_Is_Added_To_Child_Node()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>()
                .ForChild<ITestNode2>()
                .SetShouldExecute(ctxt => Task.FromResult(1 + 1 == 3));

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = (IPipelineNode<object>)factory.BuildFlow("TestFlow1");

            var subflow = flow.Children[0];
            subflow.ShouldExecuteFunc.Should().NotBeNull();
            subflow.ShouldExecuteFunc(new ExecutionContext<object>(new object())).Result.Should().BeFalse();
        }

        [Test]
        public async Task ShouldExecuteAsync_Is_Added_To_Root_Node()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetShouldExecute(ctxt => Task.FromResult(1 + 1 != 2))
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");
            flowRootNode.ShouldExecuteFunc.Should().NotBeNull();
            (await flowRootNode.ShouldExecuteFunc(new ExecutionContext<object>(new object()))).Should().BeFalse();
        }

        [Test]
        public async Task ShouldExecuteAsync_Is_Added_To_Child_Node()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>()
                .ForChild<ITestNode2>().SetShouldExecute(ctxt => Task.FromResult(1 + 1 != 2));

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = (IPipelineNode<object>)factory.BuildFlow("TestFlow1");

            var subflow = flow.Children[0];
            subflow.ShouldExecuteFunc.Should().NotBeNull();
            (await subflow.ShouldExecuteFunc(new ExecutionContext<object>(new object()))).Should().BeFalse();
        }

        [Test]
        public void Adding_ShouldExecute_To_Subflow_Applies_To_Subflow_Root()
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
                .AddFlow("TestFlow2")
                .ForChildFlow("TestFlow2").SetShouldExecute(ctxt => Task.FromResult(1 + 1 == 3));

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = (IPipelineNode<object>)factory.BuildFlow("TestFlow1");

            var subflowRoot = (IPipelineNode<object>)flow.Children[1];
            subflowRoot.ShouldExecuteFunc.Should().NotBeNull();
            subflowRoot.ShouldExecuteFunc(new ExecutionContext<object>(new object())).Result.Should().BeFalse();
        }

    }
}