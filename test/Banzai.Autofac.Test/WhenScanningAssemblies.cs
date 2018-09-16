using Autofac;
using NUnit.Framework;
using FluentAssertions;

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
            node.Should().NotBeNull();
        }

        [Test]
        public void Nodes_Are_Registered_As_IClassName()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var node = container.Resolve<ITestNode2>();
            node.Should().NotBeNull();
        }

        [Test]
        public void Nodes_Are_Registered_As_IClassName_Generic()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var node = container.Resolve<ITestNode<object>>();
            node.Should().NotBeNull();
        }

        [Test]
        public void Group_Node_Is_Registered_When_Core_Registered()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var node = container.Resolve<IGroupNode<object>>();
            node.Should().NotBeNull();
            node = container.Resolve<GroupNode<object>>();
            node.Should().NotBeNull();
        }

        [Test]
        public void Pipeline_Node_Is_Registered_When_Core_Registered()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var node = container.Resolve<IPipelineNode<object>>();
            node.Should().NotBeNull();
            node = container.Resolve<PipelineNode<object>>();
            node.Should().NotBeNull();
        }

        [Test]
        public void FirstMatch_Node_Is_Registered_When_Core_Registered()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();

            var node = container.Resolve<IFirstMatchNode<object>>();
            node.Should().NotBeNull();
            node = container.Resolve<FirstMatchNode<object>>();
            node.Should().NotBeNull();
        }

    }
}