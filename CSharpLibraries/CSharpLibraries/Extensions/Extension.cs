using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace CSharpLibraries.Extensions
{
    public static class Extension
    {
        public readonly struct MatrixIndex
        {
            public readonly int Row;
            public readonly int Col;

            public MatrixIndex(int r, int c)
            {
                Row = r;
                Col = c;
            }

            public void Deconstruct(out int r, out int c)
            {
                r = Row;
                c = Col;
            }
        }
        
        /// <summary>
        /// dynamic nested list(tree) to string<br/>
        /// nested list should have same element type which should be the generic type<br/>
        /// </summary>
        /// <param name="nl">nested list</param>
        /// <param name="start">start string builder</param>
        /// <typeparam name="T">element to print</typeparam>
        /// <returns>nested list with '[' and ']' split</returns>
        public static string NestedToString<T>(this IEnumerable nl, StringBuilder? start = null)
        {
            StringBuilder res = new StringBuilder("[");
            var enumerator = nl.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (current != null && current.GetType() == typeof(T))
                { 
                    res.Append(current);
                }
                else if (current is IEnumerable t)
                {
                    res.Append(NestedToString<T>(t, res));
                }
                else
                { 
                    res.Append(current);
                }
            }
            while(enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (current != null && current.GetType() == typeof(T))
                { 
                    res.Append(", ");
                    res.Append(current);
                }
                else if (current is IEnumerable t)
                {
                    res.Append(", ");
                    res.Append(NestedToString<T>(t, res));
                }
                else
                {
                    res.Append(", ");
                    res.Append(current);
                }
            }
            res.Append(']');
            return res.ToString();
        }

        public static string MatrixToString<T>(IEnumerable<IEnumerable<T>> m)
        {
            StringBuilder res = new StringBuilder();
            foreach (var r in m)
            {
                foreach (var e in r)
                {
                    res.Append(e);
                    res.Append(' ');
                }

                res.Append('\n');
            }

            return res.ToString();
        }
        
        /// <summary>
        /// Fisher-Yates shuffle:
        /// </summary>
        /// <param name="list">list</param>
        /// <typeparam name="T">any</typeparam>
        public static void Shuffle<T>(this IList<T> list)  
        {  
            Random rng = new Random();
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }

        /// <summary>
        /// get almost exactly type name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetReadableTypeName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }
            else
            {
                var s = new StringBuilder();
                s.Append(type.Name.Take(type.Name.IndexOf('`')).ToArray());
                s.Append('<');
                var genericTypes = type.GetGenericArguments();
                foreach (var t in genericTypes)
                {
                    s.Append(GetReadableTypeName(t));
                    s.Append(", ");
                }
                s.Remove(s.Length - 2, 2);
                s.Append('>');
                return s.ToString();
            }
        }

        public static void NotNullArg(object o, string name)
        {
            if (o == null) throw new ArgumentNullException(name);
        }

        public static IList<int> ShuffledArithmeticSequence(int low, int high, int d)
        {
            var l = new List<int>();
            do
            {
                l.Add(low);
                low += d;
            } while (low < high);

            l.Shuffle();
            return l;
        }
    }
}