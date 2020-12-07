using Banzai.Factories;
using Ninject;
using Xunit;

namespace Banzai.Ioc.Ninject.Test
{
    public class WhenFlowAddsCustomData
    {
        [Fact]
        public void Custom_String_Data_Is_Set_On_Node()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetCustomData("TestData")
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");
            Assert.Equal(flowRootNode.CustomData, "TestData");
        }

        [Fact]
        public void Custom_Anonymous_Object_Is_Set_On_Node()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetCustomData(new {Test = "Test", Thang = "Thang"})
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");
            Assert.Equal(flowRootNode.CustomData.Test, "Test");
            Assert.Equal(flowRootNode.CustomData.Thang, "Thang");
        }
    }
}