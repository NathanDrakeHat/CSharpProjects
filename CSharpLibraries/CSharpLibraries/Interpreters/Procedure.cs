using Lambda = System.Func<System.Collections.Generic.List<object>, object>;

namespace CSharpLibraries.Interpreters
{
    internal class Procedure
    {
        internal readonly Lambda Lambda;
        internal readonly Environment Environment;
        internal readonly object Expression;
        internal readonly object Parameters;

        internal Procedure(object parameters, object expression, Environment env)
        {
            Environment = env;
            Expression = expression;
            Parameters = parameters;
            
        }
    }
}