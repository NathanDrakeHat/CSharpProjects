using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CSharpLibraries.Interpreters.Lisp;
using static CSharpLibraries.Interpreters.NumericOperators;
using static CSharpLibraries.Interpreters.InterpretersExceptions;

namespace CSharpLibraries.Interpreters
{
    public sealed class Env : Dictionary<object, object>
    {
        
        internal delegate object Lambda(List<object> args);
        
        private readonly Env _outer;

        public Env(IEnumerable<object> parameters, IEnumerable<object> args, Env outer = null)
        {
            using var paramEnum = parameters.GetEnumerator();
            using var argsEnum = args.GetEnumerator();
            while (paramEnum.MoveNext() && argsEnum.MoveNext())
            {
                this[paramEnum.Current!] = argsEnum.Current;
            }

            _outer = outer;
        }

        public Env Find(object variable)
        {
            return ContainsKey(variable) ? this : _outer.Find(variable);
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            foreach (KeyValuePair<object,object> keyValuePair in this)
            {
                s.Append($"[{keyValuePair.Key}:{keyValuePair.Value}]\n");
            }

            s.Remove(s.Length - 1, 1);
            return s.ToString();
        }
        
        internal static Env StandardEnv()
        {
            var d = new Dictionary<object, object>()
            {
                {
                    "+", new Lambda(args =>
                    {
                        if (args.Count < 1) throw new ArgumentsCountException(">=2");
                        object first = args[0];
                        if (args.Count == 1 && LessThan(first, 0))
                        {
                            return Negative(first);
                        }
                        else
                        {
                            return args.Aggregate((o1, o2) =>
                            {
                                object a = o1;
                                object b = o2;
                                return Plus(a,b);
                            });

                        }
                    })
                },
                {
                    "-", new Lambda(args =>
                    {
                        object first = args[0];
                        switch (args.Count)
                        {
                            case 1:
                                if (LessThan(0, first))
                                {
                                    first = Negative(first);
                                }

                                return first;
                            case 2:
                                object second = args[1];
                                return Minus(first,second);
                            default:
                                throw new ArgumentsCountException("1 or 2");
                        }
                    })
                },
                {
                    "*", new Lambda(args =>
                    {
                        if (args.Count < 2) throw new ArgumentsCountException(">= 2");
                        return args.Aggregate(Multiply);
                    })
                },
                {
                    "/", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentsCountException("2");
                        object a = args[0];
                        object b = args[1];
                        return Divide(a,b);
                    })
                },
                {
                    ">", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentsCountException("2");
                        object a = args[0];
                        object b = args[1];
                        return LessThan(b,a);
                    })
                },
                {
                    "<", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentsCountException("2");
                        object a = args[0];
                        object b = args[1];
                        return LessThan(a,b);
                    })
                },
                {
                    ">=", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentsCountException("2");

                        object a = args[0];
                        object b = args[1];
                        return LessThan(b,a);
                    })
                },
                {
                    "<=", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentsCountException("2");

                        object a = args[0];
                        object b = args[1];
                        return LessThan(a,b);
                    })
                },
                {
                    "=", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentsCountException("2");
                        object a = args[0];
                        object b = args[1];
                        return Equal(a,b);
                    })
                },
                {
                    "abs", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        object res = args[0];
                        if (LessThan(res,0)) return Negative(res);
                        else return res;
                    })
                },
                {
                    "append", new Lambda(args =>
                    {
                        if (args.Count < 2) throw new ArgumentsCountException(">=2");
                        for (int i = 0; i < args.Count - 1; i++)
                        {
                            SchemeList list = (SchemeList)args[i];
                            var list1 = args[i + 1];
                            list.Append(list1);
                        }

                        return args[0];
                    })
                },
                {
                    "apply", new Lambda(args =>
                    {
                        Lambda proc = (Lambda)args[0];
                        return proc(args.GetRange(1, args.Count - 1));
                    })
                },
                {"begin", new Lambda(args => args[^1])},
                {
                    "car", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        return ((SchemeList) args[0]).Car;
                    })
                },
                {
                    "cdr", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        return ((SchemeList) args[0]).Cdr;
                    })
                },
                {
                    "cons", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentsCountException("2");
                        if (args[0] is SchemeList s)
                        {
                            s.Cdr = args[1];
                            return s;
                        }
                        else
                        {
                            return new SchemeList(args[0], args[1]);
                        }
                    })
                },
                {
                    "eq?", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentsCountException("2");
                        return ReferenceEquals(args[0],args[1]);
                    })
                },
                {
                    "expt", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentsCountException("2");
                        var a = Value(args[0]);
                        var b = Value(args[1]);
                        return Math.Pow(a, b);
                    })
                },
                {
                    "equal?", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentsCountException("2");
                        return args[0].Equals(args[1]);
                    })
                },
                {
                    "length", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        SchemeList list = (SchemeList)args[0];
                        return list.Count;
                    })
                },
                {
                    "list", new Lambda(args =>
                    {
                        if (args.Count < 1)
                        {
                            return new SchemeList(Nil);
                        }
                        var res = new SchemeList(args[0]);
                        var p = res;
                        for (int i = 1; i < args.Count; i++)
                        {
                            p = p.ChainAdd(args[i]);
                        }

                        return res;
                    })
                },
                {
                    "list?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        return args[0] is SchemeList;
                    })
                },
                {
                    "map", new Lambda(args =>
                    {
                        if (args.Count < 2) throw new ArgumentsCountException(">=2");
                        Lambda func = (Lambda)args[0];
                        var lists = args.GetRange(1, args.Count-1);
                        var elements0 = new List<object>();
                        for (int i = 0; i < lists.Count; i++)
                        {
                            elements0.Add(((SchemeList)lists[i]).Car);
                            lists[i] = ((SchemeList)lists[i]).Cdr;
                        }

                        var r = new SchemeList(func(elements0));
                        var p = r;
                        while (true)
                        {
                            var elements = new List<object>();
                            for (int i = 0; i < lists.Count; i++)
                            {
                                if (!lists[i].Equals("'()"))
                                {
                                    elements.Add(((SchemeList)lists[i]).Car);
                                    lists[i] = ((SchemeList)lists[i]).Cdr;
                                }
                            }

                            if (elements.Count == lists.Count)
                            {
                                p = p.ChainAdd(func(elements));
                            }
                            else
                            {
                                break;
                            }
                        }

                        return r;
                    })
                },
                {"max", new Lambda(args => args.Max())},
                {"min", new Lambda(args => args.Min())},
                {
                    "not", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        return !(bool) args[0];
                    })
                },
                {
                    "null?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        return args[0].Equals(Nil);
                    })
                },
                {
                    "number?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        object type = args[0].GetType();
                        return type.Equals(typeof(int)) || type.Equals(typeof(double));
                    })
                },
                {
                    "print", new Lambda(args =>
                    {
                        StringBuilder b = new StringBuilder();
                        for (int i = 0; i < args.Count - 1; i++)
                        {
                            b.Append(args[i]);
                            b.Append(' ');
                        }

                        b.Append(args[^1]);
                        Console.WriteLine(b.ToString());
                        return null;
                    })
                },
                {
                    "procedure?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        var type = args[0].GetType();
                        return type == typeof(Lambda);
                    })
                },
                {
                    "round", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        var a = Value(args[0]);
                        return Math.Round(a, MidpointRounding.AwayFromZero);
                    })
                },
                {
                    "symbol?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        return args[0] is string;
                    })
                },
                {"pi", Math.PI},
                {
                    "quote",
                    new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentsCountException("1");
                        return args[0];
                    })
                },
                {
                    "nil",
                    Nil
                }
            };
            var env = new Env(d.Keys, d.Values);
            return env;
        }
    }
}