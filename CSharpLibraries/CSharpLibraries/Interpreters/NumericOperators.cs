using System;

namespace CSharpLibraries.Interpreters
{
    internal static class NumericOperators
    {
        
        internal static double Value(object o)
        {
            return o switch
            {
                double d => d,
                int i => i,
                _ => throw new InvalidCastException()
            };
        }
        
        internal static bool LessThan(object a, object b)
        {
            return Value(a) < Value(b);
        }

        internal static bool LessOrEqual(object a, object b)
        {
            return Value(a) <= Value(b);
        }

        internal static bool Equal(object a, object b)
        {
            return Math.Abs(Value(a) - Value(b)) < double.Epsilon;
        }

        internal static object Negative(object a)
        {
            return a switch
            {
                int i => -i,
                double d => -d,
                _ => throw new InvalidCastException()
            };
        }

        internal static object Plus(object a, object b)
        {
            return a switch
            {
                int c when b is int e => c + e,
                int c when b is double d => c + d,
                int => throw new InvalidCastException(),
                double e when b is int f => e + f,
                double e when b is double d => e + d,
                double => throw new InvalidCastException(),
                _ => throw new InvalidCastException()
            };
        }

        internal static object Minus(object a, object b)
        {
            return a switch
            {
                int c when b is int f => c - f,
                int c when b is double d => c - d,
                int => throw new InvalidCastException(),
                double e when b is int f => e - f,
                double e when b is double d => e - d,
                double => throw new InvalidCastException(),
                _ => throw new InvalidCastException()
            };
        }

        internal static object Divide(object a, object b)
        {
            return a switch
            {
                int c when b is int e => c / e,
                int c when b is double d => c / d,
                int => throw new InvalidCastException(),
                double e when b is int f => e / f,
                double e when b is double d => e / d,
                double => throw new InvalidCastException(),
                _ => throw new InvalidCastException()
            };
        }

        internal static object Multiply(object a, object b)
        {
            return a switch
            {
                int c when b is int e => c * e,
                int c when b is double d => c * d,
                int => throw new InvalidCastException(),
                double e when b is int f => e * f,
                double e when b is double d => e * d,
                double => throw new InvalidCastException(),
                _ => throw new InvalidCastException()
            };
        }
    }
}