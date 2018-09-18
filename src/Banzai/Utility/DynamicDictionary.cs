using System.Collections.Concurrent;
using System.Dynamic;

namespace Banzai.Utility
{
    /// <summary>
    /// Dynamic dictionary for storing state.
    /// </summary>
    internal class DynamicDictionary : DynamicObject
    {
        private readonly ConcurrentDictionary<string, object> _dictionary = new ConcurrentDictionary<string, object>();

        public int Count => _dictionary.Count;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            _dictionary.TryGetValue(binder.Name, out result);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dictionary.AddOrUpdate(binder.Name, value, (s, o) => value);
            return true;
        }
    }
}