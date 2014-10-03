using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenPassingState
    {
        [Test]
        public async void Adding_State_To_A_Node_Is_Available_In_Following_Node()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new NodeSync<TestObjectA> { ExecutedFunc = ctxt => { ctxt.State.Foo = "Bar"; return NodeResultStatus.Succeeded; } });
            pipelineNode.AddChild(new NodeSync<TestObjectA> { ExecutedFunc = ctxt => (ctxt.State.Foo == "Bar") ? NodeResultStatus.Succeeded : NodeResultStatus.Failed });

            var testObject = new TestObjectA();
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
        }

        [Test]
        public async void Adding_State_To_A_Node_Is_Available_In_Global_Context()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new NodeSync<TestObjectA> { ExecutedFunc = ctxt => { ctxt.State.Foo = "Bar"; return NodeResultStatus.Succeeded; } });

            var testObject = new TestObjectA();
            var context = new ExecutionContext<TestObjectA>(testObject);
            NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(context);
            result.Status.ShouldEqual(NodeResultStatus.Succeeded);

            Assert.AreEqual("Bar", context.State.Foo);
        } 
    }
}