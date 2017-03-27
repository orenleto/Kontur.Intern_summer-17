using System.Collections.Generic;
using System.Linq;

namespace Kontur.GameStats.Domain
{
    public static class Utils
    {
        public static IDictionary<string, int> Increment(this IDictionary<string, int> dict, string key)
        {
            IEnumerable<KeyValuePair<string, int>> temp;
            if (dict.ContainsKey(key))
            {
                temp = dict.Where(pair => pair.Key == key).Select(pair => new KeyValuePair<string, int>(pair.Key, pair.Value + 1));
            }
            else
            {
                temp = new[] {new KeyValuePair<string, int>(key, 1)};
            }
            return new Dictionary<string, int>(dict.Where(pair => pair.Key != key)
                .Concat(temp)
                .ToDictionary(pair => pair.Key, pair => pair.Value));
        }
    }
}