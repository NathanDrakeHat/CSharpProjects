using System.Collections.Generic;
using NUnit.Framework;
using static CSharpLibraries.Scheme.Scheme;

namespace CSharpLibrariesTest.Scheme
{
    public static class SchemeTest
    {
        [Test]
        public static void ParseTest()
        {
            Assert.AreEqual(new List<object>
            {
                "begin",
                new List<object>
                {
                    "define", "r", 10
                },
                new List<object>
                {
                    "*", "pi",
                    new List<object>
                    {
                        "*", "r", "r"
                    }
                }
            },Parse("(begin (define r 10) (* pi (* r r)))"));
        }
        
        [Test]
        public static void EvalTest()
        {
            Assert.AreEqual(314.1592653589793,Eval(Parse("(begin (define r 10) (* pi (* r r)))")));
        }
    }
}