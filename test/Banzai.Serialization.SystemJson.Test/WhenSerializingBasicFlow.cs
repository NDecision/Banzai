using System.Threading.Tasks;
using Autofac;
using Banzai.Ioc.Autofac;
using Banzai.Factories;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Banzai.Serialization.SystemJson.Test
{
    public class WhenSerializingBasicFlow
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public WhenSerializingBasicFlow(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            TypeAbbreviationCache.Clear();
            TypeAbbreviationCache.RegisterCoreTypes();
        }

        [Fact]
        public void Simple_Flow_Is_Serialized()
        {
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, failOnCollision: false);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            _testOutputHelper.WriteLine(definition);

            definition.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Simple_Flow_Is_Serialized_With_Abbreviations()
        {
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, failOnCollision: false);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            _testOutputHelper.WriteLine(definition);

            definition.Should().NotBeNullOrEmpty();
            definition.Should().NotContain("Banzai.Serialization.SystemJson.Test");
        }

        [Fact]
        public void Simple_Flow_Is_Serialized_With_Full_Abbreviations()
        {
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, true, false);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            _testOutputHelper.WriteLine(definition);

            definition.Should().NotBeNullOrEmpty();
            definition.Should().Contain("Banzai.Serialization.SystemJson.Test");
        }

        [Fact]
        public void Flow_With_ShouldExecuteBlock_Is_Serialized_With_Abbreviations()
        {
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, failOnCollision: false);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetShouldExecuteBlock<ShouldNotExecuteTestBlock>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            _testOutputHelper.WriteLine(definition);

            definition.Should().NotBeNullOrEmpty();
            definition.Should().Contain("\"ShouldExecuteBlockType\":\"ShouldNotExecuteTestBlock\"");
        }

        [Fact]
        public void Flow_With_ShouldExecuteBlock_Is_Serialized_With_Full_Abbreviations()
        {
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, true, false);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetShouldExecuteBlock<ShouldNotExecuteTestBlock>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            _testOutputHelper.WriteLine(definition);

            definition.Should().NotBeNullOrEmpty();
            definition.Should()
                .Contain(
                    "\"ShouldExecuteBlockType\":\"Banzai.Serialization.SystemJson.Test.ShouldNotExecuteTestBlock\"");
        }

        [Fact]
        public void Simple_Flow_Is_Deserialized()
        {
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, failOnCollision: false);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            var deserializedComponent = serializer.Deserialize<object>(definition);

            deserializedComponent.Should().NotBeNull();

            deserializedComponent.Children[0].Children.Count.Should().Be(1);
        }

        [Fact]
        public void Flow_With_ShouldExecuteBlock_Is_Deserialized()
        {
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, failOnCollision: false);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>().SetShouldExecuteBlock<ShouldNotExecuteTestBlock>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            var deserializedComponent = serializer.Deserialize<object>(definition);

            deserializedComponent.Should().NotBeNull();
            deserializedComponent.Children[0].ShouldExecuteBlockType.Should().Be(typeof(ShouldNotExecuteTestBlock));
        }

        [Fact]
        public async Task Deserialized_Flow_Component_Can_Be_Built_And_Run()
        {
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, failOnCollision: false);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            _testOutputHelper.WriteLine(definition);

            var deserializedComponent = serializer.Deserialize<object>(definition);

            flowBuilder.RootComponent = deserializedComponent;

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");

            var result = await flowRootNode.ExecuteAsync(new object());

            result.Status.Should().Be(NodeResultStatus.Succeeded);
        }


        [Fact]
        public async Task Deserialized_Flow_Component_With_ShouldExecuteBlock_Can_Be_Built_And_Attempted()
        {
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, failOnCollision: false);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlowDeserializeShouldExecute")
                .AddRoot<IPipelineNode<object>>().SetShouldExecuteBlock<ShouldNotExecuteTestBlock>()
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            var deserializedComponent = serializer.Deserialize<object>(definition);

            flowBuilder.RootComponent = deserializedComponent;

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlowDeserializeShouldExecute");

            var result = await flowRootNode.ExecuteAsync(new object());

            result.Status.Should().Be(NodeResultStatus.NotRun);
        }
    }
}