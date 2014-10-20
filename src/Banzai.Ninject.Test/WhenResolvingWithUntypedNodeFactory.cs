using System.Linq;
using Ninject;
using Banzai.Factories;
using NUnit.Framework;
using Should;

namespace Banzai.Ninject.Test
{
    [TestFixture]
    public class WhenResolvingWithUntypedNodeFactory
    {
        [Test]
        public void Node_Is_Retrieved_As_Self()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory>();
            var node = nodeFactory.GetNode<TestNode>();

            node.ShouldNotBeNull();
        }

        [Test]
        public void Node_Is_Retrieved_As_Primary_Interface()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory>();
            var node = nodeFactory.GetNode<ITestNode<object>>();

            node.ShouldNotBeNull();
        }

        [Test]
        public void All_INodes_Are_Retrieved()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory>();
            var nodes = nodeFactory.GetAllNodes<INode<object>>().ToList();

            nodes.Count.ShouldBeGreaterThan(1);
        }

        [Test]
        public void Resolution_Of_Named_Node_Succeeds()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            kernel.Bind<ITestNode<object>>().To<TestNode>().Named("TestName");

            var nodeFactory = kernel.Get<INodeFactory>();
            var node = nodeFactory.GetNode<ITestNode<object>>("TestName");

            node.ShouldNotBeNull();

            Assert.Throws<ActivationException>(() => nodeFactory.GetNode<ITestNode<object>>("TestName2"));
        }

        [Test]
        public void Resolution_Of_Transition_Node_Succeeds()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var nodeFactory = kernel.Get<INodeFactory>();
            var node = nodeFactory.GetNode<ITestTransitionNode1>();

            node.ShouldNotBeNull();
        }


    }

}