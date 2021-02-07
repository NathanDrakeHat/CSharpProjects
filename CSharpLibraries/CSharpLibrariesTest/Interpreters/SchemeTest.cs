using System;
using System.Collections.Generic;
using NUnit.Framework;
using static CSharpLibraries.Interpreters.Scheme;

namespace CSharpLibrariesTest.Interpreters
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
            Assert.AreEqual(42, Eval(Parse("(if (> (* 11 11) 120) (* 7 6) oops)")));
            SchemeList t = Eval(Parse("(list (+ 1 1) (+ 2 2) (* 2 3) (expt 2 3))"));
            SchemeList e = new SchemeList(2.0);
            e.ChainAppend(4.0).ChainAppend(6.0).ChainAppend(8.0);
            Assert.True(t.Equals(e));
        }
        
    }
}