using Autofac;
using Banzai.Autofac;
using Banzai.Factories;
using NUnit.Framework;
using Should;

namespace Banzai.Json.Test
{
    [TestFixture]
    public class WhenSerializingBasicFlow
    {
        [Test]
        public void Simple_Flow_Is_Serialized()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestJsNode>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            definition.ShouldNotBeNull().ShouldNotBeEmpty();
           
        }

        [Test]
        public void Simple_Flow_Is_Deserialized()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestJsNode>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            FlowComponent<object> deserializedComponent = serializer.Deserialize<object>(definition);

            deserializedComponent.ShouldNotBeNull();

            deserializedComponent.Children[0].Children.Count.ShouldEqual(2);

        }

        [Test]
        public async void Deserialized_Flow_Component_Can_Be_Built_And_Run()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            FlowComponent<object> deserializedComponent = serializer.Deserialize<object>(definition);

            flowBuilder.RootComponent = deserializedComponent;

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowRootNode = factory.GetFlow("TestFlow1");

            NodeResult result = await flowRootNode.ExecuteAsync(new object());

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
        }

    }
}