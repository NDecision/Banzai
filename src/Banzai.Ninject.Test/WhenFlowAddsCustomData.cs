using Banzai.Factories;
using FluentAssertions;
using Ninject;
using NUnit.Framework;

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

            var flowRootNode = factory.BuildFlow("TestFlow1");
            var testData = flowRootNode.CustomData as string;
            testData.Should().Be("TestData");
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

            var flowRootNode = factory.BuildFlow("TestFlow1");
            var test = flowRootNode.CustomData.Test as string;
            var thang = flowRootNode.CustomData.Thang as string;
            test.Should().Be("Test");
            thang.Should().Be("Thang");
        }

    }
}