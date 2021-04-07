using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using static CSharpLibraries.Interpreters.Lisp;
using static CSharpLibraries.Interpreters.Utils;
using static CSharpLibraries.Interpreters.InterpretersExceptions;

namespace CSharpLibraries.Interpreters{
    internal sealed class Environment : Dictionary<object, object>{
        private readonly Environment _outer;

        internal Environment(object parameters, IEnumerable<object> args, Environment outer){
            if (parameters is Symbol){
                this[parameters] = args;
            }
            else{
                var p = (IEnumerable<object>) parameters;
                if (args.Count() == p.Count()){
                    using var pi = p.GetEnumerator();
                    using var ai = args.GetEnumerator();
                    while (pi.MoveNext() && ai.MoveNext()){
                        if (pi.Current == null) throw new Exception();
                        this[pi.Current] = ai.Current;
                    }
                }
                else{
                    throw new TypeException($"expected {EvalToString(parameters)}, given {EvalToString(args)}");
                }
            }

            _outer = outer;
        }

        private Environment(IEnumerable<DictionaryEntry> entries){
            foreach (var entry in entries){
                this[entry.Key] = entry.Value;
            }

            _outer = null;
        }

        internal Environment FindEnv(object variable){
            if (ContainsKey(variable)){
                return this;
            }
            else if (_outer == null){
                throw new LookUpException(variable.ToString());
            }
            else{
                return _outer.FindEnv(variable);
            }
        }

        public override string ToString(){
            var s = new StringBuilder();
            foreach (KeyValuePair<object, object> keyValuePair in this){
                s.Append($"[{keyValuePair.Key}:{keyValuePair.Value}]\n");
            }

            s.Remove(s.Length - 1, 1);
            return s.ToString();
        }

