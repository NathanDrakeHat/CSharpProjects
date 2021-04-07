using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using CSharpLibraries.Interpreters;
using NUnit.Framework;

namespace CSharpLibrariesTest.Interpreters{
    public static class LispTest{
        public static Lisp Interpreter = new();

        [Test]
        public static void ParseTest(){
            var res = Interpreter.Parse("(begin (define r 10) (* pi (* r r)))");
            Assert.AreEqual(new List<object>{
                new Symbol("begin"),
                new List<object>{
                    new Symbol("define"), new Symbol("r"), 10
                },
                new List<object>{
                    new Symbol("*"), new Symbol("pi"),
                    new List<object>{
                        new Symbol("*"), new Symbol("r"), new Symbol("r")
                    }
                }
            }, (IList<object>) res);
        }

        [Test]
        public static void CaseTest1(){
            var res = Interpreter.EvalScripts("(begin (define r 10) (* pi (* r r)))");
            Console.WriteLine(res);
            Assert.AreEqual(314.1592653589793, res);
        }

        [Test]
        public static void CaseTest2(){
            var res = Interpreter.EvalScripts("(if (> (* 11 11) 120) (* 7 6) oops)");
            Assert.AreEqual(42, res);
        }

        [Test]
        public static void CaseTest3(){
            var t = (IList<object>) Interpreter.EvalScripts("(list (+ 1 1) (+ 2 2) (* 2 3) (expt 2 3))");
            var e = new List<object>{2, 4, 6, 8};
            Assert.AreEqual(e, t);
        }

        [Test]
        public static void CaseTest4(){
            var res = Interpreter.EvalScripts("(begin " +
                                              "(define fact (lambda (n) (if (<= n 1) 1 (* n (fact (- n 1)))))) " +
                                              "(fact 5))");
            Assert.AreEqual(120, res);
        }

        [Test]
        public static void CaseTest5(){
            var res = Interpreter.EvalScripts("(begin " +
                                              "(define fib (lambda (n) (if (< n 2) 1 (+ (fib (- n 1)) (fib (- n 2)))))) " +
                                              "(fib 6))");
            Assert.AreEqual(13, res);
        }

        [Test]
        public static void CaseTest6(){
            Assert.AreEqual(3, Interpreter.EvalScripts("(begin " +
                                                       "(define count (lambda (item L) (if (null? L) 0 (+ (if (equal? item (car L)) 1 0) (count item (cdr L)))))) " +
                                                       "(count 0 (list 0 1 2 3 0 0)))"));
        }

        [Test]
        public static void CaseTest7(){
            var l = new List<int>(){1, 4, 9, 16};
            Assert.AreEqual(l, Interpreter.EvalScripts("(begin " +
                                                       "(define square (lambda (x) (* x x))) " +
                                                       "(define range (lambda (a b) (if (= a b) nil (cons a (range (+ a 1) b))))) " +
                                                       "(map square (range 1 5)))"));
        }

        [Test]
        public static void CaseTest8(){
            var e = new List<int>(){4, 6, 8, 10};
            Assert.AreEqual(e, Interpreter.EvalScripts("(begin " +
                                                       "(define two (lambda (a b) (+ a b 2))) " +
                                                       "(define l (list 1 2 3 4)) " +
                                                       "(map two l l))"));
        }

        [Test]
        public static void CaseTest9(){
            var l = new List<int>(){1, 2, 3, 4, 5, 6};
            var r = (IList<object>) Interpreter.EvalScripts("(append (list 1 2) (list 3 4) (list 5 6))");
            Assert.AreEqual(l, r);
        }
        
        [Test]
        public static void tailRecursionTest() {
            Assert.AreEqual(500500, Interpreter.EvalScripts("(begin (define (sum2 n acc)"+
                " (if (= n 0) "+
                " acc "+
                " (sum2 (- n 1) (+ n acc)))) (sum2 1000 0))"));
        }

        [Test]
        public static void expandTest() {
            Assert.AreEqual(1000, Interpreter.EvalScripts("(begin (define (cube x) (* x x x)) (cube 10))"));
        }

        [Test]
        public static void exceptionTest() {
            Assert.Throws(typeof(InterpretersExceptions.SyntaxException), () => Interpreter.EvalScripts("(if 1 2 3 4 5)"));
        }

        [Test]
        public static void setTest() {
            Assert.AreEqual(3,
                Interpreter.EvalScripts("(begin (define x 1) (set! x (+ x 1)) (+ x 1))"));
        }
        

