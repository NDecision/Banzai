using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Registration;
using Banzai.Factories;
using Xunit;
using FluentAssertions;

namespace Banzai.Ioc.Autofac.Test
{

    
    public class WhenResolvingWithNodeFactory
    {
        [Fact]
        public void Node_Is_Retrieved_As_Self()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory<object>>();
            var node = nodeFactory.GetNode<TestNode>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void Node_Is_Retrieved_As_Primary_Interface()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory<object>>();
            var node = nodeFactory.GetNode<ITestNode<object>>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void All_INodes_Are_Retrieved()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory<object>>();
            var nodes = nodeFactory.GetAllNodes<INode<object>>().ToList();

            nodes.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        public void Resolution_Of_Named_Node_Succeeds()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            containerBuilder.RegisterType<TestNode>().Named<ITestNode<object>>("TestName");

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory<object>>();
            var node = nodeFactory.GetNode<ITestNode<object>>("TestName");

            node.Should().NotBeNull();

            Assert.Throws<ComponentNotRegisteredException>(() => nodeFactory.GetNode<ITestNode<object>>("TestName2"));
        }

        [Fact]
        public void NodeFactory_Is_Automatically_Set_On_MultiNode()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var pipelineNode = container.Resolve<PipelineNode<object>>();

            pipelineNode.NodeFactory.Should().NotBeNull();
        }

        [Fact]
        public void NodeFactory_On_MultiNode_Can_Resolve_Other_Nodes()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var pipelineNode = container.Resolve<TestPipelineNode1>();

            var retrievedNode = pipelineNode.NodeFactory.GetNode<ITestNode2>();

            retrievedNode.Should().NotBeNull();
        }

        [Fact]
        public async Task NodeFactory_Is_Available_In_OnBeforeExecute()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var pipelineNode = container.Resolve<TestPipelineNode1>();

            var result = await pipelineNode.ExecuteAsync(new object());

            result.Status.Should().Be(NodeResultStatus.Succeeded);
        }


        [Fact]
        public void Resolution_Of_Transition_Node_Succeeds()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory<TestObjectA>>();
            var node = nodeFactory.GetNode<ITestTransitionNode1>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void Resolution_Of_Transition_Func_Node_Succeeds()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory<TestObjectA>>();
            var node = nodeFactory.GetNode<ITransitionFuncNode<TestObjectA, TestObjectB>>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void Resolution_Of_Transition_Func_Node_As_Self_Succeeds()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory<TestObjectA>>();
            var node = nodeFactory.GetNode<TransitionFuncNode<TestObjectA, TestObjectB>>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void Resolution_Of_Transition_Node_Succeeds_From_Pipeline()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory<TestObjectA>>();
            var node = nodeFactory.GetNode<ITestPipelineNode2>();

            var childNode = node.NodeFactory.GetNode<ITestTransitionNode1>();

            childNode.Should().NotBeNull();
        }

        [Fact]
        public void Resolved_Transition_Node_Has_NodeFactory()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory<TestObjectA>>();
            var node = nodeFactory.GetNode<ITestPipelineNode2>();

            var childNode = node.NodeFactory.GetNode<ITestTransitionNode1>();

            childNode.NodeFactory.Should().NotBeNull();
        }


    }

}