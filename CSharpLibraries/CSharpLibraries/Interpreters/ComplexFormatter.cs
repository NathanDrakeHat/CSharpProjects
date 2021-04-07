using System;
using System.Globalization;
using System.Numerics;

namespace CSharpLibraries.Interpreters
{
    public class ComplexFormatter :IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        public string Format(string format, object arg,
            IFormatProvider provider)
        {
            if (arg is Complex c1)
            {
                
                if (format.Equals("I"))
                    return c1.Real + " + " + c1.Imaginary + "i";
                else if (format.Equals("J"))
                    return c1.Real + " + " + c1.Imaginary + "j";
                else
                    return c1.ToString(format, provider);
            }
            else
            {
                if (arg is IFormattable formattable)
                    return formattable.ToString(format, provider);
                else if (arg != null)
                    return arg.ToString();
                else
                    return String.Empty;
            }
        }
    }
}