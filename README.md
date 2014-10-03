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

* Node/NodeSync/INode - The simplest node type.  This is overridden to provide functionality.  NodeSync provides a convenience wrapper for synchronous methods.

  * Provide an implementation of PerformExecute/PerformExecuteAsync to perform operations on the subject.

  * Override ShouldExecute to determine if the node should execute.

* FuncNode/IFuncNode - Node implementation that accepts Task delegates for ShouldExecute and Execute methods.

  * Provide a function to ExecuteFunc and/or ShouldExecuteFunc instead of overriding the function provided by Node.

* FuncNodeSync/IFuncNodeSync - Node implementation that accepts synchronous delegates for ShouldExecute and Execute methods.

###Grouping Nodes
The following nodes allow 

* PipelineNode/IPipelineNode - Runs a group of nodes serially on the subject.  This will be the root node of most flows.

* GroupNode/IGroupNode - An aggregation of nodes that are run on a subject using the asyncrhonous Task.WhenAll pattern.

* FirstMatchNode/IFirstMatchNode - An aggregation of nodes of which the first matching it's ShouldExecute condition is run on the subject.

###ExecutionContext

###NodeResult

####NodeResultStatus

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


