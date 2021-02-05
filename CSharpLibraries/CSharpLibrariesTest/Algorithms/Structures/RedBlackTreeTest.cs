using System;
using System.Collections.Generic;
using System.Diagnostics;
using CSharpLibraries.Algorithms.Structures;
using NUnit.Framework;

namespace CSharpLibrariesTest.Algorithms.Structures
{
    public static class RedBlackTreeTest
    {
        [Test]
        public static void InsertFixUpTest()
        {
            var rBtree = new RedBlackTree<int, int>((a, b) => a - b);
            rBtree.Insert(11, 0);
            Assert.True(rBtree.Contains(11));
            rBtree.Insert(2, 0);
            rBtree.Insert(14, 0);
            rBtree.Insert(1, 0);
            rBtree.Insert(7, 0);
            rBtree.Insert(15, 0);
            rBtree.Insert(5, 0);
            rBtree.Insert(8, 0);
            rBtree.Insert(4, 0);
            var root = rBtree.Root;
            Assert.True(root.Key == 7); // 7
            Assert.True(root.Left.Key == 2); // 2
            Assert.True(root.Right.Key == 11); // 11
            Assert.True(root.Left.Left.Key == 1); // 1
            Assert.True(root.Left.Right.Key == 5); // 5
            Assert.True(root.Left.Right.Left.Key == 4); // 4
            Assert.True(root.Right.Left.Key == 8); // 8
            Assert.True(root.Right.Right.Key == 14); // 14
            Assert.True(root.Right.Right.Right.Key == 15); // 15
        }

        [Test]
        public static void BalanceTest()
        {
            var t = new RedBlackTree<int, int>((a, b) => a - b);
            for (int i = 0; i < 127; i++) t.Insert(i, i);
            Assert.AreEqual(6,t.GetHeight());
            for (int i = 0; i < 65; i++) t.Delete(i);
            Assert.AreEqual(5,t.GetHeight());
        }

        [Test]
        public static void FunctionsTest()
        {
            var t = new RedBlackTree<int, string>((a, b) => a - b);
            Assert.Throws(typeof(InvalidOperationException), () => t.Search(1));
            var l1 = new List<int>();
            var l2 = new List<int>();
            var l3 = new LinkedList<int>();
            var l4 = new LinkedList<int>();
            for (int i = 0; i < 63; i++)
            {
                t.Insert(i, i.ToString());
                l1.Add(i);
                l3.AddFirst(i);
            }

            Assert.AreEqual(44 - 12 + 1, t.KeyRangeSearch(12, 44).Count);
            Assert.AreEqual(33 + 1 - 1, t.KeyRangeSearch(1, 33).Count);
            Assert.AreEqual(63, t.KeyRangeSearch(0, 62).Count);

            t.Set(2, "3");
            Assert.True(t.Search(2) == "3");
            Assert.True(t.GetHeight() == 5);
            Assert.True(t.Count == 63);
            Assert.True(t.MinKey() == 0);
            Assert.True(t.MaxKey() == 62);
            Assert.True(t.ValueOfMaxKey() == "62");
            Assert.True(t.ValueOfMinKey() == "0");
            foreach (var tuple in t)
            {
                l2.Add(tuple.Item1);
            }

            foreach (var tuple in t.GetReverseEnumerator())
            {
                l4.AddLast(tuple.Item1);
            }

            Assert.AreEqual(l1, l2);
            Assert.AreEqual(l3, l4);
        }
        
        public static void PerformanceTest()
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            var t = new RedBlackTree<int, int>((a, b) => a - b);
            for (int i = 0; i < 16777215; i++) t.Insert(i, i);
            Console.WriteLine(t.GetHeight());
            for (int i = 0; i < 8388609; i++) t.Delete(i);
            Console.WriteLine(t.GetHeight());
            Console.WriteLine(w.ElapsedMilliseconds / 1000.0);
        }

        private sealed class OneClass
        {
            public int Content;
        }


        [Test]
        public static void ComparerTest()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var t = new RedBlackTree<OneClass, string>((a, b) => a.Content - b.Content);
            for (int i = 0; i < 16; i++) t.Insert(new OneClass {Content = i}, i.ToString());

            Assert.True(t.GetHeight() == 4);
            Assert.True(t.Count == 16);
            Assert.True(t.MinKey().Content == 0);
            Assert.True(t.MaxKey().Content == 15);
            Assert.True(t.ValueOfMaxKey() == "15");
            Assert.True(t.ValueOfMinKey() == "0");
        }
    }
}