        [Test]
        public static void LispyTest(){
            Interpreter.LoadLib(new FileInfo("functions.ss"));
            var t = Interpreter.EvalScripts("(quote (testing 1 (2.0) -3.14e159))");
            var tt = new List<object>{new Symbol("testing"), 1, new List<object>{2.0}, -3.14e159};
            Assert.AreEqual(tt, t);
            Assert.AreEqual(4, Interpreter.EvalScripts("(+ 2 2)"));
            Assert.AreEqual(210, Interpreter.EvalScripts("(+ (* 2 100) (* 1 10))"));
            Assert.AreEqual(2, Interpreter.EvalScripts("(if (> 6 5) (+ 1 1) (+ 2 2))"));
            Assert.AreEqual(4, Interpreter.EvalScripts("(if (< 6 5) (+ 1 1) (+ 2 2))"));
            Assert.AreEqual(3, Interpreter.EvalScripts("(begin (define x 3) x)"));
            Assert.AreEqual(6, Interpreter.EvalScripts("(begin (define x 3) (+ x x))"));
            Assert.AreEqual(3, Interpreter.EvalScripts("(begin (define x 1) (set! x (+ x 1)) (+ x 1))"));
            Assert.AreEqual(10, Interpreter.EvalScripts("((lambda (x) (+ x x)) 5)"));
            Assert.AreEqual(10, Interpreter.EvalScripts("(twice 5)"));
            Assert.AreEqual(new List<object>{ 10 }, Interpreter.EvalScripts("((compose list twice) 5)"));
            Assert.AreEqual(80, Interpreter.EvalScripts("((repeat (repeat twice)) 5)"));
            Assert.AreEqual(6, Interpreter.EvalScripts("(fact 3)"));
            Assert.AreEqual(3628800, Interpreter.EvalScripts("(fact 10)"));
            Assert.AreEqual(
                new List<object>{ 3, 0, 3 },
                Interpreter.EvalScripts("(list (abs -3) (abs 0) (abs 3)) ; => (3 0 3)"));
            Assert.AreEqual(
                new List<object>{
                    new List<object>{ 1, 5 }, new List<object>{ 2, 6 }, new List<object>{ 3, 7 }, new List<object>{4, 8}},
                Interpreter.EvalScripts("(zip (list 1 2 3 4) (list 5 6 7 8))"));
            Assert.AreEqual(new List<object>{ 1, 5, 2, 6, 3, 7, 4, 8 },
                Interpreter.EvalScripts("(riff-shuffle (list 1 2 3 4 5 6 7 8))"));
            Assert.AreEqual(new List<object>{ 1, 3, 5, 7, 2, 4, 6, 8 },
                Interpreter.EvalScripts("((repeat riff-shuffle) (list 1 2 3 4 5 6 7 8))"));
            Assert.AreEqual(new List<object>{ 1, 2, 3, 4, 5, 6, 7, 8 },
                Interpreter.EvalScripts("(riff-shuffle (riff-shuffle (riff-shuffle (list 1 2 3 4 5 6 7 8))))"));
            Assert.Throws(typeof(InterpretersExceptions.SyntaxException), () => Interpreter.EvalScripts("()"));
            Assert.Throws(typeof(InterpretersExceptions.SyntaxException), () => Interpreter.EvalScripts("(set! x)"));
            Assert.Throws(typeof(InterpretersExceptions.SyntaxException), () => Interpreter.EvalScripts("(define 3 4)"));
            Assert.Throws(typeof(InterpretersExceptions.SyntaxException), () => Interpreter.EvalScripts("(quote 1 2)"));
            Assert.Throws(typeof(InterpretersExceptions.SyntaxException), () => Interpreter.EvalScripts("(if 1 2 3 4)"));
            Assert.Throws(typeof(InterpretersExceptions.SyntaxException), () => Interpreter.EvalScripts("(lambda 3 3)"));
            Assert.Throws(typeof(InterpretersExceptions.SyntaxException), () => Interpreter.EvalScripts("(lambda (x))"));
            Assert.Throws(typeof(InterpretersExceptions.SyntaxException),
            () => Interpreter.EvalScripts("(if (= 1 2) (define-macro a 'a) \n" +
                                          "(define-macro a 'b))"));
            Assert.AreEqual(4, Interpreter.EvalScripts("(twice 2)"));
            Assert.Throws(typeof(InterpretersExceptions.TypeException), () => Interpreter.EvalScripts("(twice 2 2)"));
            Assert.AreEqual(new List<object>{ 1, 2, 3, 4 }, Interpreter.EvalScripts("(lyst 1 2 3 (+ 2 2))"));
            Assert.AreEqual(2, Interpreter.EvalScripts("(if 1 2)"));
            Assert.Null(Interpreter.EvalScripts("(if (= 3 4) 2)"));
            Assert.AreEqual(new List<object>{ 100, 110, 120 }, Interpreter.EvalScripts("(begin " +
                                                                          "(define ((account bal) amt) (set! bal (+ bal amt)) bal) " +
                                                                          "(define a1 (account 100)) (list (a1 0) (a1 10) (a1 10)))"));
            Assert.True((bool) Interpreter.EvalScripts("(> (square-root 200.) 14.14213)"));
            Assert.True((bool) Interpreter.EvalScripts("(< (square-root 200.) 14.14215)"));
            Assert.True((bool) Interpreter.EvalScripts("(= (square-root 200.) (sqrt 200.))"));
            Assert.AreEqual(9045050, Interpreter.EvalScripts("(sum-squares-range 1 300)"));
            Assert.AreEqual(1,
                Interpreter.EvalScripts("(call/cc (lambda (throw) (+ 5 (* 10 (throw 1)))))"));
            Assert.AreEqual(15,
                Interpreter.EvalScripts("(call/cc (lambda (throw) (+ 5 (* 10 1))))"));
            Assert.AreEqual(35, Interpreter.EvalScripts("(call/cc (lambda (throw) " +
                                                     "(+ 5 (* 10 (call/cc (lambda (escape) (* 100 (escape 3))))))))"));
            Assert.AreEqual(3, Interpreter.EvalScripts("(call/cc (lambda (throw) " +
                                                    "(+ 5 (* 10 (call/cc (lambda (escape) (* 100 (throw 3))))))))"));
            Assert.AreEqual(1005, Interpreter.EvalScripts("(call/cc (lambda (throw) " +
                                                       "(+ 5 (* 10 (call/cc (lambda (escape) (* 100 1)))))))"));
            Assert.AreEqual(new Complex(-1, 0), Interpreter.EvalScripts("(* 1i 1i)"));
            Assert.AreEqual(new Complex(0, 1), Interpreter.EvalScripts("(sqrt -1)"));
            Assert.AreEqual(3, Interpreter.EvalScripts("(let ((a 1) (b 2)) (+ a b))"));
            Assert.Throws(typeof(InterpretersExceptions.SyntaxException), () => Interpreter.EvalScripts(
                "(let ((a 1) (b 2 3)) (+ a b))"));
            Assert.AreEqual(3, Interpreter.EvalScripts("(and 1 2 3)"));
            Assert.AreEqual(3, Interpreter.EvalScripts("(and (> 2 1) 2 3)"));
            Assert.True((bool) Interpreter.EvalScripts("(and)"));
            Assert.False((bool) Interpreter.EvalScripts("(and (> 2 1) (> 2 3))"));
            Assert.Null(Interpreter.EvalScripts("(unless (= 2 (+ 1 1)) (display 2) 3 4)"));
            Assert.AreEqual(2, Interpreter.EvalScripts("2"));
            Assert.AreEqual(4,
                Interpreter.EvalScripts("(unless (= 4 (+ 1 1)) (display 2) (display 'n) 3 4)"));
            Assert.AreEqual(new Symbol("x"),
                Interpreter.EvalScripts("(quote x)"));
            Assert.AreEqual(new List<object>{1, 2, new Symbol("three")}, 
                Interpreter.EvalScripts("(quote (1 2 three))"));
            Assert.AreEqual(new Symbol("x"), Interpreter.EvalScripts("'x"));
            Assert.AreEqual(new List<object>{ new Symbol("one") , 2, 3}, Interpreter.EvalScripts("'(one 2 3)"));
            Assert.AreEqual(
                new List<object>{
                    new List<object>{ new Symbol("testing") , 1, 2, 3, new Symbol("testing")},
                    new List<object>{ new Symbol("testing") , new List<object>{ 1, 2, 3 }, new Symbol("testing")}
                },
                Interpreter.EvalScripts("(begin " +
                                        "(define L (list 1 2 3)) " +
                                        "(list `(testing ,@L testing) `(testing ,L testing) ) ) "));
            Assert.AreEqual(new List<object>{ 1, 2, 3 }, Interpreter.EvalScripts(""));
            /*
             * '(1 ; test comments '
                ;
            skip this line
            2;
            more;
            comments; ) )
            3) ;
            final comment; => (1 2 3)
             */
        }
    }
}