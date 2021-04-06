﻿#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using static CSharpLibraries.Interpreters.InterpretersExceptions;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using static CSharpLibraries.Interpreters.Env;

[assembly: InternalsVisibleTo("CSarpLibrariesTest")]

namespace CSharpLibraries.Interpreters
{
    public sealed class Lisp
    {
        internal static readonly IList<object> Nil = new List<object>(0);

        private readonly Env _globalEnv = StandardEnv();

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

        public static object Parse(string program) => ParseTokens(Tokenize(program));

        private object Eval(object x) => Eval(x, _globalEnv);

        private static object Eval(object x, Env currentEnv)
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
                    return new Lambda(arguments => Eval(body, new Env(parameters, arguments, currentEnv)));
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
        private static object ParseTokens(Queue<string> tokens)
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

        private static object ToAtom(string x)
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
            if (o is bool b)
            {
                return b;
            }
            else if (o == null)
            {
                return false;
            }
            else if (o is int || o is double)
            {
                return !o.Equals(0);
            }
            else
            {
                throw new SyntaxException("not bool");
            }
        }
    }
}