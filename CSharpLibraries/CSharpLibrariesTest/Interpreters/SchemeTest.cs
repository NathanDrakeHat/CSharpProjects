﻿using System.Collections.Generic;
using CSharpLibraries.Interpreters;
using NUnit.Framework;
using static CSharpLibraries.Interpreters.LispInterpreter;

namespace CSharpLibrariesTest.Interpreters
{
    public static class SchemeTest
    {

        private static readonly LispInterpreter Interpreter = new();
        
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
            },Interpreter.Parse("(begin (define r 10) (* pi (* r r)))"));
        }
        
        [Test]
        public static void CaseTest1()
        {
            var res = Interpreter.RunScheme("(begin (define r 10) (* pi (* r r)))");
            Assert.AreEqual(314.1592653589793,res);
        }

        [Test]
        public static void CaseTest2()
        {
            var res = Interpreter.RunScheme("(if (> (* 11 11) 120) (* 7 6) oops)");
            Assert.AreEqual(42, res);
        }

        [Test]
        public static void CaseTest3()
        {
            SchemeList t = (SchemeList)Interpreter.RunScheme("(list (+ 1 1) (+ 2 2) (* 2 3) (expt 2 3))");
            SchemeList e = new SchemeList(2);
            e.ChainAdd(4).ChainAdd(6).ChainAdd(8);
            Assert.AreEqual(e,t);
        }

        [Test]
        public static void CaseTest4()
        {
            var res = Interpreter.RunScheme("(begin " +
                                "(define fact (lambda (n) (if (<= n 1) 1 (* n (fact (- n 1)))))) " +
                                "(fact 5))");
            Assert.AreEqual(120, res);
        }

        [Test]
        public static void CaseTest5()
        {
            var res = Interpreter.RunScheme("(begin " +
                                "(define fib (lambda (n) (if (< n 2) 1 (+ (fib (- n 1)) (fib (- n 2)))))) " +
                                "(fib 6))");
            Assert.AreEqual(13, res);
        }

        [Test]
        public static void CaseTest6()
        {
            Assert.AreEqual(3, Interpreter.RunScheme("(begin " +
                                           "(define count (lambda (item L) (if (null? L) 0 (+ (if (equal? item (car L)) 1 0) (count item (cdr L)))))) " +
                                           "(count 0 (list 0 1 2 3 0 0)))"));
        }

        [Test]
        public static void CaseTest7()
        {
            SchemeList l = new SchemeList(1);
            l.ChainAdd(4).ChainAdd(9).ChainAdd(16);
            Assert.AreEqual(l, Interpreter.RunScheme("(begin " +
                                         "(define square (lambda (x) (* x x))) " +
                                         "(define range (lambda (a b) (if (= a b) nil (cons a (range (+ a 1) b))))) " +
                                         "(map square (range 1 5)))"));
        }

        [Test]
        public static void CaseTest8()
        {
            SchemeList e = new SchemeList(4);
            e.ChainAdd(6).ChainAdd(8).ChainAdd(10);
            Assert.AreEqual(e,Interpreter.RunScheme("(begin " +
                                        "(define two (lambda (a b) (+ a b 2))) " +
                                        "(define l (list 1 2 3 4)) " +
                                        "(map two l l))"));
        }

        [Test]
        public static void CaseTest9()
        {
            var l = new SchemeList(1);
            l.ChainAdd(2).ChainAdd(3).ChainAdd(4).ChainAdd(5).ChainAdd(6);
            SchemeList r = (SchemeList)Interpreter.RunScheme("(append (list 1 2) (list 3 4) (list 5 6))");
            Assert.AreEqual(l, r);
        }
    }
}