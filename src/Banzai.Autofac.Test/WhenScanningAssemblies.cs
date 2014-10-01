using Autofac;
using NUnit.Framework;
using Should;

namespace Banzai.Autofac.Test
{
    [TestFixture]
    public class WhenScanningAssemblies
    {
        [Test]
        public void Node_Is_Registered_As_Self()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var node = container.Resolve<TestNode>();
            node.ShouldNotBeNull();
        }

        [Test]
        public void Nodes_Are_Registered_As_IClassName()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var node = container.Resolve<ITestNode2>();
            node.ShouldNotBeNull();
        }

        [Test]
        public void Nodes_Are_Registered_As_IClassName_Generic()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var node = container.Resolve<ITestNode<object>>();
            node.ShouldNotBeNull();
        }

        [Test]
        public void Group_Node_Is_Registered_When_Core_Registered()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var node = container.Resolve<IGroupNode<object>>();
            node.ShouldNotBeNull();
            node = container.Resolve<GroupNode<object>>();
            node.ShouldNotBeNull();
        }

        [Test]
        public void Pipeline_Node_Is_Registered_When_Core_Registered()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var node = container.Resolve<IPipelineNode<object>>();
            node.ShouldNotBeNull();
            node = container.Resolve<PipelineNode<object>>();
            node.ShouldNotBeNull();
        }

        [Test]
        public void FirstMatch_Node_Is_Registered_When_Core_Registered()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var node = container.Resolve<IFirstMatchNode<object>>();
            node.ShouldNotBeNull();
            node = container.Resolve<FirstMatchNode<object>>();
            node.ShouldNotBeNull();
        }

    }
}