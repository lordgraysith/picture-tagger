using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureTagger
{
    public static class ExtensionMethods
    {
        public static T Before<T>(this IEnumerable<T> source, T item)
        {
            if (source.Count() <= 0 || !source.Contains(item))
            {
                return default(T);
            }
            T previous = default(T);
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (object.Equals(enumerator.Current, item))
                    {
                        break;
                    }
                    previous = enumerator.Current;
                }
            }
            if (object.Equals(previous, default(T)))
            {
                previous = source.Last();
            }
            return previous;
        }
        public static T After<T>(this IEnumerable<T> source, T item)
        {
            if (source.Count() <= 0 || !source.Contains(item))
            {
                return default(T);
            }
            T previous = default(T);
            T result = default(T);
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (object.Equals(item, previous))
                    {
                        result = enumerator.Current;
                        break;
                    }
                    previous = enumerator.Current;
                }
            }
            if (object.Equals(result, default(T)))
            {
                result = source.First();
            }
            return result;
        }
    }
}
