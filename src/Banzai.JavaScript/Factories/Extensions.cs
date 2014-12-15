using Banzai.Factories;

namespace Banzai.JavaScript.Factories
{
    /// <summary>
    /// Extensions to help setting up javascript nodes with the component builder.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Sets the ShouldExecute JavaScript.
        /// </summary>
        /// <typeparam name="T">Subject type of the component builder.</typeparam>
        /// <param name="builder">The FlowComponentBuilder.</param>
        /// <param name="shouldExecuteScript">The ShouldExecute script to run.</param>
        /// <returns>The flow component builder with JavaScript set.</returns>
        public static IFlowComponentBuilder<T> SetShouldExecuteJavaScript<T>(this IFlowComponentBuilder<T> builder, string shouldExecuteScript)
        {
            return builder.SetMetaData(JavaScriptMetaDataBuilder.MetaDataKeys.ShouldExecuteScript, shouldExecuteScript);
        }

        /// <summary>
        /// Sets the Excuted JavaScript.
        /// </summary>
        /// <typeparam name="T">Subject type of the component builder.</typeparam>
        /// <param name="builder">The FlowComponentBuilder.</param>
        /// <param name="executedScript">The script to run.</param>
        /// <returns>The flow component builder with JavaScript set.</returns>
        public static IFlowComponentBuilder<T> SetExecutedJavaScript<T>(this IFlowComponentBuilder<T> builder, string executedScript)
        {
            return builder.SetMetaData(JavaScriptMetaDataBuilder.MetaDataKeys.ExecutedScript, executedScript);
        }

        /// <summary>
        /// Gets the components ShouldExecute JavaScript.
        /// </summary>
        /// <typeparam name="T">Subject type of the component builder.</typeparam>
        /// <param name="component">Component to query for the ShouldExecute JavaScript.</param>
        /// <returns>The requested script.</returns>
        public static string GetShouldExecuteJavaScript<T>(this FlowComponent<T> component)
        {
            if (component.MetaData.ContainsKey(JavaScriptMetaDataBuilder.MetaDataKeys.ShouldExecuteScript))
                return (string) component.MetaData[JavaScriptMetaDataBuilder.MetaDataKeys.ShouldExecuteScript];

            return null;
        }

        /// <summary>
        /// Gets the components Executed JavaScript.
        /// </summary>
        /// <typeparam name="T">Subject type of the component builder.</typeparam>
        /// <param name="component">Component to query for the Executed JavaScript.</param>
        /// <returns>The requested script.</returns>
        public static string GetExecutedJavaScript<T>(this FlowComponent<T> component)
        {
            if (component.MetaData.ContainsKey(JavaScriptMetaDataBuilder.MetaDataKeys.ExecutedScript))
                return (string)component.MetaData[JavaScriptMetaDataBuilder.MetaDataKeys.ExecutedScript];

            return null;
        }
    }
}