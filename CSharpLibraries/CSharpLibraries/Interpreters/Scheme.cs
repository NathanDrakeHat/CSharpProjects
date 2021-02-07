#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

// TODO extend
[assembly: InternalsVisibleTo("CSarpLibrariesTest")]

namespace CSharpLibraries.Interpreters
{
    public static class Scheme
    {
        private static readonly SchemeEnv GlobalEnv = StandardEnv();

        internal sealed record SchemeList : IEnumerable<object>
        {
            private object _car;
            private object _cdr;

            internal dynamic Car
            {
                get => _car;
                set => _car = value;
            }

            internal dynamic Cdr
            {
                get => _cdr;
                set => _cdr = value;
            }

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
                        r += Cdr.Count + 1;
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
            public SchemeList ChainAppend(object o)
            {
                if (!(o is SchemeList))
                {
                    Cdr = new SchemeList(o);
                    return Cdr;
                }
                else
                {
                    Cdr = o;
                    return Cdr;
                }
            }

            public IEnumerator<object> GetEnumerator()
            {
                dynamic t = this;
                while (!t.Equals(Nil))
                {
                    var r = t.Car;
                    t = t.Cdr;
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

        private sealed class SchemeEnv : Dictionary<object, dynamic>
        {
            private readonly SchemeEnv _outer;

            public SchemeEnv(IEnumerable parameters, IEnumerable args, SchemeEnv outer = null)
            {
                var paramEnum = parameters.GetEnumerator();
                var argsEnum = args.GetEnumerator();
                while (paramEnum.MoveNext() && argsEnum.MoveNext())
                {
                    this[paramEnum.Current!] = argsEnum.Current;
                }

                _outer = outer;
            }

            public SchemeEnv Find(object variable)
                => ContainsKey(variable) ? this : _outer.Find(variable);
        }

        private const string Nil = "'()";

        private static SchemeEnv StandardEnv()
        {
            var d = new Dictionary<object, object>()
            {
                {
                    "+", new Lambda(args =>
                    {
                        if (args.Count < 1) throw new ArgumentException();
                        double res = 0;
                        dynamic first = args[0];
                        if (args.Count == 1 && first < 0)
                        {
                            return -first;
                        }
                        else
                        {
                            for (int i = 0; i < args.Count; i++)
                            {
                                dynamic t = args[i];
                                res += t;
                            }

                            return res;
                        }
                    })
                },
                {
                    "-", new Lambda(args =>
                    {
                        if (args.Count < 1) throw new ArgumentException();
                        double res = 0;
                        dynamic first = args[0];
                        if (args.Count == 1 && first < 0)
                        {
                            return -first;
                        }
                        else
                        {
                            for (int i = 0; i < args.Count; i++)
                            {
                                dynamic t = args[i];
                                res -= t;
                            }

                            return res;
                        }
                    })
                },
                {
                    "*", new Lambda(args =>
                    {
                        if (args.Count <= 1) throw new ArgumentException();
                        double r = 1;
                        foreach (dynamic i in args)
                        {
                            r *= i;
                        }

                        return r;
                    })
                },
                {
                    "/", new Lambda(args =>
                    {
                        if (args.Count <= 1 || args.Count > 2) throw new ArgumentException();
                        dynamic a = args[0];
                        dynamic b = args[1];
                        return a / b;
                    })
                },
                {
                    ">", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentException();
                        dynamic a = args[0];
                        dynamic b = args[1];
                        return a > b;
                    })
                },
                {
                    "<", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentException();
                        dynamic a = args[0];
                        dynamic b = args[1];
                        return a < b;
                    })
                },
                {
                    ">=", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentException();

                        dynamic a = args[0];
                        dynamic b = args[1];
                        return a >= b;
                    })
                },
                {
                    "<=", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentException();

                        dynamic a = args[0];
                        dynamic b = args[1];
                        return a < b;
                    })
                },
                {
                    "=", new LambdaAction(args =>
                    {
                        if (args.Count != 2) throw new ArgumentException();
                        GlobalEnv[args[0]] = args[1];
                    })
                },
                {
                    "abs", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentException();
                        dynamic res = args[0];
                        if (res < 0) return -res;
                        else return res;
                    })
                },
                {
                    "append", new Lambda(args =>
                    {
                        if (args.Count < 2) throw new ArgumentException();
                        for (int i = 0; i < args.Count - 1; i++)
                        {
                            dynamic list = args[i];
                            dynamic list1 = args[i + 1];
                            list.Append(list1);
                        }

                        return args[0];
                    })
                },
                {
                    "apply", new Lambda(args =>
                    {
                        dynamic proc = args[0];
                        return proc(args);
                    })
                },
                {"begin", new Lambda(args => args[^1])},
                {
                    "car", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentException();
                        return ((SchemeList) args[0]).Car;
                    })
                },
                {
                    "cdr", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentException();
                        return ((SchemeList) args[0]).Cdr;
                    })
                },
                {
                    "cons", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentException();
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
                        if (args.Count != 2) throw new ArgumentException();
                        return args[0].Equals(args[1]);
                    })
                },
                {
                    "expt", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentException();
                        dynamic a = args[0];
                        dynamic b = args[1];
                        return Math.Pow(a, b);
                    })
                },
                {
                    "equal?", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentException();
                        return args[0].Equals(args[1]);
                    })
                },
                {
                    "length", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentException();
                        int len = 1;
                        dynamic ptr = args[0];
                        while (!ptr.Cdr.Equals(Nil))
                        {
                            ptr = ptr.Cdr;
                            len++;
                        }

                        return len;
                    })
                },
                {
                    "list", new Lambda(args =>
                    {
                        if (args.Count < 1) throw new ArgumentException();
                        var res = new SchemeList(args[0]);
                        var p = res;
                        for (int i = 1; i < args.Count; i++)
                        {
                            p = p.ChainAppend(args[i]);
                        }

                        return res;
                    })
                },
                {
                    "list?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentException();
                        return args[0] is SchemeList;
                    })
                },
                {
                    "map", new Lambda(args =>
                    {
                        if (args.Count < 2) throw new ArgumentException();
                        dynamic func = args[0];
                        dynamic dArgs = args;
                        var elements0 = new List<object>();
                        for (int i = 1; i < dArgs.Count; i++)
                        {
                            elements0.Add(dArgs[i].Car);
                            dArgs[i] = dArgs[i].Cdr;
                        }

                        var r = new SchemeList(func(elements0));
                        var p = r;
                        while (true)
                        {
                            var elements = new List<object>();
                            for (int i = 1; i < dArgs.Count; i++)
                            {
                                if (dArgs[i] != ".()")
                                {
                                    elements.Add(dArgs[i].Car);
                                    dArgs[i] = dArgs[i].Cdr;
                                }
                            }

                            if (elements.Count == args.Count - 1)
                            {
                                p = p.ChainAppend(func(elements));
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
                        if (args.Count != 1) throw new ArgumentException();
                        return !(bool) args[0];
                    })
                },
                {
                    "null?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentException();
                        return args[0].Equals(Nil);
                    })
                },
                {
                    "number?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentException();
                        dynamic type = args[0].GetType();
                        return type.Equals(typeof(int)) || type.Equals(typeof(double));
                    })
                },
                {
                    "print", new LambdaAction(args =>
                    {
                        StringBuilder b = new StringBuilder();
                        for (int i = 0; i < args.Count - 1; i++)
                        {
                            b.Append(args[i]);
                            b.Append(' ');
                        }

                        b.Append(args[^1]);
                        Console.WriteLine(b.ToString());
                    })
                },
                {
                    "procedure?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentException();
                        var type = args[0].GetType();
                        return type == typeof(Lambda) || type == typeof(LambdaAction);
                    })
                },
                {
                    "round", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentException();
                        dynamic a = args[0];
                        return Math.Round(a, MidpointRounding.AwayFromZero);
                    })
                },
                {
                    "symbol?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentException();
                        return args[0] is string;
                    })
                },
                {"pi", Math.PI},
                {
                    "quote",
                    new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentException();
                        return args[0];
                    })
                },
            };
            var env = new SchemeEnv(d.Keys, d.Values);
            return env;
        }

        public static dynamic Parse(string program) => ParseTokens(Tokenize(program));

        public static dynamic Eval(dynamic x)
        {
            return Eval(x, GlobalEnv);
        }

        private static dynamic Eval(dynamic x, SchemeEnv env)
        {
            // var env = GlobalEnv;
            if (x.GetType().Equals(typeof(string)))
            {
                return env[x];
            }
            else if (x.GetType().Equals(typeof(int)) || x.GetType().Equals(typeof(double)))
            {
                return x;
            }

            var op = x[0];
            var args = x.GetRange(1, x.Count-1);

            switch (op)
            {
                case "if":
                    dynamic test = args[0];
                    dynamic conseq = args[1];
                    dynamic alt = args[2];
                    dynamic exp = Eval(test) ? conseq : alt;
                    return Eval(exp);
                case "define":
                    dynamic symbol = args[0];
                    dynamic expression = args[1];
                    env[symbol] = Eval(expression, env);
                    return null;
                case "lambda":
                    // TODO implement procedure
                    return null;
                default:
                    dynamic proc = Eval(op, env);
                    List<object> vals = new List<object>();
                    foreach (var arg in args)
                    {
                        vals.Add(Eval(arg,env));
                    }
                    return proc(vals);
            }
        }

        public static void Repl(string prompt = "NScheme>")
        {
            while (true)
            {
                dynamic val = Eval(Parse(ReadLine(prompt)));
                if (val != null)
                {
                    Console.WriteLine(val);
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private delegate dynamic Lambda(List<object> args);

        private delegate void LambdaAction(List<object> args);

        private static string ReadLine(string prompt = "lis.py>>>")
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        /// <summary>
        /// tokens to tree
        /// </summary>
        /// <param name="tokens">list of tokens</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static dynamic ParseTokens(Queue<string> tokens)
        {
            if (tokens.Count == 0) throw new ArgumentException("unexpected EOF");
            var token = tokens.Dequeue();
            switch (token)
            {
                case "(":
                {
                    var l = new List<object>();
                    while (!tokens.Peek().Equals(")"))
                    {
                        l.Add(ParseTokens(tokens));
                    }

                    tokens.Dequeue();
                    return l;
                }
                case ")":
                    throw new ArgumentException("unexpected ')'");
                default:
                    return ConvertToAtom(token);
            }
        }

        /// <summary>
        /// string to list of tokens
        /// </summary>
        /// <param name="program">string</param>
        /// <returns>tokens list</returns>
        private static Queue<string> Tokenize(string program)
        {
            var t = program.Replace("(", " ( ").Replace(")", " ) ").Split().ToList();
            t.RemoveAll(s => s.Equals(""));
            Queue<string> res = new Queue<string>();
            foreach (var i in t)
            {
                res.Enqueue(i);
            }

            return res;
        }

        private static dynamic ConvertToAtom(string x)
        {
            bool succ = int.TryParse(x, out int t);
            if (!succ)
            {
                bool succ1 = double.TryParse(x, out double t1);
                if (!succ1)
                {
                    return x;
                }
                else
                {
                    return t1;
                }
            }
            else
            {
                return t;
            }
        }
    }
}