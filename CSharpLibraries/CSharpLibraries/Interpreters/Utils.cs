#nullable disable
using System;
using System.Collections.Generic;
using System.Numerics;
using static CSharpLibraries.Interpreters.InterpretersExceptions;

namespace CSharpLibraries.Interpreters{
  internal static class Utils{
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
      if (a is int ia){
        if (b is int ib){
          return ia + ib;
        }
        else if (b is double db){
          return ia + db;
        }
        else if (b is Complex cb){
          return ia + cb;
        }

        throw new SyntaxException("not number");
      }
      else if (a is double da){
        if (b is int ib){
          return da + ib;
        }
        else if (b is double db){
          return da + db;
        }
        else if (b is Complex cb){
          return da + cb;
        }

        throw new SyntaxException("not number");
      }
      else if (a is Complex ca){
        if (b is int ib){
          return ca + ib;
        }
        else if (b is double db){
          return ca + db;
        }
        else if (b is Complex cb){
          return ca + cb;
        }

        throw new SyntaxException("not number");
      }

      throw new SyntaxException("not number");
    }

    internal static object Minus(object a, object b){
      if (a is int ia){
        if (b is int ib){
          return ia - ib;
        }
        else if (b is double db){
          return ia - db;
        }
        else if (b is Complex cb){
          return ia - cb;
        }

        throw new SyntaxException("not number");
      }
      else if (a is double da){
        if (b is int ib){
          return da - ib;
        }
        else if (b is double db){
          return da - db;
        }
        else if (b is Complex cb){
          return da - cb;
        }

        throw new SyntaxException("not number");
      }
      else if (a is Complex ca){
        if (b is int ib){
          return ca - ib;
        }
        else if (b is double db){
          return ca - db;
        }
        else if (b is Complex cb){
          return ca - cb;
        }

        throw new SyntaxException("not number");
      }

      throw new SyntaxException("not number");
    }

    internal static object Divide(object a, object b){
      if (a is int ia){
        if (b is int ib){
          return ia / ib;
        }
        else if (b is double db){
          return ia / db;
        }
        else if (b is Complex cb){
          return ia / cb;
        }

        throw new SyntaxException("not number");
      }
      else if (a is double da){
        if (b is int ib){
          return da / ib;
        }
        else if (b is double db){
          return da / db;
        }
        else if (b is Complex cb){
          return da / cb;
        }

        throw new SyntaxException("not number");
      }
      else if (a is Complex ca){
        if (b is int ib){
          return ca / ib;
        }
        else if (b is double db){
          return ca / db;
        }
        else if (b is Complex cb){
          return ca / cb;
        }

        throw new SyntaxException("not number");
      }

      throw new SyntaxException("not number");
    }

    internal static object Multiply(object a, object b){
      if (a is int ia){
        if (b is int ib){
          return ia * ib;
        }
        else if (b is double db){
          return ia * db;
        }
        else if (b is Complex cb){
          return ia * cb;
        }

        throw new SyntaxException("not number");
      }
      else if (a is double da){
        if (b is int ib){
          return da * ib;
        }
        else if (b is double db){
          return da * db;
        }
        else if (b is Complex cb){
          return da * cb;
        }

        throw new SyntaxException("not number");
      }
      else if (a is Complex ca){
        if (b is int ib){
          return ca * ib;
        }
        else if (b is double db){
          return ca * db;
        }
        else if (b is Complex cb){
          return ca * cb;
        }

        throw new SyntaxException("not number");
      }

      throw new SyntaxException("not number");
    }

    internal static bool TryParseImaginary(string s, out Complex? c){
      if (s.Length <= 1){
        c = null;
        return false;
      }
      else if (!s.EndsWith("i")){
        c = null;
        return false;
      }
      else if (!char.IsDigit(s[^2])){
        c = null;
        return false;
      }
      else{
        var img = s.Substring(0, s.Length - 1);
        if (double.TryParse(img, out double dImg)){
          c = new Complex(0, dImg);
          return true;
        }
        else{
          throw new SyntaxException("unknown parse error");
        }
      }
    }

    internal static bool ContainsDigit(this string s){
      foreach (char c in s){
        if (char.IsDigit(c)){
          return true;
        }
      }

      return false;
    }

    internal static void AddRange(this IList<object> l, IEnumerable<object> other){
      if (l is List<object> ll){
        ll.AddRange(other);
      }
      else{
        foreach (var o in other){
          l.Add(o);
        }
      }
    }
  }
}