#nullable disable
using System;
using System.Collections.Generic;
using static CSharpLibraries.Interpreters.InterpretersExceptions;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using static CSharpLibraries.Interpreters.Environment;
using static CSharpLibraries.Interpreters.Symbol;

[assembly: InternalsVisibleTo("CSarpLibrariesTest")]

namespace CSharpLibraries.Interpreters{
    public sealed class LispInterpreter{
        internal static readonly IList<object> Nil = new List<object>(0);

        private readonly Environment _globalEnv = NewStandardEnv();

        public object RunScheme(string program) => Eval(Parse(program), _globalEnv);

        public void Repl(string prompt = "NScheme>"){
            while (true){
                var s = ReadLine(prompt);
                if (s.Equals("")){
                    continue;
                }

                object val = null;
                try{
                    val = Eval(Parse(s), _globalEnv);
                }
                catch (Exception e){
                    Console.WriteLine($"{e.GetType().Name}: {e.Message}\n");
                }

                if (val != null){
                    Console.WriteLine(val);
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

        public object Parse(string program) => ParseTokens(Tokenize(program));

        
        private static object Read(InputPort inPort){
            var token = inPort.NextToken();
            return token.Equals(SymEof) ? SymEof : ReadAhead(token, inPort);
        }
        
        private static object ReadAhead(object token, InputPort inPort){
            if (token.Equals("(")){
                IList<object> l = new List<object>();
                while (true){
                    token = inPort.NextToken();
                    if (token.Equals(")")){
                        return l;
                    }
                    else{
                        l.Add(ReadAhead(token, inPort));
                    }
                }
            }
            else if (token.Equals(")")){
                throw new SyntaxException("unexpected )");
            }
            else if (QuotesDict.ContainsKey((string) token)){
                return new List<object>(){QuotesDict[(string)token], Read(inPort)};
            }
            else if (token.Equals(SymEof)){
                throw new SyntaxException("unexpected EOF in list");
            }
            else{
                return ToAtom((string) token);
            }
        }

        private static object ToAtom(string x){
            switch (x){
                case "#t":
                    return true;
                case "#f":
                    return false;
                default:{
                    if (x.StartsWith("\"") && x.EndsWith("\"")){
                        return x.Substring(1, x.Length - 1);
                    }
                    else if (x.ContainsDigit()){
                        if (int.TryParse(x, out int iX)){
                            return iX;
                        }
                        else{
                            if (double.TryParse(x, out double dX)){
                                return dX;
                            }
                            else{
                                if (Utils.TryParseImaginary(x, out Complex? cX)){
                                    return cX;
                                }
                                else{
                                    return new Symbol(x);
                                }
                            }
                        }
                    }
                    else{
                        return new Symbol(x);
                    }
                }
            }
        }

        internal static object Eval(object x, Environment currentEnv){
            while (true){
                if (x is Symbol){
                    return currentEnv.Find(x)[x];
                }
                else if (!(x is IList<object>)){
                    return x;
                }

                IList<object> l = (IList<object>) x;
                var op = l[0];
                if (op.Equals(SymQuote)){
                    return l[1];
                }
                else if (op.Equals(SymIf)){
                    var test = l[1];
                    var conseq = l[2];
                    var alt = l[3];
                    bool testBool = ObjectIsTrue(Eval(test, currentEnv));
                    x = testBool ? conseq : alt;
                }
                else if (op.Equals(SymSet)){
                    var v = l[1];
                    var exp = l[2];
                    currentEnv.Find(v)[v] = Eval(exp, currentEnv);
                    return null;
                }
                else if (op.Equals(SymDefine)){
                    var v = l[1];
                    var exp = l[2];
                    currentEnv[v] = Eval(exp, currentEnv);
                    return null;
                }
                else if (op.Equals(SymLambda)){
                    var vars = l[1];
                    var exp = l[2];
                    return new Procedure(vars, exp, currentEnv);
                }
                else if (op.Equals(SymBegin)){
                    foreach (var exp in l.Take(1).ToList()) Eval(exp, currentEnv);
                    x = l[^1];
                }
                else{
                    Environment finalEnv = currentEnv;
                    IList<object> expression = l.Select(exp => Eval(exp, finalEnv)).ToList();
                    var proc = expression[0];
                    expression = expression.Take(1).ToList();
                    if (proc is Procedure p){
                        x = p.Expression;
                        currentEnv = new Environment(p.Parameters, expression, p.Environment);
                    }
                    else{
                        return ((Lambda) proc).Apply(expression);
                    }
                }
            }
        }

        private static string ReadLine(string prompt){
            Console.Write(prompt);
            return Console.ReadLine();
        }

        /// <summary>
        /// tokens to string tree
        /// </summary>
        /// <param name="tokens">list of tokens</param>
        /// <returns></returns>
        /// <exception cref="SyntaxException"></exception>
        private object ParseTokens(Queue<string> tokens){
            if (tokens.Count == 0) throw new SyntaxException("unexpected EOF");
            var token = tokens.Dequeue();
            switch (token){
                case "(":{
                    var l = new List<object>();
                    while (!tokens.Peek().Equals(")")){
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
        private Queue<string> Tokenize(string program){
            var t = program.Replace("(", " ( ").Replace(")", " ) ").Split().ToList();
            t.RemoveAll(s => s.Equals(""));
            Queue<string> res = new Queue<string>();
            foreach (var i in t){
                res.Enqueue(i);
            }

            return res;
        }


        internal static string EvalToString(object x){
            if (x == null){
                return null;
            }
            else if (x.Equals(true)){
                return "#t";
            }
            else if (x.Equals(false)){
                return "#f";
            }
            else
                switch (x){
                    case Symbol symX:
                        return symX.ToString();
                    case String strX:
                        return Regex.Unescape(strX);
                    case List<object> lx:{
                        var s = new StringBuilder("(");
                        foreach (var i in lx){
                            s.Append(EvalToString(i));
                            s.Append(' ');
                        }

                        if (lx.Count >= 1){
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

        internal static bool ObjectIsNil(object o){
            if (o is List<object> l){
                return l.Count == 0;
            }
            else{
                return false;
            }
        }

        static bool ObjectIsTrue(object o){
            switch (o){
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