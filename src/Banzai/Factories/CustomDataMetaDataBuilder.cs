using System.Collections.Generic;

namespace Banzai.Factories
{
    public class CustomDataMetaDataBuilder : IMetaDataBuilder
    {
        public class MetaDataKeys
        {
            public const string CustomData = "CoreMetaDataBuilder:CustomData";
        }

        public void Apply<T>(INode<T> node, IDictionary<string, object> metaData)
        {
            if (metaData == null || node == null)
                return;

            if (metaData.TryGetValue(MetaDataKeys.CustomData, out var result))
            {
                node.CustomData = result;
            }
        }
    }
}