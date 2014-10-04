![Banzai Pipeline Image](http://upload.wikimedia.org/wikipedia/commons/0/03/Empty_wave_at_Banzai_Pipeline.jpeg)

##Banzai!! - Your Simple Pipeline Solution

Banzai is an easy .Net pipeline solution that contains composable nodes for construcing simple and complex pipelines.  
Yes, there is TPL Dataflow and it's really cool, but I was looking for something easy that solved the 80% case of simple
asynchronous pipelines in my business applications.

##Basic Construction 
Flows are composed from nodes, of which there are a few types.  All flows accept a Subject Type (T).  
This the type of the subject that is acted upon by the workflow.  All methods that are either overridden 
or provided via a function accept an ExecutionContext.  All node executions return a NodeResult.

###Basic Nodes
These are the nodes that actually contain functions that run against the subject of the pipeline.

  * <b>Node/INode</b> - The simplest node type.  This is overridden to provide functionality or via the function properties.  
  * <b>NodeSync/INodeSync</b> - Inherits from Node/INode and provides convenience methods for synchronous methods.

####Node Usage
  * Override PerformExecute/PerformExecuteAsync to perform operations on the subject.
  * Override ShouldExecute to determine if the node should execute.
  * PerformExecuteFunc/PerformExecuteFuncAsync - Function property that accepts a function to perform on the subject.
  * ShouldExecuteFunc/ShouldExecuteFuncAsync - Function property that accepts a function to determine if the node should be executed.

Example of overriding to provide functionality

    public class SimpleTestNodeA1 : Node<TestObjectA>
    {
        private readonly bool _shouldExecute = true;

        public SimpleTestNodeA1(bool shouldExecute)
        {
            _shouldExecute = shouldExecute;
        }

        public override Task<bool> ShouldExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            return Task.FromResult(_shouldExecute);
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            context.Subject.TestValueString = "Completed";
            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

Example of providing functionality via functions

    var node = new Node<TestObjectA>();

    node.ShouldExecuteFuncAsync = context => Task.FromResult(context.Subject.TestValueInt == 5);
    node.ExecutedFuncAsync = context => 
        { context.Subject.TestValueString = "Completed"; return Task.FromResult(NodeResultStatus.Succeeded); };

###Grouping Nodes
The following nodes allow 

* <b>PipelineNode/IPipelineNode</b> - Runs a group of nodes serially on the subject.  This will be the root node of most flows.
* <b>GroupNode/IGroupNode</b> - An aggregation of nodes that are run on a subject using the asyncrhonous Task.WhenAll pattern.
* <b>FirstMatchNode/IFirstMatchNode</b> - An aggregation of nodes of which the first matching it's ShouldExecute condition is run on the subject.

###ExecutionContext
The execution context flows through all nodes in the flow.  The execution context contains options for running the flow as well as the
instance of the subject that the flow is executed on.  

####Subject
This is the main subject instance that all nodes in the flow operate on.  If it is necessary to change the subject reference, use the ChangeSubject() method of the 
ExecutionContext to do so.

####State
The execution context also contains a dynamic State property that can be used to 
flow any random state needed for the workflow.  Any node in the flow can update the state or add dynamic properties to the state.

###NodeResult
When a node executes, it returns a NodeResult.  The NodeResult will contain:
  * The NodeResultStatus - Which represents the status of this node.  If this is a parent node, it represents a rollup status of all child nodes.
  * A reference to the subject.
  * A collection of child result nodes corresponding to the current node's children.
  * An Exception if an exception occurred during execution of the node.

####NodeResultStatus
Each node returns a result that contains a status when run.  The status will be one of the following:

  * NotRun - Node has not been run
  * SucceededWithErrors - Node is flagged as succeeded, but some error occurred.  Typically indicates that a subnode failed but "ContinueOnError" was set to true.
  * Succeeded - Node Succeed
  * Failed - Node reported a failure or an exception was thrown during the execution of the node.

###NodeRunStatus

##Registering Nodes

##Building Flows
There are multiple ways in which flows can be built up.

###Manually Constructing Flows

###Injecting Child Nodes

###Injecting INodeFactory

###Using FlowBuilder
FlowBuilder allows you to build complex workflows with a simple fluent interface.  Complex flows can be constructed by 
adding both nodes and subflows.

##Advanced Scenarios

###Changing the Subject
In some cases, you may want to switch the subject reference during part of the flow.  One example is that you call an external
service within a node and it returns a different subject reference back to you.  In this case, you want to nodes following the 
current node to recieve the new subject.  This can be accomplished by calling the ExecutionContext.ChangeSubject() method.

####More documentation to follow shortly....


