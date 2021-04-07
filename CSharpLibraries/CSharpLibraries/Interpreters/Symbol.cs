using System;
using System.Collections.Generic;

namespace CSharpLibraries.Interpreters{
    public sealed class Symbol{
        internal readonly string Value;

        internal Symbol(string s){
            this.Value = s;
        }

        public override bool Equals(object o){
            if (o is Symbol symbol){
                return Value.Equals(symbol.Value);
            }
            else{
                return false;
            }
        }

        public override int GetHashCode(){
            return HashCode.Combine(Value, typeof(Symbol));
        }

        public override string ToString(){
            return Value;
        }

        internal static readonly Symbol SymQuote = new("quote");
        internal static readonly Symbol SymIf = new("if");
        internal static readonly Symbol SymSet = new("set!");
        internal static readonly Symbol SymDefine = new("define");
        internal static readonly Symbol SymLambda = new("lambda");
        internal static readonly Symbol SymBegin = new("begin");
        internal static readonly Symbol SymDefineMacro = new("define-macro");
        internal static readonly Symbol SymQuasiQuote = new("quasi-quote");
        internal static readonly Symbol SymUnquote = new("unquote");
        internal static readonly Symbol SymUnquoteSplicing = new("unquote-splicing");
        public static readonly Symbol SymEof = new("#<symbol-eof>");

        internal static readonly Dictionary<string, Symbol> QuotesDict = new(){
            {"'", SymQuote},
            {"`", SymQuasiQuote},
            {",", SymUnquote},
            {",@", SymUnquoteSplicing}
        };

        internal static readonly Symbol SymAppend = new("append");
        internal static readonly Symbol SymCons = new("cons");
        internal static readonly Symbol SymLet = new("let");
    }
}