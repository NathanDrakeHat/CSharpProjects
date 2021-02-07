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
            Assert.AreEqual(314.1592653589793,RunScheme("(begin (define r 10) (* pi (* r r)))"));
            
            Assert.AreEqual(42, RunScheme("(if (> (* 11 11) 120) (* 7 6) oops)"));
            
            SchemeList t = RunScheme("(list (+ 1 1) (+ 2 2) (* 2 3) (expt 2 3))");
            SchemeList e = new SchemeList(2.0);
            e.ChainAppend(4.0).ChainAppend(6.0).ChainAppend(8.0);
            Assert.True(t.Equals(e));
            
            Assert.AreEqual(120.0, RunScheme("(begin " +
                                             "(define fact (lambda (n) (if (<= n 1) 1 (* n (fact (- n 1)))))) " +
                                             "(fact 5))"));
            
            Assert.AreEqual(13.0, RunScheme("(begin " +
                                            "(define fib " +
                                            "(lambda (n) " +
                                            "(if (< n 2) 1 (+ (fib (- n 1)) (fib (- n 2)))))) " +
                                            "(fib 6))"));
            
            Assert.AreEqual(3.0, RunScheme("(begin " +
                                           "(define count " +
                                           "(lambda (item L) " +
                                           "(if (null? L) 0 (+ (if (equal? item (car L)) 1 0) (count item (cdr L)))))) " +
                                           "(count 0 (list 0 1 2 3 0 0)))"));
            /*
            (define range (lambda (a b) (if (= a b) '() (cons a (range (+ a 1) b)))))
            (map fib (range 0 10))
             */
            
        }
        
    }
}