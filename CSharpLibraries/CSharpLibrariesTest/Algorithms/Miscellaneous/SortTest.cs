using System;
using System.Collections.Generic;
using System.Linq;
using CSharpLibraries.Extensions;
using NUnit.Framework;
using static CSharpLibraries.Algorithms.Miscellaneous.Sort;
using static CSharpLibraries.Extensions.Extension;

namespace CSharpLibrariesTest.Algorithms.Miscellaneous
{
    public static class SortTest
    {
        private static bool IsAscending<T>(IList<T> l) where T : IComparable<T>
        {
            return new Enumeration(0, l.Count - 1).AsParallel().All(i => l[i].CompareTo(l[i + 1]) <= 0);
        }

        private static bool IsAscending(IList<RawDate> l)
        {
            return new Enumeration(0, l.Count - 1).AsParallel().All(i =>
            {
                var former = l[i];
                var later = l[i + 1];
                return (former.Year <= later.Year) && (former.Year != later.Year || former.Month <= later.Month) && (former.Year != later.Year || former.Month != later.Month || former.Day <= later.Day);
            });
        }
        
        [Test]
        public static void RecursiveMergeSortTest()
        {
            foreach (var _ in new Enumeration(1, 10))
            {
                Assert.IsTrue(IsAscending(ShuffledArithmeticSequence(1,64,2).RecursiveMergeSort()));
            }
        }

        [Test]
        public static void IterativeMergeSortTest()
        {
            foreach (var _ in new Enumeration(1, 10))
            {
                Assert.IsTrue(IsAscending(ShuffledArithmeticSequence(1, 64, 2).IterativeMergeSort()));
            }
        }

        [Test]
        public static void QuickSortTest()
        {
            foreach (var _ in new Enumeration(1, 10))
            {
                Assert.IsTrue(IsAscending(ShuffledArithmeticSequence(1, 64, 2).QuickSort()));
            }
        }

        [Test]
        public static void RandomQuickSortTest()
        {
            foreach (var _ in new Enumeration(1, 10))
            {
                Assert.IsTrue(IsAscending(ShuffledArithmeticSequence(1, 64, 2).RandomQuickSort()));
            }
        }

        [Test]
        public static void HeapSortTest()
        {
            foreach (var _ in new Enumeration(1, 10))
            {
                Assert.IsTrue(IsAscending(ShuffledArithmeticSequence(1, 64, 2).HeapSort()));
            }
        }

        [Test]
        public static void CountingSortTest()
        {
            foreach (var _ in new Enumeration(1, 10))
            {
                Assert.IsTrue(IsAscending(ShuffledArithmeticSequence(1, 64, 2).CountingSort()));
            }
        }

        [Test]
        public static void BucketSortTest()
        {
            foreach (var _ in new Enumeration(1, 10))
            {
                Assert.IsTrue(IsAscending(RandomDoubleList(32).BucketSort()));
            }
        }

        private static IList<double> RandomDoubleList(int len)
        {
            var rand = new Random();
            return new Enumeration(0, len).Select(_ => rand.NextDouble()).ToList();
        }

        [Test]
        public static void RadixSortTest()
        {
            foreach (var _ in new Enumeration(1, 10))
            {
                Assert.IsTrue(IsAscending(RandomRawDateArray(32).RadixSort()));
            }
        }

        private static RawDate[] RandomRawDateArray(int len)
        {
            var rand = new Random();
            var a = new RawDate[len];
            foreach (var i in new Enumeration(0, len))
            {
                a[i] = new RawDate(rand.Next(2000, 2022), rand.Next(1, 13), rand.Next(1, 29));
            }

            return a;
        }
    }
}