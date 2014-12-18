using System.Collections;

namespace Banzai.Utility
{
    internal static class ListExtensions
    {
        public static bool SafeAny(this IList list)
        {
            return list != null && list.Count > 0;
        }
    }
}