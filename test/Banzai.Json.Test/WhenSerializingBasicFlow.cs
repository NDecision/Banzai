using System;
using System.Threading.Tasks;
using Autofac;
using Banzai.Autofac;
using Banzai.Factories;
using Banzai.Serialization;
using NUnit.Framework;
using FluentAssertions;

namespace Banzai.Json.Test
{
    [TestFixture, Explicit]
    public class WhenSerializingBasicFlow
    {
        [SetUp]
        public void Setup()
        {
            //TypeAbbreviationCache.Clear();
        }

        [Test]
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

            Console.WriteLine(definition);

            definition.Should().NotBeNullOrEmpty();

        }

        [Test]
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

            Console.WriteLine(definition);

            definition.Should().NotBeNullOrEmpty();
            definition.Should().NotContain("Banzai.Json.Test");

        }

        [Test]
        public void Simple_Flow_Is_Serialized_With_Full_Abbreviations()
        {
            TypeAbbreviationCache.Clear();
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

            Console.WriteLine(definition);

            definition.Should().NotBeNullOrEmpty();
            definition.Should().Contain("Banzai.Json.Test");
        }

        [Test]
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

            Console.WriteLine(definition);

            definition.Should().NotBeNullOrEmpty();
            definition.Should().Contain("\"ShouldExecuteBlockType\":\"ShouldNotExecuteTestBlock\"");
        }

        [Test]
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

            Console.WriteLine(definition);

            definition.Should().NotBeNullOrEmpty();
            definition.Should().Contain("\"ShouldExecuteBlockType\":\"Banzai.Json.Test.ShouldNotExecuteTestBlock\"");
        }

        [Test]
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

        [Test]
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

        [Test]
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

            Console.WriteLine(definition);

            FlowComponent<object> deserializedComponent = serializer.Deserialize<object>(definition);

            flowBuilder.RootComponent = deserializedComponent;

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");

            NodeResult result = await flowRootNode.ExecuteAsync(new object());

            result.Status.Should().Be(NodeResultStatus.Succeeded);
        }


        [Test]
        public async Task Deserialized_Flow_Component_With_ShouldExecuteBlock_Can_Be_Built_And_Attempted()
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

            string definition = serializer.Serialize(rootComponent);

            FlowComponent<object> deserializedComponent = serializer.Deserialize<object>(definition);

            flowBuilder.RootComponent = deserializedComponent;

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<object>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");

            NodeResult result = await flowRootNode.ExecuteAsync(new object());

            result.Status.Should().Be(NodeResultStatus.NotRun);
        }

    }
}