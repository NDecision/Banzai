using Ninject;
using Banzai.Factories;
using NUnit.Framework;
using Should;

namespace Banzai.Ninject.Test
{
    [TestFixture]
    public class WhenFlowAddsShouldExecuteBlock
    {
        [Test]
        public void ShouldExecute_Is_Added_To_Root_Node()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetShouldExecuteBlock<ShouldNotExecuteTestBlock>()
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flowRootNode = factory.GetFlow("TestFlow1");
            flowRootNode.ShouldExecuteBlock.ShouldNotBeNull();
            ((IShouldExecuteBlock<object>) flowRootNode.ShouldExecuteBlock).ShouldExecute(new ExecutionContext<object>(new object())).ShouldBeFalse();
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
                .SetShouldExecuteBlock<ShouldNotExecuteTestBlock>();

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = (IPipelineNode<object>) factory.GetFlow("TestFlow1");

            var subflow = flow.Children[0];
            subflow.ShouldExecuteBlock.ShouldNotBeNull();
            ((IShouldExecuteBlock<object>) subflow.ShouldExecuteBlock).ShouldExecute(new ExecutionContext<object>(new object())).ShouldBeFalse();
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
                .ForChildFlow("TestFlow2").SetShouldExecuteBlock<ShouldNotExecuteTestBlock>();

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flow = (IPipelineNode<object>) factory.GetFlow("TestFlow1");

            var subflowRoot = (IPipelineNode<object>) flow.Children[1];
            subflowRoot.ShouldExecuteBlock.ShouldNotBeNull();
            ((IShouldExecuteBlock<object>) subflowRoot.ShouldExecuteBlock).ShouldExecute(new ExecutionContext<object>(new object())).ShouldBeFalse();
        }
    }
}