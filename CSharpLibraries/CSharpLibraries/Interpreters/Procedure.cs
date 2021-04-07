using System;
using System.Collections.Generic;

namespace CSharpLibraries.Interpreters{
    internal class Procedure : Lambda{
        internal readonly Environment Environment;
        internal readonly object Expression;
        internal readonly object Parameters;

        internal Procedure(object parameters, object expression, Environment env)
            : base(args => Lisp.Eval(expression, new Environment(parameters, args, env))){
            Environment = env;
            Expression = expression;
            Parameters = parameters;
        }
    }

    internal class Lambda{
        private readonly Func<IList<object>, object> _func;

        internal Lambda(Func<IList<object>, object> func){
            _func = func;
        }

        internal object Apply(IList<object> args){
            return _func(args);
        }
    }
}