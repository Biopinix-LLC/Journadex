using System;
using System.Collections.Generic;
using System.Linq;

namespace Journadex.Library
{
    internal static class DictionaryExtensions
    {

        /// <summary>
        /// ChatGPT: Returns the specified key if it doesn't already exist; if it does exist, key is returned with a underscore and count appended to it so that it follows the format "key" "key_2" "key_3" 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetNextAvailableKey<T>(this Dictionary<string, T> dictionary, string key)
        {
            {
                int counter = 2;
                string modifiedKey = key;
                while (dictionary.ContainsKey(modifiedKey))
                {
                    modifiedKey = key + "_" + counter;
                    counter++;
                }
                return modifiedKey;
            }
        }

        /// <summary>
        /// ChatGPT: Write a C# extension method for a Dictionary<string, List<Range>> that gets the index in the list of the range that contains a specified integer
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetRangeIndexContaining(this Dictionary<string, List<Range>> dict, int value)
        {
            foreach (var kvp in dict)
            {
                var rangeList = dict[kvp.Key];
                for (int i = 0; i < rangeList.Count; i++)
                {
                    if (rangeList[i].Start <= value && value <= rangeList[i].End)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }




        /// <summary>
        /// ChatGPT: Write a C# extension method for a dictionary that gets a unique key by appending a lowercase letter to it starting with 'b' and continuing to 'z' as long as there continues to be duplicate keys. After `z`, it would start at `aa` followed by `bb` until `zz` after which it would start at `aaa` until `zzz` etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetUniqueKey<T>(this Dictionary<string, T> dictionary, string key)
        {
            var uniqueKey = key;
            var counter = 'b';
            while (dictionary.ContainsKey(uniqueKey))
            {
                uniqueKey = key + counter++;
                if (counter > 'z')
                {
                    counter = 'a';
                    key += 'a';
                }
            }
            return uniqueKey;
        }

        public static string AddWithUniqueKey<T>(this Dictionary<string, T> dictionary, string key, T value)
        {
            string uniqueKey = dictionary.GetUniqueKey(key);
            dictionary.Add(uniqueKey, value);
            return uniqueKey;
        }

        public static Dictionary<string, string> ToCaseInsensitiveDictionary(this Dictionary<string, string> originalDictionary, StringComparer comparer = null)
            => new Dictionary<string, string>(originalDictionary, comparer ?? StringComparer.OrdinalIgnoreCase);
    }
}