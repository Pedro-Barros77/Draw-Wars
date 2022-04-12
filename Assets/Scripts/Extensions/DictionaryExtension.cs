using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Extensions
{
    internal static class DictionaryExtension
    {
        public static void AddRange<TKey,TValue>(this Dictionary<TKey, TValue> dic, IEnumerable<KeyValuePair< TKey,TValue>> values)
        {
            foreach(var item in values)
            {
                dic.Add(item.Key, item.Value);
            }
        }
    }
}
