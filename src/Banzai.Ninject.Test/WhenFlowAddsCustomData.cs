using Banzai.Factories;
using Ninject;
using NUnit.Framework;
using Should;

namespace Banzai.Ninject.Test
{

    [TestFixture]
    public class WhenFlowAddsCustomData
    {
        [Test]
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

            var flowRootNode = factory.GetFlow("TestFlow1");
            ObjectAssertExtensions.ShouldEqual(flowRootNode.CustomData, "TestData");
        }

        [Test]
        public void Custom_Anonymous_Object_Is_Set_On_Node()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new NinjectFlowRegistrar(kernel));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetCustomData(new{Test="Test", Thang="Thang"})
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var factory = kernel.Get<INodeFactory<object>>();

            var flowRootNode = factory.GetFlow("TestFlow1");
            ObjectAssertExtensions.ShouldEqual(flowRootNode.CustomData.Test, "Test");
            ObjectAssertExtensions.ShouldEqual(flowRootNode.CustomData.Thang, "Thang");
        }

    }
}