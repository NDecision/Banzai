##Banzai!! - Your Simple Pipeline Solution

Banzai is an easy .Net pipeline solution that contains composable nodes for construcing simple and complex pipelines.  
Yes, there is TPL Dataflow and it's really cool, but I was looking for something easy that solved the 80% case of simple
asynchronous pipelines in my business applications.

###Basic Construction 
Flows are composed from nodes, of which there are a few types.

* Node/INode - The simplest node type.  This is overridden to provide functionality.

* GroupNode/IGroupNode - An aggregation of nodes that are run on a subject using the asyncrhonous Task.WhenAll pattern.

* FirstMatchNode/IFirstMatchNode - An aggregation of nodes of which the first matching it's ShouldExecute condition is run on the subject.

* PipelineNode/IPipelineNode - Runs a group of nodes serially on the subject.


