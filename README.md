![Banzai Pipeline Image](http://upload.wikimedia.org/wikipedia/commons/0/03/Empty_wave_at_Banzai_Pipeline.jpeg)

##Banzai!! - Your Simple Pipeline Solution

Banzai is an easy .Net pipeline solution that contains composable nodes for constructing simple and complex pipelines.  
Yes, there is TPL Dataflow and it's really cool, but I was looking for something easy that solved the 80% case of simple
asynchronous pipelines in my business applications.

##Basic Construction 
Flows are composed from nodes, of which there are a few types.  All flows accept a Subject Type (T).  
This the type of the subject that is acted upon by the workflow.  All methods that are either overridden 
or provided via a function accept an ExecutionContext.  All node executions return a [NodeResult](#noderesult).

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
        public override Task<bool> ShouldExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            return Task.FromResult(true);
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
        { 
          context.Subject.TestValueString = "Completed"; 
          return Task.FromResult(NodeResultStatus.Succeeded); 
        };

###Grouping Nodes
The following nodes allow you to organize and run other nodes together.

<b>PipelineNode/IPipelineNode</b> - Runs a group of nodes serially on the subject.  This will be the root node of most flows.

    var pipelineNode = new PipelineNode<TestObjectA>();

    pipelineNode.AddChild(new SimpleTestNodeA1());
    pipelineNode.AddChild(new SimpleTestNodeA2());

    var testObject = new TestObjectA();
    NodeResult<TestObjectA> result = await pipelineNode.ExecuteAsync(testObject);

<b>GroupNode/IGroupNode</b> - An aggregation of nodes that are run on a subject using the asyncrhonous Task.WhenAll pattern.

    var groupNode = new GroupNode<TestObjectA>();

    groupNode.AddChild(new SimpleTestNodeA1());
    groupNode.AddChild(new SimpleTestNodeA2());

    var testObject = new TestObjectA();
    NodeResult<TestObjectA> result =  await groupNode.ExecuteAsync(testObject);

<b>FirstMatchNode/IFirstMatchNode</b> - An aggregation of nodes of which the first matching its ShouldExecute condition is run on the subject.

    var matchNode = new FirstMatchNode<TestObjectA>();

    var firstNode = new SimpleTestNodeA1();
    firstNode.ShouldExecuteFuncAsync = 
        context => Task.FromResult(context.Subject.TestValueInt == 0);
    matchNode.AddChild(firstNode);

    var secondNode = new SimpleTestNodeA2();
    secondNode.ShouldExecuteFuncAsync = 
        context => Task.FromResult(context.Subject.TestValueInt == 1);
    matchNode.AddChild(secondNode);

    var testObject = new TestObjectA();
    NodeResult<TestObjectA> result = await matchNode.ExecuteAsync(testObject);

###ExecutionContext
The execution context flows through all nodes in the flow.  The execution context contains options for running the flow as well as the
instance of the subject that the flow is executed on.  

<b>Subject</b> - This is the main subject instance that all nodes in the flow operate on.  If it is necessary to change the subject reference, use the ChangeSubject() method of the 
ExecutionContext to do so.

<b>State</b> - The execution context also contains a dynamic State property that can be used to 
flow any random state needed for the workflow.  Any node in the flow can update the state or add dynamic properties to the state.

    var context = new ExecutionContext<object>(new object());
    context.State.Foo = "Bar";

<b>GlobalOptions</b> - ExecutionOptions that are set in the context and applied to every node.

<b>EffectiveOptions</b> - Overrides the GlobalOptions with any ExecutionOptions that are set on the specific node being executed.  
These are the options evaluated when executing a node.

<b>ParentResult</b> - The root result of this node execution and all of its children.

####ExecutionOptions
The execution options impact the behavior of node execution. 
 
<b>ContinueOnFailure</b> - Continue execution of other nodes under the parent if this node fails. Defaults to false.

<b>ThrowOnException</b> - Should a node execution exception throw or register as a failure and store the exception in the NodeResults exception property.
Defaults to false.

###NodeResult
When a node executes, it returns a NodeResult.  The NodeResult will contain:

<b>NodeResultStatus</b> - Represents the status of this node.  If this is a parent node, it represents a rollup status of all child nodes.

<b>Subject</b> - A reference to the subject.

<b>ChildResults</b> - A collection of child result nodes corresponding to the current node's children.

<b>Exception</b> - An Exception if any exception occurred during execution of the node.

<b>GetFailExceptions()</b> - This method aggregates all the exceptions on the failure path of the current node and returns them as an IEnumerable&lt;Exception&gt;.  
This includes any exception from nodes that contributed to a failure status of the current.  It's important to know that if a PipelineNode is set to ContinueOnFailure 
and some of the nodes succeed, the PipelineNode will have a status of PartiallySucceeded and as such will not have failures added to this collection.

####NodeResultStatus
Each node returns a result that contains a status when run.  The status will be one of the following:

  * NotRun - Node has not been run
  * SucceededWithErrors - Node is flagged as succeeded, but some error occurred.  Typically indicates that a subnode failed but "ContinueOnError" was set to true.
  * Succeeded - Node Succeed
  * Failed - Node reported a failure or an exception was thrown during the execution of the node.

###NodeRunStatus
Represents the run status of the node.  The status will be one of the following:
  * NotRun - The node has not been run
  * Running - The node is in process
  * Completed - The node has completed
  * Faulted - The node faulted (threw an exception)

##Registering Nodes
Registering nodes with an IOC container is easy.  Currently, we provide Autofac helpers in the [Banzai.Autofac library](https://www.nuget.org/packages/Banzai.Autofac/).  
These extensions scan the indicated assembly for any custom nodes you have created and register them as themselves and their implemented interfaces.
Nodes are registered as Transient/PerDependency.

Scan the current assembly

    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterBanzaiNodes(GetType().Assembly);

or

    containerBuilder.RegisterBanzaiNodes<TypeFromAssembly>();

All the RegisterBanzaiNode methods have an optional parameter that automatically registers the Banzai nodes and other classes:

    containerBuilder.RegisterBanzaiNodes(GetType().Assembly, **true**);

Or to explicitly register the Banzai nodes and classes using the overload with no arguments:

    containerBuilder.RegisterBanzaiNodes();

##Building Flows
There are multiple ways in which flows can be built up.

###Manually Constructing Flows
Any node can be manually added to the Children collection of any Multinode (Pipeline, Group, FirstMatch) using the AddChild or AddChildren methods.
Multinodes can be added to other multinodes, allowing a node tree to be built.

    var pipelineNode = new PipelineNode<TestObjectA>();

    pipelineNode.AddChild(new SimpleTestNodeA1());
    pipelineNode.AddChild(new SimpleTestNodeA2());

Alternately, if the children are known at design time, the children may simply be added in the parents constructor.  
However, this approach will be problematic if the child nodes have dependencies injected. 

###Injecting Child Nodes
If nodes are registerd with the DI container (See [Registering Nodes](#registering-nodes)), they can be injected into the 
constructor of any node.

    public class Pipeline1 : PipelineNode<TestObjectA>
    {
        private ISimpleTestNode _child1;

        public Pipeline1(ISimpleTestNode child1)
        {
            _child1 = child1;
        }
    }

###Injecting INodeFactory
As an alternative to directly injecting child nodes into a parent node, you can use the INodeFactory instead.  An INodeFactory
is automatically set up when you register core constructs with RegisterBanzaiNodes. Any node that implements IMultiNode 
(Pipeline/Group/FirstMatch or an implementation you provide) will have the INodeFactory automatically injected into the NodeFactory property (as of 1.0.6).  
Yes it's somewhat like using a service locator for nodes ([so so terrible right? o_O](https://www.youtube.com/watch?v=aNUr__-VZeQ)).  
Typically, nodes want to concern themselves with injecting things that provide additional functionality such as external services/repositories etc...
so this declutters the constructor and I see this as a cross-cutting concern.  Additionally, this allows the parent flow to apply logic to adding child flows 
rather than just receiving the same child flows each time via straight injection.

If you've used Banzai.Autofac to register your nodes, INodeFactory will be able to request nodes via any interface implemented or via the class type itself.

    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

    var container = containerBuilder.Build();
    var nodeFactory = container.Resolve<INodeFactory<object>>();

    public class MyComplexNode : IPipeline<object>
    {
        public MyComplexNode(INodeFactory<object> nodeFactory)
        {
            var node = nodeFactory.GetNode<ITestNode<object>>();
        }
    }

or

    public class MyComplexNode : IPipeline<object>
    {
        public override void OnBeforeExecute(ExecutionContext<object> context)
        {
            //Use the built-in nodefactory (not available yet in the constructor)
            var node = NodeFactory.GetNode<ITestNode<object>>();
        }
    }


###Using FlowBuilder
FlowBuilder allows you to build complex workflows with a simple fluent interface.  Complex flows can be constructed by 
adding both nodes and subflows.  Once a flow is registered, it can be accessed from the container or via the INodeFactory.

####Methods
<b>CreateFlow</b> - Initiates flow creation and returns a FlowBuilder for the flow.
<b>AddRoot</b> - Adds a root node to a flow.  Returns a reference to a FlowComponentBuilder for the root node.
<b>AddChild</b> - Adds a child to the current node and returns the same FlowComponentBuilder for the current node (not the child).
<b>AddFlow</b> - Allows the addition of one flow as a child of the current node.  Allows for the creation of common flows that can be added to other flows.
<b>ForChild</b> - Changes the FlowComponentBuilder context to the specified child node.
<b>Register</b> - Must be called to indicate the flow definition as been completed and to register the flow with the container. 

    var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

    flowBuilder.CreateFlow("TestFlow1")
      .AddRoot<IPipelineNode<object>>()
      .AddChild<ITestNode2>()
      .AddChild<IPipelineNode<object>>()
        .ForChild<IPipelineNode<object>>()
        .AddChild<ITestNode4>()
        .AddChild<ITestNode3>()
        .AddChild<ITestNode2>();
    flowBuilder.Register();

    var container = containerBuilder.Build();

Access it via the container:

    var flow = container.ResolveNamed<FlowComponent<object>>("TestFlow1");

Or via the nodefactory:

    var nodeFactory = container.Resolve<INodeFactory<object>>();
    var flow = (IPipelineNode<object>)factory.GetFlow("TestFlow1");


##Logging
By default, Banzai will log to the Debug console in debug mode and will not log in release mode.  The [Banzai.Log4Net package](https://www.nuget.org/packages/Banzai.Log4Net/) will
allow you to log to Log4Net.  Obviously, you must configure Log4Net as you normally would.

Banzai will log a number of Error/Info/Debug operations to the log by default.  From any INode (v1.0.7 or greater), a LogWriter
will be exposed for your own custom logging.

Setting up Log4Net (after installing the package)

    LogWriter.SetFactory(new Log4NetWriterFactory());


##Advanced Scenarios

###Changing the Subject
In some cases, you may want to switch the subject reference during part of the flow.  One example is that you call an external
service within a node and it returns a different subject reference back to you.  In this case, you want to nodes following the 
current node to recieve the new subject.  This can be accomplished by calling the ExecutionContext.ChangeSubject() method.

####More documentation to follow shortly...


