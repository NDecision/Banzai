using Ninject;
using NUnit.Framework;
using Should;

namespace Banzai.Ninject.Test
{
    [TestFixture]
    public class WhenScanningAssemblies
    {
        [Test]
        public void Node_Is_Registered_As_Self()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<TestNode>();
            node.ShouldNotBeNull();
        }

        [Test]
        public void Nodes_Are_Registered_As_IClassName()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<ITestNode2>();
            node.ShouldNotBeNull();
        }

        [Test]
        public void Nodes_Are_Registered_As_IClassName_Generic()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<ITestNode<object>>();
            node.ShouldNotBeNull();
        }

        [Test]
        public void Group_Node_Is_Registered_When_Core_Registered()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<IGroupNode<object>>();
            node.ShouldNotBeNull();
            node = kernel.Get<GroupNode<object>>();
            node.ShouldNotBeNull();
        }

        [Test]
        public void Pipeline_Node_Is_Registered_When_Core_Registered()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<IPipelineNode<object>>();
            node.ShouldNotBeNull();
            node = kernel.Get<PipelineNode<object>>();
            node.ShouldNotBeNull();
        }

        [Test]
        public void FirstMatch_Node_Is_Registered_When_Core_Registered()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<IFirstMatchNode<object>>();
            node.ShouldNotBeNull();
            node = kernel.Get<FirstMatchNode<object>>();
            node.ShouldNotBeNull();
        }

    }
}