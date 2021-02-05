#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpLibraries.Demos
{
    public static class Scheme
    {
        private static readonly Dictionary<object, dynamic> GlobalEnv = StandardEnv();

        private sealed record SchemeList
        {
            internal object Car;
            internal object Cdr;

            
            public SchemeList(){}
            public SchemeList(object car, object cdr)
            {
                Car = car;
                Cdr = cdr;
            }
        }

        private static Dictionary<object, object> StandardEnv()
        {
            return new()
            {
                {"+",new Lambda(args =>
                {
                    if (args.Count < 1) throw new ArgumentException();
                    var res = ConvertToNumber(args[0]);
                    if (args.Count == 1 && res < 0)
                    {
                        res = -res;
                    }
                    for (int i = 1; i < args.Count; i++)
                    {
                        res += args[i];
                    }
        
                    return res;
                })}, 
                {"-",new Lambda(args =>
                {
                    if (args.Count < 1) throw new ArgumentException();
                    var res = ConvertToNumber(args[0]);
                    if (args.Count == 1 && res > 0)
                    {
                        res = -res;
                    }
                    for (int i = 1; i < args.Count; i++)
                    {
                        res -= args[i];
                    }
        
                    return res;
                })}, 
                {"*",new Lambda(args =>
                {
                    if (args.Count <= 1) throw new ArgumentException();
                    return args.Aggregate<object, double>(1, 
                        (current, t) => (double) (current * ConvertToNumber(t)));
                })}, 
                {"/",new Lambda(args =>
                {
                    if (args.Count <= 1) throw new ArgumentException();
                    var res = ConvertToNumber(args[0]);
                    if (args.Count == 1 || res < 0)
                    {
                        res = -res;
                    }
                    for (int i = 1; i < args.Count; i++)
                    {
                        res += args[i];
                    }
        
                    return res;
                })},
                {">",new Lambda(args =>
                {
                    if (args.Count != 2) throw new ArgumentException();
        
                    return ConvertToNumber(args[0]) > ConvertToNumber(args[1]);
                })},
                {"<",new Lambda(args =>
                {
                    if (args.Count != 2) throw new ArgumentException();
        
                    return ConvertToNumber(args[0]) < ConvertToNumber(args[1]);
                })},
                {">=",new Lambda(args =>
                {
                    if (args.Count != 2) throw new ArgumentException();
        
                    return ConvertToNumber(args[0]) >= ConvertToNumber(args[1]);
                })},
                {"<=",new Lambda(args =>
                {
                    if (args.Count != 2) throw new ArgumentException();
        
                    return ConvertToNumber(args[0]) <= ConvertToNumber(args[1]);
                })},
                {"=",new LambdaAction(args =>
                {
                    if (args.Count != 2) throw new ArgumentException();
                    GlobalEnv[args[0]] = args[1];
                })},
                {"abs",new Lambda(args =>
                {
                    if (args.Count != 1) throw new ArgumentException();
                    var res = ConvertToNumber(args[0]);
                    if (res < 0) return -res;
                    else return res;
                })},
                {"append",new LambdaAction(args =>
                {
                    if (args.Count !< 2) throw new ArgumentException();
                    SchemeList t = null;
                    for (int i = args.Count - 1; i >= 0; i--)
                    {
                        if (i == args.Count - 1)
                        {
                            t = new SchemeList(args[i], null);
                        }
                        else
                        {
                            t = new SchemeList(args[i], t);
                        }
                    }
                })},
                {"apply",new LambdaAction(args =>
                {
                    dynamic proc = args[0];
                    proc(args);
                })},
                {"begin", new Lambda(args => args.Last())},
                {"car", new Lambda(args =>
                {
                    if (args.Count != 1) throw new ArgumentException();
                    return ((SchemeList)args[0]).Car;
                })},
                {"cdr", new Lambda(args =>
                {
                    if (args.Count != 1) throw new ArgumentException();
                    return ((SchemeList)(args[0])).Cdr;
                })},
                {"cons",new Lambda(args =>
                {
                    if (args.Count != 2) throw new ArgumentException();
                    if (args[0] is SchemeList s)
                    {
                        s.Cdr = args[1];
                        return s;
                    }
                    else
                    {
                        return new SchemeList(args[0], new SchemeList(args[1], null));
                    }
                    
                })},
                {"eq?", new Lambda(args =>
                {
                    if (args.Count != 2) throw new ArgumentException();
                    return args[0].Equals(args[1]);
                })},
                {"expt", new Lambda(args =>
                {
                    if (args.Count != 2) throw new ArgumentException();
                    return Math.Pow(ConvertToNumber(args[0]), ConvertToNumber(args[1]));
                })},
                {"equal?", new Lambda(args =>
                {
                    if (args.Count != 2) throw new ArgumentException();
                    return args[0].Equals(args[1]);
                })},
                {"length", new Lambda(args =>
                {
                    if (args.Count != 1) throw new ArgumentException();
                    int len = 1;
                    dynamic ptr = args[0];
                    while (!ptr.Cdr.Equals("'()"))
                    {
                        ptr = ptr.Cdr;
                        len++;
                    }
                    return len;
                })},
                {"list", new Lambda(args =>
                {
                    if (args.Count < 1) throw new ArgumentException();
                    dynamic res = new SchemeList();
                    res.Car = args[0];
                    if (args.Count == 1)
                    {
                        res.Cdr = "'()";
                    }
                    else
                    {
                        res.Cdr = new SchemeList();
                        var ptr = res.Cdr;
                        for (int i = 1; i < args.Count; i++)
                        {
                            ptr.Car = args[i];
                        }
                    }
        
                    return res;
                })},
                {"list?", new Lambda(args =>
                {
                    if(args.Count != 1) throw new ArgumentException();
                    return args[0] is SchemeList;
                })},
                {"map", new LambdaAction(args => throw new NotImplementedException())},
                {"max", new Lambda(args => args.Max())},
                {"min", new Lambda(args => args.Min())},
                {"not", new Lambda(args =>
                {
                    if(args.Count != 1) throw new ArgumentException();
                    return !(bool)args[0];
                })},
                {"null?", new Lambda(args =>
                {
                    if(args.Count != 1) throw new ArgumentException();
                    return args[0].Equals("'()");
                })}, 
                {"number?", new Lambda(args =>
                {
                    if(args.Count != 1) throw new ArgumentException();
                    dynamic type = args[0].GetType();
                    return type.Equals(typeof(int)) || type.Equals(typeof(double));
                })},
                {"print", new LambdaAction(args => throw new NotImplementedException())},
                {"procedure?", new Lambda(args =>
                {
                    if(args.Count != 1) throw new ArgumentException();
                    dynamic type = args[0].GetType();
                    return type.Equals(typeof(Lambda)) || type.Equals(typeof(LambdaAction));
                })},
                {"round", new Lambda(args =>
                {
                    if(args.Count != 1) throw new ArgumentException();
                    return Math.Round(ConvertToNumber(args[0]), MidpointRounding.AwayFromZero);
                })},
                {"symbol?", new Lambda(args =>
                {
                    if(args.Count != 1) throw new ArgumentException();
                    return ((string)args[0]).StartsWith("'");
                })},
                {"pi", Math.PI}
            };
        }
        
        public static List<object> Parse(string program) => ReadFromTokens(Tokenize(program));
        
        public static dynamic Eval(dynamic x)
        {
            var env = GlobalEnv;
            if (x.GetType().Equals(typeof(string)))
            {
                return env[x];
            }
            else if (x.GetType().Equals(typeof(int)) || x.GetType().Equals(typeof(double)))
            {
                return x;
            }
            else if (x[0] == "if")
            {
                dynamic test = x[1];
                dynamic conseq = x[2];
                dynamic alt = x[3];
                dynamic exp = Eval(test) ? conseq : alt;
                return Eval(exp);
            }
            else if (x[0] == "define")
            {
                dynamic symbol = x[1];
                dynamic exp = x[2];
                env[symbol] = Eval(exp);
                return null;
            }
            else
            {
                dynamic proc = Eval(x[0]);
                List<object> args = new List<object>();
                int i = 0;
                
                foreach (dynamic arg in x)
                {
                    if (i != 0)
                    {
                        args.Add(Eval(arg));
                    }
                    else
                    {
                        i++;
                    }
                }

                return proc(args);
            }
        }

        private delegate dynamic Lambda(List<object> args);

        private delegate void LambdaAction(List<object> args);

        private static dynamic ReadFromTokens(Queue<string> tokens)
        {
            if (tokens.Count == 0) throw new ArgumentException("unexpected EOF");
            var token = tokens.Dequeue();
            if (token.Equals("("))
            {
                var l = new List<object>();
                while (!tokens.Peek().Equals(")"))
                {
                    l.Add(ReadFromTokens(tokens));
                }

                tokens.Dequeue();
                return l;
            }
            else if (token.Equals(")")) throw new ArgumentException("unexpected ')'");
            else return ConvertToAtom(token);
        }

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
                bool succ1 = decimal.TryParse(x, out decimal t1);
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

        private static dynamic ConvertToNumber(object x)
        {
            dynamic res = x switch
            {
                int i => i,
                decimal d => d,
                double db => db,
                _ => throw new ArrayTypeMismatchException("type error")
            };
            return res;
        }
    }
}