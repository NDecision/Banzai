using System.Collections.Generic;
using Banzai.Factories;

namespace Banzai.JavaScript.Factories
{
    public class JavaScriptMetaDataBuilder : IMetaDataBuilder
    {
        public class MetaDataKeys
        {
            public const string ShouldExecuteScript = "JavaScriptMetaDataBuilder:ShouldExecuteScript";
            public const string ExecutedScript = "JavaScriptMetaDataBuilder:ExecutedScript";
        }

        public void Apply<T>(INode<T> node, IDictionary<string, object> metaData)
        {
            if (metaData == null)
                return;

            var jsNode = node as JavaScriptNode<T>;

            if (jsNode != null)
            {
                object result;
                if (metaData.TryGetValue(MetaDataKeys.ShouldExecuteScript, out result))
                {
                    jsNode.ShouldExecuteScript = (string) result;
                }

                if (metaData.TryGetValue(MetaDataKeys.ExecutedScript, out result))
                {
                    jsNode.ExecutedScript = (string)result;
                }
            }
        }
    }
}