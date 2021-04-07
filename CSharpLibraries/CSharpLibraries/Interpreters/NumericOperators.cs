﻿using System;
using System.Numerics;
using static CSharpLibraries.Interpreters.InterpretersExceptions;

namespace CSharpLibraries.Interpreters{
    internal static class NumericOperators{
        internal static double Value(object o){
            return o switch{
                double d => d,
                int i => i,
                _ => throw new SyntaxException("not number")
            };
        }

        internal static bool LessThan(object a, object b){
            return Value(a) < Value(b);
        }

        internal static bool ValueEqual(object a, object b){
            return Math.Abs(Value(a) - Value(b)) < double.Epsilon;
        }

        internal static object Negative(object a){
            return a switch{
                int i => -i,
                double d => -d,
                Complex c => new Complex(-c.Real, -c.Imaginary),
                _ => throw new SyntaxException("not number")
            };
        }

        internal static object Plus(object a, object b){
            return a switch{
                int c when b is int e => c + e,
                int c when b is double d => c + d,
                int c when b is Complex cx => Complex.Add(cx, c),
                int => throw new InvalidCastException(),
                double e when b is int f => e + f,
                double e when b is double d => e + d,
                double e when b is Complex cx => Complex.Add(cx, e),
                double => throw new InvalidCastException(),
                Complex cx when b is int e => Complex.Add(cx, e),
                Complex cx when b is double d => Complex.Add(cx, d),
                Complex cx when b is Complex cx1 => Complex.Add(cx, cx1),
                _ => throw new InvalidCastException()
            };
        }

        internal static object Minus(object a, object b){
            return a switch{
                int c when b is int e => c - e,
                int c when b is double d => c - d,
                int c when b is Complex cx => Complex.Add(c, Complex.Negate(cx)),
                int => throw new InvalidCastException(),
                double e when b is int f => e - f,
                double e when b is double d => e - d,
                double e when b is Complex cx => Complex.Add(e, Complex.Negate(cx)),
                double => throw new InvalidCastException(),
                Complex cx when b is int e => Complex.Negate(Complex.Add(Complex.Negate(cx), e)),
                Complex cx when b is double d => Complex.Negate(Complex.Add(Complex.Negate(cx), d)),
                Complex cx when b is Complex cx1 => Complex.Negate(Complex.Add(Complex.Negate(cx), cx1)),
                _ => throw new InvalidCastException()
            };
        }

        internal static object Divide(object a, object b){
            return a switch{
                int c when b is int e => c / e,
                int c when b is double d => c / d,
                int c when b is Complex cx => Complex.Divide(c, cx),
                int => throw new InvalidCastException(),
                double e when b is int f => e / f,
                double e when b is double d => e / d,
                double e when b is Complex cx => Complex.Divide(e, cx),
                double => throw new InvalidCastException(),
                Complex cx when b is int e => Complex.Divide(cx, e),
                Complex cx when b is double d => Complex.Divide(cx, d),
                Complex cx when b is Complex cx1 => Complex.Divide(cx, cx1),
                _ => throw new InvalidCastException()
            };
        }

        internal static object Multiply(object a, object b){
            return a switch{
                int c when b is int e => c * e,
                int c when b is double d => c * d,
                int c when b is Complex cx => Complex.Multiply(c, cx),
                int => throw new InvalidCastException(),
                double e when b is int f => e * f,
                double e when b is double d => e * d,
                double e when b is Complex cx => Complex.Multiply(e, cx),
                double => throw new InvalidCastException(),
                Complex cx when b is int e => Complex.Multiply(cx, e),
                Complex cx when b is double d => Complex.Multiply(cx, d),
                Complex cx when b is Complex cx1 => Complex.Multiply(cx, cx1),
                _ => throw new InvalidCastException()
            };
        }
    }
}