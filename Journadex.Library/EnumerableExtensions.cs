using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journadex.Library
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// https://stackoverflow.com/questions/11830174/how-to-flatten-tree-via-linq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f) => e.SelectMany(c => f(c).Flatten(f)).Concat(e);
    }
}
