using Autofac;
using Banzai.Factories;
using NUnit.Framework;
using Should;

namespace Banzai.Autofac.Test
{

    [TestFixture]
    public class WhenFlowAddsCustomData
    {
        [Test]
        public void Custom_String_Data_Is_Set_On_Node()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetCustomData("TestData")
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");
            ObjectAssertExtensions.ShouldEqual(flowRootNode.CustomData, "TestData");
        }

        [Test]
        public void Custom_Anonymous_Object_Is_Set_On_Node()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetCustomData(new{Test="Test", Thang="Thang"})
                .AddChild<ITestNode2>();

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");
            ObjectAssertExtensions.ShouldEqual(flowRootNode.CustomData.Test, "Test");
            ObjectAssertExtensions.ShouldEqual(flowRootNode.CustomData.Thang, "Thang");
        }

    }
}