using System.Linq;
using Banzai.Factories;
using FluentAssertions;
using Ninject;
using Xunit;

namespace Banzai.Ioc.Ninject.Test
{
    public class WhenResolvingWithUntypedNodeFactory
    {
        [Fact]
        public void Node_Is_Retrieved_As_Self()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory>();
            var node = nodeFactory.GetNode<TestNode>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void Node_Is_Retrieved_As_Primary_Interface()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory>();
            var node = nodeFactory.GetNode<ITestNode<object>>();

            node.Should().NotBeNull();
        }

        [Fact]
        public void All_INodes_Are_Retrieved()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory>();
            var nodes = nodeFactory.GetAllNodes<INode<object>>().ToList();

            nodes.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        public void Resolution_Of_Named_Node_Succeeds()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            kernel.Bind<ITestNode<object>>().To<TestNode>().Named("TestName");

            var nodeFactory = kernel.Get<INodeFactory>();
            var node = nodeFactory.GetNode<ITestNode<object>>("TestName");

            node.Should().NotBeNull();

            Assert.Throws<ActivationException>(() => nodeFactory.GetNode<ITestNode<object>>("TestName2"));
        }

        [Fact]
        public void Resolution_Of_Transition_Node_Succeeds()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory>();
            var node = nodeFactory.GetNode<ITestTransitionNode1>();

            node.Should().NotBeNull();
        }
    }
}