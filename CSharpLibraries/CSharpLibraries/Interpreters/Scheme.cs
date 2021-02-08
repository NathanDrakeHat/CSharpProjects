#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;

[assembly: InternalsVisibleTo("CSarpLibrariesTest")]

namespace CSharpLibraries.Interpreters
{
    public static class Scheme
    {
        private class ArgumentAmountException : Exception
        {
            private readonly string _number;
            public ArgumentAmountException(string a)
            {
                _number = a;
            }

            public override string ToString()
            {
                return $"expected: {_number}";
            }
        }

        private class ParseException : Exception
        {
            public ParseException(string s) :base(s)
            {
                
            }
        }

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
            public SchemeList ChainAdd(object o)
            {
                if (!(o is SchemeList))
                {
                    Cdr = new SchemeList(o);
                    return Cdr;
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
                        Cdr.Append(other);
                    }
                }
                else
                {
                    throw new InvalidCastException();
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

        public sealed class Env : Dictionary<object, dynamic>
        {
            private readonly Env _outer;

            public Env(IEnumerable parameters, IEnumerable args, Env outer = null)
            {
                var paramEnum = parameters.GetEnumerator();
                var argsEnum = args.GetEnumerator();
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
                foreach (KeyValuePair<object,dynamic> keyValuePair in this)
                {
                    s.Append($"[{keyValuePair.Key}:{keyValuePair.Value}]\n");
                }

                s.Remove(s.Length - 1, 1);
                return s.ToString();
            }
        }

        private const string Nil = "'()";
        
        private delegate dynamic Lambda(List<object> args);
        
        private static readonly Env GlobalEnv = StandardEnv();

        private static Env StandardEnv()
        {
            var d = new Dictionary<object, object>()
            {
                {
                    "+", new Lambda(args =>
                    {
                        if (args.Count < 1) throw new ArgumentAmountException(">=2");
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
                        dynamic first = args[0];
                        switch (args.Count)
                        {
                            case 1:
                                if (first > 0)
                                {
                                    first = -first;
                                }

                                return first;
                            case 2:
                                dynamic second = args[1];
                                return first - second;
                            default:
                                throw new ArgumentAmountException("1 or 2");
                        }
                    })
                },
                {
                    "*", new Lambda(args =>
                    {
                        if (args.Count <= 1) throw new ArgumentAmountException("> 1");
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
                        if (args.Count != 2) throw new ArgumentAmountException("2");
                        dynamic a = args[0];
                        dynamic b = args[1];
                        return a / b;
                    })
                },
                {
                    ">", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentAmountException("2");
                        dynamic a = args[0];
                        dynamic b = args[1];
                        return a > b;
                    })
                },
                {
                    "<", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentAmountException("2");
                        dynamic a = args[0];
                        dynamic b = args[1];
                        return a < b;
                    })
                },
                {
                    ">=", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentAmountException("2");

                        dynamic a = args[0];
                        dynamic b = args[1];
                        return a >= b;
                    })
                },
                {
                    "<=", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentAmountException("2");

                        dynamic a = args[0];
                        dynamic b = args[1];
                        return a <= b;
                    })
                },
                {
                    "=", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentAmountException("2");
                        dynamic a = args[0];
                        dynamic b = args[1];
                        return a.Equals(b);
                    })
                },
                {
                    "abs", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentAmountException("1");
                        dynamic res = args[0];
                        if (res < 0) return -res;
                        else return res;
                    })
                },
                {
                    "append", new Lambda(args =>
                    {
                        if (args.Count < 2) throw new ArgumentAmountException(">=2");
                        for (int i = 0; i < args.Count - 1; i++)
                        {
                            dynamic list = args[i];
                            var list1 = args[i + 1];
                            list.Append(list1);
                        }

                        return args[0];
                    })
                },
                {
                    "apply", new Lambda(args =>
                    {
                        dynamic proc = args[0];
                        return proc(args.GetRange(1, args.Count - 1));
                    })
                },
                {"begin", new Lambda(args => args[^1])},
                {
                    "car", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentAmountException("1");
                        return ((SchemeList) args[0]).Car;
                    })
                },
                {
                    "cdr", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentAmountException("1");
                        return ((SchemeList) args[0]).Cdr;
                    })
                },
                {
                    "cons", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentAmountException("2");
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
                        if (args.Count != 2) throw new ArgumentAmountException("2");
                        return ReferenceEquals(args[0],args[1]);
                    })
                },
                {
                    "expt", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentAmountException("2");
                        dynamic a = args[0];
                        dynamic b = args[1];
                        return Math.Pow(a, b);
                    })
                },
                {
                    "equal?", new Lambda(args =>
                    {
                        if (args.Count != 2) throw new ArgumentAmountException("2");
                        return args[0].Equals(args[1]);
                    })
                },
                {
                    "length", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentAmountException("1");
                        dynamic list = args[0];
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
                        if (args.Count != 1) throw new ArgumentAmountException("1");
                        return args[0] is SchemeList;
                    })
                },
                {
                    "map", new Lambda(args =>
                    {
                        if (args.Count < 2) throw new ArgumentAmountException(">=2");
                        dynamic func = args[0];
                        dynamic lists = args.GetRange(1, args.Count-1);
                        var elements0 = new List<object>();
                        for (int i = 0; i < lists.Count; i++)
                        {
                            elements0.Add(lists[i].Car);
                            lists[i] = lists[i].Cdr;
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
                                    elements.Add(lists[i].Car);
                                    lists[i] = lists[i].Cdr;
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
                        if (args.Count != 1) throw new ArgumentAmountException("1");
                        return !(bool) args[0];
                    })
                },
                {
                    "null?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentAmountException("1");
                        return args[0].Equals(Nil);
                    })
                },
                {
                    "number?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentAmountException("1");
                        dynamic type = args[0].GetType();
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
                        if (args.Count != 1) throw new ArgumentAmountException("1");
                        var type = args[0].GetType();
                        return type == typeof(Lambda);
                    })
                },
                {
                    "round", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentAmountException("1");
                        dynamic a = args[0];
                        return Math.Round(a, MidpointRounding.AwayFromZero);
                    })
                },
                {
                    "symbol?", new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentAmountException("1");
                        return args[0] is string;
                    })
                },
                {"pi", Math.PI},
                {
                    "quote",
                    new Lambda(args =>
                    {
                        if (args.Count != 1) throw new ArgumentAmountException("1");
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

        public static dynamic RunScheme(string program) => Eval(Parse(program));
        
        public static void Repl(string prompt = "NScheme>")
        {
            var timer = new Stopwatch();
            while (true)
            {
                var s = ReadLine(prompt);
                if (s.Equals(""))
                {
                    continue;
                }
                dynamic val = null;
                try
                {
                    timer.Reset();
                    val = Eval(Parse(s));
                    Console.WriteLine($"timer:{timer.ElapsedMilliseconds}ms");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.GetType().Name}: {e.Message}\n{e.StackTrace}");
                }
                if (val != null)
                {
                    Console.WriteLine(val);
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

        public static dynamic Parse(string program) => ParseTokens(Tokenize(program));

        private static dynamic Eval(dynamic x) => Eval(x, GlobalEnv);
        
        private static dynamic Eval(dynamic x, Env currentEnv)
        {
            if (x.GetType().Equals(typeof(string)))
            {
                return currentEnv.Find(x)[x];
            }
            else if (!(x is List<object>))
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
                    dynamic exp = Eval(test, currentEnv) ? conseq : alt;
                    return Eval(exp,currentEnv);
                case "define":
                    dynamic symbol = args[0];
                    dynamic expression = args[1];
                    currentEnv[symbol] = Eval(expression, currentEnv);
                    return null;
                case "lambda":
                    dynamic parameters = args[0];
                    dynamic body = args[1];
                    return new Lambda(arguments => Eval(body, new Env(parameters, arguments, currentEnv)));
                default:
                    dynamic proc = Eval(op, currentEnv);
                    List<object> vals = new List<object>();
                    foreach (var arg in args)
                    {
                        vals.Add(Eval(arg,currentEnv));
                    }
                    return proc(vals);
            }
        }

        private static string ReadLine(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        /// <summary>
        /// tokens to string tree
        /// </summary>
        /// <param name="tokens">list of tokens</param>
        /// <returns></returns>
        /// <exception cref="ParseException"></exception>
        private static dynamic ParseTokens(Queue<string> tokens)
        {
            if (tokens.Count == 0) throw new ParseException("unexpected EOF");
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
                    throw new ParseException("unexpected ')'");
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