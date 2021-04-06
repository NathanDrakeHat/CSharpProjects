using System;
using System.Collections.Generic;

namespace CSharpLibraries.Interpreters
{
    internal sealed class Symbol
    {
        public readonly string Value;

        public Symbol(string s)
        {
            this.Value = s;
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }

            if (o is Symbol symbol)
            {
                return Value.Equals(symbol.Value);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, typeof(Symbol));
        }

        public override string ToString()
        {
            return Value;
        }

        internal static readonly Symbol SymQuote = new Symbol("quote");
        internal static readonly Symbol SymIf = new Symbol("if");
        internal static readonly Symbol SymSet = new Symbol("set!");
        internal static readonly Symbol SymDefine = new Symbol("define");
        internal static readonly Symbol SymLambda = new Symbol("lambda");
        internal static readonly Symbol SymBegin = new Symbol("begin");
        internal static readonly Symbol SymDefineMacro = new Symbol("define-macro");
        internal static readonly Symbol SymQuasiQuote = new Symbol("quasi-quote");
        internal static readonly Symbol SymUnquote = new Symbol("unquote");
        internal static readonly Symbol SymUnquoteSplicing = new Symbol("unquote-splicing");
        internal static readonly Symbol SymEof = new Symbol("#<symbol-eof>");

        internal static readonly Dictionary<string, Symbol> QuotesDict = new()
        {
            {"'", SymQuote},
            {"`", SymQuasiQuote},
            {",", SymUnquote},
            {",@", SymUnquoteSplicing}
        };

        internal static readonly Symbol SymbolAppend = new Symbol("append");
        internal static readonly Symbol SymCons = new Symbol("cons");
        internal static readonly Symbol SymLet = new Symbol("let");
    }
}