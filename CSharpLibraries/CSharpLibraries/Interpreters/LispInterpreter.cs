#nullable disable
using System;
using System.Collections.Generic;
using static CSharpLibraries.Interpreters.InterpretersExceptions;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Lambda = System.Func<System.Collections.Generic.List<object>, object>;
using static CSharpLibraries.Interpreters.Environment;

[assembly: InternalsVisibleTo("CSarpLibrariesTest")]

namespace CSharpLibraries.Interpreters
{
    public sealed class LispInterpreter
    {
        internal static readonly IList<object> Nil = new List<object>(0);

        private readonly Environment _globalEnv = StandardEnv();

        public object RunScheme(string program) => Eval(Parse(program));

        public void Repl(string prompt = "NScheme>")
        {
#if DEBUG
            var w = new Stopwatch();
#endif

            while (true)
            {
                var s = ReadLine(prompt);
                if (s.Equals(""))
                {
                    continue;
                }

                object val = null;
                try
                {
#if DEBUG
                    w.Restart();
#endif
                    val = Eval(Parse(s));
#if DEBUG
                    Console.WriteLine($"time:{w.ElapsedMilliseconds}ms");
#endif
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.GetType().Name}: {e.Message}\n");
#if DEBUG
                    Console.WriteLine(e.StackTrace);
#endif
                }

                if (val != null)
                {
                    Console.WriteLine(val);
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

        public object Parse(string program) => ParseTokens(Tokenize(program));

        private object Eval(object x) => Eval(x, _globalEnv);

        private static object Eval(object x, Environment currentEnv)
        {
            if (x is string)
            {
                return currentEnv.Find(x)[x];
            }
            else if (!(x is List<object>))
            {
                return x;
            }

            List<object> list = (List<object>) x;
            var op = list[0];
            var args = list.GetRange(1, list.Count - 1);

            switch (op)
            {
                case "if":
                    object test = args[0];
                    object conSeq = args[1];
                    object alt = args[2];
                    object exp = (bool) Eval(test, currentEnv) ? conSeq : alt;
                    return Eval(exp, currentEnv);
                case "define":
                    object symbol = args[0];
                    object expression = args[1];
                    currentEnv[symbol] = Eval(expression, currentEnv);
                    return null;
                case "lambda":
                    IEnumerable<object> parameters = (IEnumerable<object>) args[0];
                    object body = args[1];
                    return new Lambda(arguments => Eval(body, new Environment(parameters, arguments, currentEnv)));
                default:
                    Lambda proc = (Lambda) Eval(op, currentEnv);
                    List<object> vals = args.Select(arg => Eval(arg, currentEnv)).ToList();

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
        /// <exception cref="SyntaxException"></exception>
        private object ParseTokens(Queue<string> tokens)
        {
            if (tokens.Count == 0) throw new SyntaxException("unexpected EOF");
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
                    throw new SyntaxException("unexpected ')'");
                default:
                    return ToAtom(token);
            }
        }

        /// <summary>
        /// string to list of tokens
        /// </summary>
        /// <param name="program">string</param>
        /// <returns>tokens list</returns>
        private Queue<string> Tokenize(string program)
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

        private object ToAtom(string x)
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

        internal static string EvalToString(object x)
        {
            if (x == null)
            {
                return null;
            }
            else if (x.Equals(true))
            {
                return "#t";
            }
            else if (x.Equals(false))
            {
                return "#f";
            }
            else switch (x)
            {
                case Symbol symX:
                    return symX.ToString();
                case String strX:
                    return Regex.Unescape(strX);
                case List<object> lx:
                {
                    var s = new StringBuilder("(");
                    foreach (var i in lx)
                    {
                        s.Append(EvalToString(i));
                        s.Append(' ');
                    }

                    if (lx.Count >= 1)
                    {
                        s.Remove(s.Length - 1, 1);
                    }

                    s.Append(')');
                    return s.ToString();
                }
                case Complex comX:
                    // TODO formatter
                    return comX.ToString();
                default:
                    return x.ToString();
            }
        }

        internal static bool ObjectIsNil(object o)
        {
            if (o is List<object> l)
            {
                return l.Count == 0;
            }
            else
            {
                return false;
            }
        }

        static bool ObjectIsTrue(object o)
        {
            switch (o)
            {
                case bool b:
                    return b;
                case null:
                    return false;
                case int:
                case double:
                    return !o.Equals(0);
                default:
                    throw new SyntaxException("not bool");
            }
        }
    }
}