using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using static CSharpLibraries.Interpreters.LispInterpreter;
using static CSharpLibraries.Interpreters.NumericOperators;
using static CSharpLibraries.Interpreters.InterpretersExceptions;

namespace CSharpLibraries.Interpreters{
    internal sealed class Environment : Dictionary<object, object>{
        private readonly Environment _outer;

        internal Environment(object parameters, IEnumerable<object> args, Environment outer){
            if (parameters is Symbol){
                this[parameters] = args;
            }

            var p = (IEnumerable<object>) parameters;
            if (args.Count() == p.Count()){
                using var pi = p.GetEnumerator();
                using var ai = args.GetEnumerator();
                while (pi.MoveNext()){
                    this[pi.Current] = ai.Current;
                }
            }
            else{
                throw new TypeException($"expected {EvalToString(parameters)}, given {EvalToString(args)}");
            }

            _outer = outer;
        }

        private Environment(IEnumerable<DictionaryEntry> entries){
            foreach (var entry in entries){
                this[entry.Key] = entry.Value;
            }

            _outer = null;
        }

        internal Environment Find(object variable){
            if (ContainsKey(variable)){
                return this;
            }
            else if (variable == null){
                throw new LookUpException(variable.ToString());
            }
            else{
                return _outer.Find(variable);
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
                    "+", new Lambda(args => {
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
                new("-", new Lambda(args => {
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
                new("*", new Lambda(args => {
                    if (args.Count < 2) throw new ArgumentsCountException(">= 2");
                    return args.Aggregate(Multiply);
                })),
                new(
                    "/", new Lambda(args => {
                        if (args.Count < 2) throw new ArgumentsCountException("2");
                        return args.Aggregate(Divide);
                    })),
                new(">", new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    object a = args[0];
                    object b = args[1];
                    return LessThan(b, a);
                })),
                new("<", new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    object a = args[0];
                    object b = args[1];
                    return LessThan(a, b);
                })),
                new(">=", new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");

                    object a = args[0];
                    object b = args[1];
                    return LessThan(b, a) || ValueEqual(a, b);
                })),
                new("<=", new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");

                    object a = args[0];
                    object b = args[1];
                    return LessThan(a, b) || ValueEqual(a, b);
                })),
                new("=", new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    object a = args[0];
                    object b = args[1];
                    return ValueEqual(a, b);
                })),
                new("abs", new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    object res = args[0];
                    if (LessThan(res, 0)) return Negative(res);
                    else return res;
                })),
                new("append", new Lambda(args => {
                    if (args.Count < 2) throw new ArgumentsCountException(">=2");
                    IList<object> res = new List<object>((IList<object>) args[0]);
                    for (int i = 1; i < args.Count; i++){
                        res.AddRange((IList<object>) args[i]);
                    }

                    return res;
                })),
                new("apply", new Lambda(args => {
                    Lambda proc = (Lambda) args[0];
                    return proc.Apply(args.Skip(1).ToList());
                })),
                new("begin", new Lambda(args => args[^1])),
                new("car", new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return ((IList<object>) args[0])[0];
                })),
                new("cdr", new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return ((IList<object>) args[0]).Skip(1).ToList();
                })),
                new("cons", new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    var content = (IList<object>) args[1];
                    IList<object> t = new List<object>(content.Count + 1);
                    t.Add(args[0]);
                    t.AddRange(content);
                    return t;
                })),
                new("eq?", new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    return ReferenceEquals(args[0], args[1]);
                })),
                new("expt", new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    var a = Value(args[0]);
                    var b = Value(args[1]);
                    return Math.Pow(a, b);
                })),
                new("equal?", new Lambda(args => {
                    if (args.Count != 2) throw new ArgumentsCountException("2");
                    return args[0].Equals(args[1]);
                })),
                new("length", new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return ((IList<object>) args[0]).Count;
                })),
                new("list", new Lambda(args => {
                    if (args.Count < 1){
                        return new SchemeList(Nil);
                    }

                    var res = new SchemeList(args[0]);
                    var p = res;
                    for (int i = 1; i < args.Count; i++){
                        p = p.ChainAdd(args[i]);
                    }

                    return res;
                })),
                new("list?", new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return new List<object>(args);
                })),
                new("map", new Lambda(args => {
                    if (args.Count < 2) throw new ArgumentsCountException(">=2");
                    Lambda func = (Lambda) args[0];
                    var lists = args.Skip(1).ToList();
                    IList<object> r = new List<object>(((IList<object>) lists[0]).Count);
                    while (true){
                        IList<object> vals = new List<object>(lists.Count);
                        int i = 0;
                        foreach (var list in lists.Cast<IList<object>>().Where(list => !ObjectIsNil(list))){
                            vals.Add(list[0]);
                            lists[i++] = list.Skip(1).ToList();
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
                new("max", new Lambda(args => args.Max())),
                new("min", new Lambda(args => args.Min())),
                new("not", new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return !(bool) args[0];
                })),
                new("null?", new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return ObjectIsNil(args[0]);
                })),
                new("number?", new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return (args[0] is int) || (args[0] is double) || (args[0] is Complex);
                })),
                new("print", new Lambda(args => {
                    if (args.Count != 1){
                        throw new ArgumentsCountException("1");
                    }

                    Console.WriteLine(EvalToString(args[0]));

                    return null;
                })),
                new("procedure?", new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return args[0] is Procedure;
                })),
                new("round", new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    var a = Value(args[0]);
                    return Math.Round(a, MidpointRounding.AwayFromZero);
                })),
                new("symbol?", new Lambda(args => {
                    if (args.Count != 1) throw new ArgumentsCountException("1");
                    return args[0] is Symbol;
                })),
                new("pi", Math.PI),
                new("nil",
                    Nil)
            };
            return new Environment(d);
        }
    }
}