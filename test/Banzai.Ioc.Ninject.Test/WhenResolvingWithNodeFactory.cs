﻿using System.Linq;
using System.Threading.Tasks;
using Banzai.Factories;
using FluentAssertions;
using Ninject;
using Xunit;

namespace Banzai.Ioc.Ninject.Test
{
    public class WhenResolvingWithNodeFactory
    {
        [Fact]
        public void Node_Is_Retrieved_As_Self()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory<object>>();
            var node = nodeFactory.GetNode<TestNode>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void Node_Is_Retrieved_As_Primary_Interface()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory<object>>();
            var node = nodeFactory.GetNode<ITestNode<object>>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void All_INodes_Are_Retrieved()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory<object>>();
            var nodes = nodeFactory.GetAllNodes<INode<object>>().ToList();

            nodes.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        public void Resolution_Of_Named_Node_Succeeds()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            kernel.Bind<ITestNode<object>>().To<TestNode>().Named("TestName");

            var nodeFactory = kernel.Get<INodeFactory<object>>();
            var node = nodeFactory.GetNode<ITestNode<object>>("TestName");

            node.Should().NotBeNull();

            Assert.Throws<ActivationException>(() => nodeFactory.GetNode<ITestNode<object>>("TestName2"));
        }

        [Fact]
        public void NodeFactory_Is_Automatically_Set_On_MultiNode()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var pipelineNode = kernel.Get<PipelineNode<object>>();

            pipelineNode.NodeFactory.Should().NotBeNull();
        }

        [Fact]
        public void NodeFactory_On_MultiNode_Can_Resolve_Other_Nodes()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var pipelineNode = kernel.Get<TestPipelineNode1>();

            var retrievedNode = pipelineNode.NodeFactory.GetNode<ITestNode2>();

            retrievedNode.Should().NotBeNull();
        }

        [Fact]
        public async Task NodeFactory_Is_Available_In_OnBeforeExecute()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var pipelineNode = kernel.Get<TestPipelineNode1>();

            var result = await pipelineNode.ExecuteAsync(new object());

            result.Status.Should().Be(NodeResultStatus.Succeeded);
        }


        [Fact]
        public void Resolution_Of_Transition_Node_Succeeds()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory<TestObjectA>>();
            var node = nodeFactory.GetNode<ITestTransitionNode1>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void Resolution_Of_Transition_As_Self_Node_Succeeds()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory<TestObjectA>>();
            var node = nodeFactory.GetNode<TestTransitionNode1>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void Resolution_Of_Transition_Func_Node_Succeeds()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory<TestObjectA>>();
            var node = nodeFactory.GetNode<ITransitionFuncNode<TestObjectA, TestObjectA>>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void Resolution_Of_Transition_Func_Node_As_Self_Succeeds()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory<TestObjectA>>();
            var node = nodeFactory.GetNode<TransitionFuncNode<TestObjectA, TestObjectA>>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void Resolution_Of_Transition_Node_Succeeds_From_Pipeline()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory<TestObjectA>>();
            var node = nodeFactory.GetNode<ITestPipelineNode2>();

            var childNode = node.NodeFactory.GetNode<ITestTransitionNode1>();

            childNode.Should().NotBeNull();
        }

        [Fact]
        public void Resolved_Transition_Node_Has_NodeFactory()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory<TestObjectA>>();
            var node = nodeFactory.GetNode<ITestPipelineNode2>();

            var childNode = node.NodeFactory.GetNode<ITestTransitionNode1>();

            childNode.NodeFactory.Should().NotBeNull();
        }
    }
}