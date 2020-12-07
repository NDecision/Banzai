using System;
using System.Threading.Tasks;
using Autofac;
using Banzai.Ioc.Autofac;
using Banzai.Factories;
using Banzai.Serialization.JsonNet;
using Banzai.Serialization;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;

namespace Banzai.Serialization.JsonNet.Test
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
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, failOnCollision:false);
            
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
            definition.Should().NotContain("Banzai.Serialization.JsonNet.Test");

        }

        [Fact]
        public void Simple_Flow_Is_Serialized_With_Full_Abbreviations()
        {
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, useFullName:true, failOnCollision: false);

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
            definition.Should().Contain("Banzai.Serialization.JsonNet.Test");
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
            TypeAbbreviationCache.RegisterFromAssembly(GetType().Assembly, useFullName:true, failOnCollision: false);

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
            definition.Should().Contain("\"ShouldExecuteBlockType\":\"Banzai.Serialization.JsonNet.Test.ShouldNotExecuteTestBlock\"");
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

            FlowComponent<object> deserializedComponent = serializer.Deserialize<object>(definition);

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

            FlowComponent<object> deserializedComponent = serializer.Deserialize<object>(definition);

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

            string definition = serializer.Serialize(rootComponent);

            _testOutputHelper.WriteLine(definition);

            FlowComponent<object> deserializedComponent = serializer.Deserialize<object>(definition);

            flowBuilder.RootComponent = deserializedComponent;

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");

            NodeResult result = await flowRootNode.ExecuteAsync(new object());

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

            string definition = serializer.Serialize(rootComponent);

            FlowComponent<object> deserializedComponent = serializer.Deserialize<object>(definition);

            flowBuilder.RootComponent = deserializedComponent;

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlowDeserializeShouldExecute");

            NodeResult result = await flowRootNode.ExecuteAsync(new object());

            result.Status.Should().Be(NodeResultStatus.NotRun);
        }

    }
}