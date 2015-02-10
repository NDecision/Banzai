namespace Banzai.Factories
{
    /// <summary>
    /// Extensions to help setting up javascript nodes with the component builder.
    /// </summary>
    public static class CustomDataExtensions
    {
        /// <summary>
        /// Sets the ShouldExecute JavaScript.
        /// </summary>
        /// <typeparam name="T">Subject type of the component builder.</typeparam>
        /// <param name="builder">The FlowComponentBuilder.</param>
        /// <param name="customData">Custom Data to set on the node.</param>
        /// <returns>The flow component builder with Custom Data set.</returns>
        public static IFlowComponentBuilder<T> SetCustomData<T>(this IFlowComponentBuilder<T> builder, object customData)
        {
            return builder.SetMetaData(CustomDataMetaDataBuilder.MetaDataKeys.CustomData, customData);
        }


        /// <summary>
        /// Gets the components Custom Data.
        /// </summary>
        /// <typeparam name="T">Subject type of the component builder.</typeparam>
        /// <param name="component">Component to query for the Custom Data.</param>
        /// <returns>The requested custom data.</returns>
        public static dynamic GetCustomData<T>(this FlowComponent<T> component)
        {
            if (component.MetaData.ContainsKey(CustomDataMetaDataBuilder.MetaDataKeys.CustomData))
                return component.MetaData[CustomDataMetaDataBuilder.MetaDataKeys.CustomData];

            return null;
        }

    }
}