        internal static Environment NewStandardEnv(){
            var d = new List<DictionaryEntry>(){
                new(
                    new Symbol("+"), new Lambda(args => {
                        if (args.Count < 1) throw new ArgumentsCountException(">=2");

                        if (args.Count == 1){
                            object first = args[0];
                            if (LessThan(first, 0)){
                                first = Negative(first);
                            }

                            return first;
                        }
                        else{
                            return args.Aggregate((o1, o2) => {
                                object a = o1;
                                object b = o2;
                                return Plus(a, b);
                            });
                        }
                    })
                ),
                new(new Symbol("-"), new Lambda(args => {
                    object first = args[0];
                    switch (args.Count){
                        case 1:
                            if (LessThan(0, first)){
                                first = Negative(first);
                            }

                            return first;
                        case 2:
                            object second = args[1];
                            return Minus(first, second);
                        default:
                            throw new ArgumentsCountException("1 or 2");
                    }
                })),
                new(new Symbol("*"), new Lambda(args => {
                    if (args.Count < 2) throw new ArgumentsCountException(">= 2");
                    return args.Aggregate(Multiply);
                })),
                new(
                    new Symbol("/"), new Lambda(args => {
                        if (args.Count < 2) throw new ArgumentsCountException("2");
                        return args.Aggregate(Divide);
                    })),
                new(new Symbol(">"), new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    object a = args[0];
                    object b = args[1];
                    return LessThan(b, a);
                })),
                new(new Symbol("<"), new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    object a = args[0];
                    object b = args[1];
                    return LessThan(a, b);
                })),
                new(new Symbol(">="), new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");

                    object a = args[0];
                    object b = args[1];
                    return LessThan(b, a) || ValueEqual(a, b);
                })),
                new(new Symbol("<="), new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");

                    object a = args[0];
                    object b = args[1];
                    return LessThan(a, b) || ValueEqual(a, b);
                })),
                new(new Symbol("="), new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    object a = args[0];
                    object b = args[1];
                    return ValueEqual(a, b);
                })),
                new(new Symbol("abs"), new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    object res = args[0];
                    if (LessThan(res, 0)) return Negative(res);
                    else return res;
                })),
                new(new Symbol("append"), new Lambda(args => {
                    if (args.Count < 2) throw new ArgumentsCountException(">=2");
                    IList<object> res = new List<object>((IList<object>) args[0]);
                    for (int i = 1; i < args.Count; i++){
                        res.AddRange((IList<object>) args[i]);
                    }

                    return res;
                })),
                new(new Symbol("apply"), new Lambda(args => {
                    Lambda proc = (Lambda) args[0];
                    return proc.Apply(args.Skip(1).ToList());
                })),
                new(new Symbol("begin"), new Lambda(args => args[^1])),
                new(new Symbol("car"), new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return ((IList<object>) args[0])[0];
                })),
                new(new Symbol("cdr"), new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return ((IList<object>) args[0]).Skip(1).ToList();
                })),
                new(new Symbol("cons"), new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    var content = (IList<object>) args[1];
                    IList<object> t = new List<object>(content.Count + 1);
                    t.Add(args[0]);
                    t.AddRange(content);
                    return t;
                })),
                new(new Symbol("eq?"), new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    return ReferenceEquals(args[0], args[1]);
                })),
                new(new Symbol("expt"), new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    var a = Value(args[0]);
                    var b = Value(args[1]);
                    return Math.Pow(a, b);
                })),
                new(new Symbol("equal?"), new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    return args[0].Equals(args[1]);
                })),
                new(new Symbol("length"), new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return ((IList<object>) args[0]).Count;
                })),
                new(new Symbol("list"), new Lambda(args => {
                    if (args.Count < 1){
                        throw new ArgumentsCountException(">=1");
                    }

                    return new List<object>(args);
                })),
                new(new Symbol("list?"), new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return args[0] is IList<object>;
                })),
                new(new Symbol("map"), new Lambda(args => {
                    if (args.Count < 2) throw new ArgumentsCountException(">=2");
                    Lambda func = (Lambda) args[0];
                    var lists = args.Skip(1).ToList();
                    IList<object> r = new List<object>(((IList<object>) lists[0]).Count);
                    while (true){
                        IList<object> vals = new List<object>(lists.Count);
                        for (int i = 0; i < lists.Count; i++){
                            if (!ObjectIsNil(lists[i])){
                                var list = (IList<object>) lists[i];
                                vals.Add(list[0]);
                                lists[i] = list.Skip(1).ToList();
                            }
                        }

                        if (vals.Count == lists.Count){
                            r.Add(func.Apply(vals));
                        }
                        else{
                            break;
                        }
                    }

                    return r;
                })),
                new(new Symbol("max"), new Lambda(args => args.Max())),
                new(new Symbol("min"), new Lambda(args => args.Min())),
                new(new Symbol("not"), new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return !(bool) args[0];
                })),
                new(new Symbol("null?"), new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return ObjectIsNil(args[0]);
                })),
                new(new Symbol("number?"), new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return (args[0] is int) || (args[0] is double) || (args[0] is Complex);
                })),
                new(new Symbol("print"), new Lambda(args => {
                    if (args.Count != 1){
                        throw new ArgumentsCountException("1");
                    }

                    Console.Out.WriteLine(EvalToString(args[0]));
                    return null;
                })),
                new(new Symbol("procedure?"), new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return args[0] is Procedure;
                })),
                new(new Symbol("round"), new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    var a = Value(args[0]);
                    return Math.Round(a, MidpointRounding.AwayFromZero);
                })),
                new(new Symbol("symbol?"), new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return args[0] is Symbol;
                })),
                new(new Symbol("pi"), Math.PI),
                new(new Symbol("nil"),
                    Nil),
                new(
                    new Symbol("boolean?"),
                    new Lambda(
                        args => {
                            if (args.Count != 1){
                                throw new ArgumentsCountException("1");
                            }

                            return args[0] is bool;
                        })
                ),
                new(new Symbol("port?"),
                    new Lambda(args => {
                        if (args.Count != 1){
                            throw new ArgumentsCountException("1");
                        }

                        return args[0] is FileInfo;
                    })),
                new(new Symbol("call/cc"), new Lambda(args => {
                    if (args.Count != 1){
                        throw new ArgumentsCountException("1");
                    }

                    return Callcc((Lambda) args[0]);
                })),
                new(new Symbol("sqrt"), new Lambda(args => {
                    if (args.Count != 1){
                        throw new ArgumentsCountException("1");
                    }

                    var t = args[0];
                    if (t is int i){
                        if (i >= 0){
                            return Math.Sqrt(i);
                        }
                        else{
                            Complex c = new Complex(i, 0);
                            return Complex.Sqrt(c);
                        }
                    }
                    else if (t is double d1){
                        if (d1 >= 0){
                            return Math.Sqrt(d1);
                        }
                        else{
                            Complex c = new Complex(d1, 0);
                            return Complex.Sqrt(c);
                        }
                    }
                    else if (t is Complex c){
                        return Complex.Sqrt(c);
                    }
                    else{
                        throw new SyntaxException(EvalToString(t) + " is not number");
                    }
                })),

                new(new Symbol("display"), new Lambda(
                    args => {
                        if (args.Count != 1){
                            throw new ArgumentsCountException("1");
                        }

                        Console.Out.WriteLine(EvalToString(args[0]));
                        return null;
                    })),
                new(new Symbol("port?"), new Lambda(
                    args => {
                        if (args.Count != 1){
                            throw new ArgumentsCountException("1");
                        }

                        if (args[0] is string){
                            return new FileInfo((string) args[0]).Exists;
                        }
                        else if (args[0] is Symbol){
                            return new FileInfo(((Symbol) args[0]).Value).Exists;
                        }
                        else{
                            throw new Exception("unknown error");
                        }
                    }))
            };
            return new Environment(d);
        }
    }
}