using System;
using System.Reflection;
using Banzai.Factories;
using Banzai.Ninject.Utility;
using Ninject.Components;
using Ninject.Selection.Heuristics;

namespace Banzai.Ninject
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

            Type propertyType = propertyInfo.PropertyType;

            return propertyInfo.Name.Equals("NodeFactory") && propertyType.InheritsOrImplements(typeof (INodeFactory<>));
        }
    }
}