using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static CSharpLibraries.Interpreters.LISharP;

namespace CSharpLibraries.Interpreters
{
    internal sealed class SchemeList : IEnumerable<object>
        {
            internal object Car { get; set; }

            internal object Cdr { get; set; }

            public int Count
            {
                get
                {
                    int r = 0;
                    if (Cdr.Equals(Nil))
                    {
                        r++;
                    }
                    else
                    {
                        r += ((SchemeList)Cdr).Count + 1;
                    }
            
                    return r;
                }
            }
            
            public SchemeList(object o)
            {
                Car = o;
                Cdr = Nil;
            }

            public SchemeList(object car, object cdr)
            {
                Car = car;
                Cdr = cdr;
            }

            /// <summary>
            /// append element of list
            /// </summary>
            /// <param name="o">object</param>
            /// <returns>cdr</returns>
            public SchemeList ChainAdd(object o)
            {
                if (!(o is SchemeList))
                {
                    Cdr = new SchemeList(o);
                    return (SchemeList)Cdr;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            public void Append(object other)
            {
                if (other is SchemeList)
                {
                    if (Cdr.Equals(Nil))
                    {
                        Cdr = other;
                    }
                    else
                    {
                        ((SchemeList)Cdr).Append(other);
                    }
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            public IEnumerator<object> GetEnumerator()
            {
                object t = this;
                while (!t.Equals(Nil))
                {
                    var r = ((SchemeList)t).Car;
                    t = ((SchemeList)t).Cdr;
                    yield return r;
                }
            }

            public override string ToString()
            {
                var b = new StringBuilder("[");
                foreach (var e in this)
                {
                    b.Append(e);
                    b.Append(", ");
                }

                b.Remove(b.Length - 2, 2);
                b.Append(']');
                return b.ToString();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
}