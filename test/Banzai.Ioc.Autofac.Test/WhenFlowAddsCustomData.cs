using Autofac;
using Banzai.Factories;
using Xunit;

namespace Banzai.Ioc.Autofac.Test
{
    public class WhenFlowAddsCustomData
    {
        [Fact]
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
            Assert.Equal(flowRootNode.CustomData, "TestData");
        }

        [Fact]
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
            Assert.Equal(flowRootNode.CustomData.Test, "Test");
            Assert.Equal(flowRootNode.CustomData.Thang, "Thang");
        }

    }
}