using System.Linq;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenChangingTheSubject
    {
        [Test]
        public async void Node_May_Change_Context_Subject()
        {
            var testNode = new SubjectChangingNode1();
            var testObject = new TestObjectA();
            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);

            result.Subject.ShouldBeSameAs(context.Subject);
            result.Subject.ShouldNotBeSameAs(testObject);
            ((TestObjectA)result.Subject).TestValueString.ShouldEqual("New Instance");
        }

        [Test]
        public async void Pipeline_Node_Results_Following_Subject_Change_Node_Return_Changed_Subject()
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

            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);

            var childResults = result.ChildResults.ToList();

            childResults[0].Subject.ShouldBeSameAs(testObject);
            childResults[1].Subject.ShouldNotBeSameAs(testObject);
            childResults[2].Subject.ShouldNotBeSameAs(testObject);
            childResults[1].Subject.ShouldEqual(childResults[2].Subject);
        }

        [Test]
        public async void Pipeline_Overall_Result_Subject_Equals_Changed_Subject()
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

            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);

            var childResults = result.ChildResults.ToList();

            result.Subject.ShouldNotBeSameAs(testObject);
            result.Subject.ShouldBeSameAs(childResults[1].Subject);
        }

        [Test]
        public async void Pipeline_Overall_Result_Subject_Equals_Last_Changed_Subject()
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

            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);

            var childResults = result.ChildResults.ToList();

            result.Subject.ShouldNotBeSameAs(testObject);
            result.Subject.ShouldNotBeSameAs(childResults[1].Subject);
            result.Subject.ShouldNotBeSameAs(childResults[2].Subject);
            result.Subject.ShouldBeSameAs(childResults[3].Subject);
        }


        [Test]
        public async void Group_Overall_Result_Subject_Equals_Changed_Subject()
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

            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);

            var childResults = result.ChildResults.ToList();

            result.Subject.ShouldNotBeSameAs(testObject);
            result.Subject.ShouldBeSameAs(childResults[1].Subject);
        }

        [Test]
        public async void Group_Overall_Result_Subject_Equals_Last_Changed_Subject()
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

            groupNode.Status.ShouldEqual(NodeRunStatus.Completed);

            var childResults = result.ChildResults.ToList();

            result.Subject.ShouldNotBeSameAs(testObject);
            result.Subject.ShouldNotBeSameAs(childResults[1].Subject);
            result.Subject.ShouldNotBeSameAs(childResults[2].Subject);
            result.Subject.ShouldBeSameAs(childResults[3].Subject);
        }

    }
}