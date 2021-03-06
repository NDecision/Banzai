﻿using FluentAssertions;
using Ninject;
using Xunit;

namespace Banzai.Ioc.Ninject.Test
{
    public class WhenScanningAssemblies
    {
        [Fact]
        public void Node_Is_Registered_As_Self()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<TestNode>();
            node.Should().NotBeNull();
        }

        [Fact]
        public void Nodes_Are_Registered_As_IClassName()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<ITestNode2>();
            node.Should().NotBeNull();
        }

        [Fact]
        public void Nodes_Are_Registered_As_IClassName_Generic()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<ITestNode<object>>();
            node.Should().NotBeNull();
        }

        [Fact]
        public void Group_Node_Is_Registered_When_Core_Registered()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<IGroupNode<object>>();
            node.Should().NotBeNull();
            node = kernel.Get<GroupNode<object>>();
            node.Should().NotBeNull();
        }

        [Fact]
        public void Pipeline_Node_Is_Registered_When_Core_Registered()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<IPipelineNode<object>>();
            node.Should().NotBeNull();
            node = kernel.Get<PipelineNode<object>>();
            node.Should().NotBeNull();
        }

        [Fact]
        public void FirstMatch_Node_Is_Registered_When_Core_Registered()
        {
            var kernel = new StandardKernel();
            kernel.RegisterBanzaiNodes(GetType().Assembly, true);

            var node = kernel.Get<IFirstMatchNode<object>>();
            node.Should().NotBeNull();
            node = kernel.Get<FirstMatchNode<object>>();
            node.Should().NotBeNull();
        }
    }
}