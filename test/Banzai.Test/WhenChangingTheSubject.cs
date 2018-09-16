using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenChangingTheSubject
    {
        [Test]
        public async Task Node_May_Change_Context_Subject()
        {
            var testNode = new SubjectChangingNode1();
            var testObject = new TestObjectA();
            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.Status.Should().Be(NodeResultStatus.Succeeded);

            result.Subject.Should().BeSameAs(context.Subject);
            result.Subject.Should().NotBeSameAs(testObject);
            result.GetSubjectAs<TestObjectA>().TestValueString.Should().Be("New Instance");
        }

        [Test]
        public async Task Pipeline_Node_Results_Following_Subject_Change_Node_Return_Changed_Subject()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            var node1 = new SimpleTestNodeA1();
            var node2 = new SubjectChangingNode1();
            var node3 = new SimpleTestNodeA2();

            pipelineNode.AddChild(node1);
            pipelineNode.AddChild(node2);
            pipelineNode.AddChild(node3);

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);

            var childResults = result.ChildResults.ToList();

            childResults[0].Subject.Should().BeSameAs(testObject);
            childResults[1].Subject.Should().NotBeSameAs(testObject);
            childResults[2].Subject.Should().NotBeSameAs(testObject);
            childResults[1].Subject.Should().Be(childResults[2].Subject);
        }

        [Test]
        public async Task Pipeline_Overall_Result_Subject_Equals_Changed_Subject()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            var node1 = new SimpleTestNodeA1();
            var node2 = new SubjectChangingNode1();
            var node3 = new SimpleTestNodeA2();

            pipelineNode.AddChild(node1);
            pipelineNode.AddChild(node2);
            pipelineNode.AddChild(node3);

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);

            var childResults = result.ChildResults.ToList();

            result.Subject.Should().NotBeSameAs(testObject);
            result.Subject.Should().BeSameAs(childResults[1].Subject);
        }

        [Test]
        public async Task Pipeline_Overall_Result_Subject_Equals_Last_Changed_Subject()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            var node1 = new SimpleTestNodeA1();
            var node2 = new SubjectChangingNode1();
            var node3 = new SimpleTestNodeA2();
            var node4 = new SubjectChangingNode1();

            pipelineNode.AddChild(node1);
            pipelineNode.AddChild(node2);
            pipelineNode.AddChild(node3);
            pipelineNode.AddChild(node4);

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            pipelineNode.Status.Should().Be(NodeRunStatus.Completed);

            var childResults = result.ChildResults.ToList();

            result.Subject.Should().NotBeSameAs(testObject);
            result.Subject.Should().NotBeSameAs(childResults[1].Subject);
            result.Subject.Should().NotBeSameAs(childResults[2].Subject);
            result.Subject.Should().BeSameAs(childResults[3].Subject);
        }


        [Test]
        public async Task Group_Overall_Result_Subject_Equals_Changed_Subject()
        {
            var groupNode = new GroupNode<TestObjectA>();

            var node1 = new SimpleTestNodeA1();
            var node2 = new SubjectChangingNode1();
            var node3 = new SimpleTestNodeA2();

            groupNode.AddChild(node1);
            groupNode.AddChild(node2);
            groupNode.AddChild(node3);

            var testObject = new TestObjectA();
            NodeResult result = await groupNode.ExecuteAsync(testObject);

            groupNode.Status.Should().Be(NodeRunStatus.Completed);

            var childResults = result.ChildResults.ToList();

            result.Subject.Should().NotBeSameAs(testObject);
            result.Subject.Should().BeSameAs(childResults[1].Subject);
        }

        [Test]
        public async Task Group_Overall_Result_Subject_Equals_Last_Changed_Subject()
        {
            var groupNode = new GroupNode<TestObjectA>();

            var node1 = new SimpleTestNodeA1();
            var node2 = new SubjectChangingNode1();
            var node3 = new SimpleTestNodeA2();
            var node4 = new SubjectChangingNode1();

            groupNode.AddChild(node1);
            groupNode.AddChild(node2);
            groupNode.AddChild(node3);
            groupNode.AddChild(node4);

            var testObject = new TestObjectA();
            NodeResult result = await groupNode.ExecuteAsync(testObject);

            groupNode.Status.Should().Be(NodeRunStatus.Completed);

            var childResults = result.ChildResults.ToList();

            result.Subject.Should().NotBeSameAs(testObject);
            result.Subject.Should().NotBeSameAs(childResults[1].Subject);
            result.Subject.Should().NotBeSameAs(childResults[2].Subject);
            result.Subject.Should().BeSameAs(childResults[3].Subject);
        }

    }
}