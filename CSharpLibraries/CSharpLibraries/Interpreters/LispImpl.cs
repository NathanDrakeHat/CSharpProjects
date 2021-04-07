#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
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
    public sealed partial class Lisp{
        public Lisp(){
            macro_table = new Dictionary<Symbol, Lambda>{{SymLet, new Lambda(Let)}};
            _globalEnv[new Symbol("and")] = new Lambda(
                args => {
                    if (args.Count < 1){
                        return true;
                    }
                    else if (args.Count == 1){
                        return args[0];
                    }
                    else{
                        var t = Eval(Expand(args[0]), _globalEnv);
                        if (ObjectIsTrue(t)){
                            IList<object> newExp = new List<object>(args.Count);
                            newExp.Add(new Symbol("and"));
                            newExp.AddRange(args.Skip(1).ToList());
                            return Eval(Expand(newExp), _globalEnv);
                        }
                        else{
                            return false;
                        }
                    }
                });
            _globalEnv[new Symbol("Eval")] = new Lambda(
                args => {
                    if (args.Count != 1){
                        throw new ArgumentsCountException("1");
                    }

                    return Eval(Expand(args[0]), _globalEnv);
                });
            _globalEnv[new Symbol("load")] = new Lambda(
                args => {
                    if (args.Count != 1){
                        throw new ArgumentsCountException("1");
                    }

                    LoadLib(args[0].ToString(), this);
                    return null;
                });
        }

        private static void LoadLib(string fileName, Lisp interpreter){
            var file = new FileInfo(fileName);
            var inPort = new InputPort(file);
            while (true){
                try{
                    var x = interpreter.Parse(inPort);
                    if (x == null){
                        continue;
                    }
                    else if (x.Equals(SymEof)){
                        return;
                    }

                    Eval(x, interpreter._globalEnv);
                }
                catch (Exception e){
                    Console.Error.WriteLineAsync(e.StackTrace).Wait();
                }
            }
        }
        
        private void EvalAndPrint(object x) {
            var val = Eval(x, _globalEnv);
            if (val != null) {
                Console.Out.WriteLineAsync(EvalToString(val)).Wait();
            }
        }
        
        internal static readonly IList<object> Nil = new List<object>(0);

        private readonly Environment _globalEnv = NewStandardEnv();

        private readonly Dictionary<Symbol, Lambda> macro_table;
        
        internal object Parse(object inPort){
            return Parse(inPort, this);
        }
        
        internal static object Parse(object input, Lisp interpreter){
            switch (input){
                case string sInput:{
                    var t = Read(new InputPort(sInput));
                    return interpreter.Expand(t);
                }
                case InputPort portInput:{
                    var t = Read(portInput);
                    return interpreter.Expand(t);
                }
                default:
                    throw new Exception("unknown error");
            }
        }

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
                return new List<object>(){QuotesDict[(string) token], Read(inPort)};
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
                        return x.Substring(1, x.Length - 2);
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
                    foreach (var exp in l.Skip(1).ToList()) Eval(exp, currentEnv);
                    x = l[^1];
                }
                else{
                    IList<object> express = l.Select(exp => Eval(exp, currentEnv)).ToList();
                    var proc = express[0];
                    express = express.Skip(1).ToList();
                    if (proc is Procedure p){
                        x = p.Expression;
                        currentEnv = new Environment(p.Parameters, express, p.Environment);
                    }
                    else{
                        return ((Lambda) proc).Apply(express);
                    }
                }
            }
        }

        private object Expand(object x, bool topLevel = true){
            Require(x, !ObjectIsNil(x));
            if (!(x is IList<object>)){
                return x;
            }

            IList<object> l = (IList<object>) x;
            var op = l[0];
            if (op.Equals(SymQuote)){
                Require(x, l.Count == 2);
                return x;
            }
            else if (op.Equals(SymIf)){
                if (l.Count == 3){
                    l.Add(null);
                }

                Require(x, l.Count == 4);
                return l.Select(i => Expand(i)).ToList();
            }
            else if (op.Equals(SymSet)){
                Require(x, l.Count == 3);
                var v = l[1];
                Require(x, v is Symbol, "can set! only a symbol");
                return new List<object>(){SymSet, v, Expand(l[2])};
            }
            else if (op.Equals(SymDefine) || op.Equals(SymDefineMacro)){
                Require(x, l.Count >= 3);
                var v = l[1];
                var body = l.Skip(2).ToList();
                if (v is IList<object> lv && lv.Count != 0){
                    var f = lv[0];
                    var args = lv.Skip(1).ToList();
                    var t = new List<object>{SymLambda, args};
                    t.AddRange(body);
                    return Expand(new List<object>{op, f, t});
                }
                else{
                    Require(x, l.Count == 3);
                    Require(x, v is Symbol, "can define only a symbol");
                    var exp = Expand(l[2]);
                    if (op.Equals(SymDefineMacro)){
                        Require(x, topLevel, "define-macro only allowed at top level");
                        var proc = Eval(exp, _globalEnv);
                        Require(x, proc is Lambda, "macro must be a procedure");
                        macro_table[(Symbol) v] = (Lambda) proc;
                        return null;
                    }

                    return new List<object>{SymDefine, v, exp};
                }
            }
            else if (op.Equals(SymBegin)){
                if (l.Count == 1){
                    return null;
                }
                else{
                    return l.Select(i => Expand(i, topLevel)).ToList();
                }
            }
            else if (op.Equals(SymLambda)){
                Require(x, l.Count >= 3);
                var vars = l[1];
                var body = l.Skip(2).ToList();
                Require(x, (vars is IList<object> list &&
                            list.All(v => v is Symbol)) ||
                           vars is Symbol, "illegal lambda argument list");
                object exp;
                if (body.Count == 1){
                    exp = body[0];
                }
                else{
                    IList<object> t = new List<object>(body.Count + 1);
                    exp = t;
                    t.Add(SymBegin);
                    t.AddRange(body);
                }

                return new List<object>{SymLambda, vars, Expand(exp)};
            }
            else if (op.Equals(SymQuasiQuote)){
                Require(x, l.Count == 2);
                return ExpandQuasiQuote(l[1]);
            }
            else if (op is Symbol symbol && macro_table.ContainsKey(symbol)){
                return Expand(macro_table[symbol].Apply(l.Skip(1).ToList()), topLevel);
            }
            else{
                return l.Select(i => Expand(i)).ToList();
            }
        }

        private static object ExpandQuasiQuote(object x){
            if (!IsPair(x)){
                return new List<object>{SymQuote, x};
            }

            IList<object> l = (IList<object>) x;
            Require(x, !l[0].Equals(SymUnquoteSplicing), "can't splice here");
            if (l[0].Equals(SymUnquote)){
                Require(x, l.Count == 2);
                return l[1];
            }
            else if (IsPair(l[0]) && ((IList<object>) l[0])[0].Equals(SymUnquoteSplicing)){
                Require(l[0], ((IList<object>) l[0]).Count == 2);
                return new List<object>{SymAppend, ((IList<object>) l[0])[1], ExpandQuasiQuote(l.Skip(1).ToList())};
            }
            else{
                return new List<object>{SymCons, ExpandQuasiQuote(l[0]), ExpandQuasiQuote(l.Skip(1).ToList())};
            }
        }

        private static bool IsPair(object x){
            return !ObjectIsNil(x) && x is IList<object>;
        }

        private static void Require(object x, bool predicate, string m = " wrong length"){
            if (!predicate){
                throw new SyntaxException(EvalToString(x) + m);
            }
        }

        private static string ReadLine(string prompt){
            Console.Out.WriteLineAsync(prompt).Wait();
            return Console.ReadLine();
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

        internal static bool ObjectIsTrue(object o){
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

        private object Let(IList<object> args){
            return Let(args, this);
        }

        private static object Let(IList<object> args, Lisp interpreter){
            IList<object> x = new List<object>{SymLet};
            x.Add(args);
            Require(x, x.Count > 1);
            IList<IList<object>> bindings;
            try{
                bindings = (IList<IList<object>>) args[0];
            }
            catch (InvalidCastException){
                throw new InvalidCastException("illegal binding list");
            }

            IList<object> body = args.Skip(1).ToList();
            Require(x, bindings.All(b => b != null &&
                                         b.Count == 2 &&
                                         b[0] is Symbol), "illegal binding list");
            IList<object> vars = bindings.Select(l => l[0]).ToList();
            IList<object> vals = bindings.Select(l => l[1]).ToList();
            var t = new List<object>{SymLambda, vars};
            t.AddRange(body.Select(i => interpreter.Expand(i)).ToList());
            var r = new List<object>{t};
            r.AddRange(vals.Select(i => interpreter.Expand(i)).ToList());
            return r;
        }

        internal static object Callcc(Lambda proc){
            var ball = new RuntimeWarning("Sorry, can't continue this continuation any longer.");
            try{
                return proc.Apply(new List<object>{
                    new Lambda(objects => {
                        Raise(objects[0], ball);
                        return null;
                    })
                });
            }
            catch (RuntimeWarning w){
                if (w.Equals(ball)){
                    return ball.returnValue;
                }
                else{
                    throw;
                }
            }
        }

        private static void Raise(object r, RuntimeWarning ball){
            ball.returnValue = r;
            throw ball;
        }
    }
}