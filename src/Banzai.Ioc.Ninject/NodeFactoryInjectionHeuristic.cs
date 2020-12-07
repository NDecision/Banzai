using System.Reflection;
using Banzai.Factories;
using Banzai.Ioc.Ninject.Utility;
using Ninject.Components;
using Ninject.Selection.Heuristics;

namespace Banzai.Ioc.Ninject
{
    public class NodeFactoryInjectionHeuristic : NinjectComponent, IInjectionHeuristic
    {
        public bool ShouldInject(MemberInfo memberInfo)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            return ShouldInject(propertyInfo);
        }

        private bool ShouldInject(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                return false;

            if (!propertyInfo.CanWrite)
                return false;

            var propertyType = propertyInfo.PropertyType;

            return propertyInfo.Name.Equals("NodeFactory") && propertyType.InheritsOrImplements(typeof(INodeFactory<>));
        }
    }
}