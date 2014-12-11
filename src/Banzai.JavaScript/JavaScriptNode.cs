using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ClearScript.Manager;

namespace Banzai.JavaScript
{
    /// <summary>
    /// A node that exposes functions to set to perform node execution.
    /// </summary>
    /// <typeparam name="T">Type of the subject that the node operates on.</typeparam>
    public interface IJavaScriptNode<in T> : INode<T>
    {
        /// <summary>
        /// Method that defines the async function to execute on the subject for this node.
        /// </summary>
        String ScriptId { get; set; }

        /// <summary>
        /// Method that defines the async function to execute on the subject for this node.
        /// </summary>
        String ExecutedScript { get; set; }

        /// <summary>
        /// Method that defines the async function to execute on the subject for this node.
        /// </summary>
        String ShouldExecuteScript { get; set; }

        /// <summary>
        /// Adds up host types that can be passed into the script.
        /// </summary>
        void AddScriptType(string name, Type type);
    }


    /// <summary>
    /// A node that exposes functions to set to perform node execution.
    /// </summary>
    /// <typeparam name="T">Type of the subject that the node operates on.</typeparam>
    public sealed class JavaScriptNode<T> : Node<T>, IJavaScriptNode<T>
    {
        private readonly IList<HostType> _hostTypes = new List<HostType>();
        private readonly IList<HostObject> _hostObjects = new List<HostObject>();

        public JavaScriptNode()
        {
            ManagerPoolInitializer.Initialize();  
        }

        public string ExecutedScript { get; set; }

        public string ScriptId { get; set; }

        public string ShouldExecuteScript { get; set; }

        public void AddScriptType(string name, Type type)
        {
            _hostTypes.Add(new HostType {Name = name, Type = type});
        }

        public void AddScriptObject(string name, object obj)
        {
            _hostObjects.Add(new HostObject { Name = name, Target = obj });
        }

        public override async Task<bool> ShouldExecuteAsync(IExecutionContext<T> context)
        {
            if (string.IsNullOrEmpty(ShouldExecuteScript)) return true;
            
            var result = new ShouldExecuteResult { ShouldExecute = true };

            using (var scope = new ManagerScope())
            {
                SetScriptIdIfEmpty();
                var hostObjects = SetupHostObjects(context, result);
                var hostTypes = SetupHostTypes();

                await scope.RuntimeManager.ExecuteAsync(ScriptId, ShouldExecuteScript, hostObjects, hostTypes);

                return result.ShouldExecute;
            }
        }

        public override bool ShouldExecute(IExecutionContext<T> context)
        {
            return ShouldExecuteAsync(context).Result;
        }

        protected async override Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<T> context)
        {
            if (!string.IsNullOrEmpty(ExecutedScript))
            {
                LogWriter.Debug("ExecutedScript exists, running this script.");
                var result = new NodeResult{IsSuccess = true};

                using (var scope = new ManagerScope())
                {
                    SetScriptIdIfEmpty();
                    var hostObjects = SetupHostObjects(context, result);
                    var hostTypes = SetupHostTypes();

                    await scope.RuntimeManager.ExecuteAsync(ScriptId, ExecutedScript, hostObjects, hostTypes);

                    if (!result.IsSuccess)
                    {
                        return NodeResultStatus.Failed;
                    }
                }
            }
            LogWriter.Debug("ExecutedScript doesn't exist.");

            return NodeResultStatus.Succeeded;
        }

        protected override NodeResultStatus PerformExecute(IExecutionContext<T> context)
        {
            return PerformExecuteAsync(context).Result;
        }

        private List<HostType> SetupHostTypes()
        {
            var contextType = typeof(T);
            var hostTypes = new List<HostType>
            {
                new HostType { Name = contextType.FullName, Type = contextType },
            };
            if (_hostTypes != null)
            {
                hostTypes.AddRange(_hostTypes);
            }
            return hostTypes;
        }

        private List<HostObject> SetupHostObjects(IExecutionContext<T> context, object result)
        {
            var hostObjects = new List<HostObject>
            {
                new HostObject {Name = "context", Target = context},
                new HostObject {Name = "result", Target = result}
            };
            if (_hostObjects != null)
            {
                hostObjects.AddRange(_hostObjects);
            }
            return hostObjects;
        }

        private void SetScriptIdIfEmpty()
        {
            if (string.IsNullOrEmpty(ScriptId))
            {
                ScriptId = ExecutedScript.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}