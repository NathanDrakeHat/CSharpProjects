using System;
using CSharpLibraries.Algorithms.Structures;
using NUnit.Framework;

namespace CSharpLibrariesTest.Algorithms.Structures
{
    public static class VebTreeTest
    {
        private static bool Has(VebTree tree, int[] a)
        {
            foreach (var i in a)
            {
                if (!tree.HasMember(i))
                {
                    return false;
                }
            }

            return true;
        }

        [Test]
        public static void TestCase1()
        {
            var tree = new VebTree(4);
            Assert.Throws(typeof(InvalidOperationException), () => tree.ForceGetMaximum());
            Assert.Throws(typeof(InvalidOperationException), () => tree.ForceGetMinimum());
            tree.SafeInsert(1).SafeInsert(9).SafeInsert(5).SafeInsert(3).SafeInsert(15);
            tree.SafeInsert(5).SafeInsert(3).SafeInsert(15).SafeInsert(1);
            Assert.Throws(typeof(InvalidOperationException), () => tree.ForceGetSuccessor(15));
            Assert.Throws(typeof(InvalidOperationException), () => tree.ForceGetSuccessor(15));
            Assert.Throws(typeof(InvalidOperationException), () => tree.ForceGetPredecessor(1));
            Assert.Throws(typeof(InvalidOperationException), () => tree.ForceGetPredecessor(1));
            Assert.AreEqual(1, tree.ForceGetPredecessor(3));
            Assert.AreEqual(15, tree.ForceGetSuccessor(9));
            Assert.AreEqual(3, tree.ForceGetPredecessor(5));
            Assert.True(Has(tree, new[] {1, 3, 5, 9, 15}));
            Assert.False(Has(tree, new[] {2, 4, 6, 7, 8, 10, 11, 12, 13, 14}));
            Assert.AreEqual(1, tree.ForceGetMinimum());
            Assert.AreEqual(15, tree.ForceGetMaximum());
            tree.SafeDelete(1).SafeDelete(5).SafeDelete(15);
            Assert.False(tree.HasMember(1));
            Assert.True(Has(tree, new[] {3, 9}));
            Assert.AreEqual(3, tree.ForceGetMinimum());
            Assert.AreEqual(9, tree.ForceGetMaximum());
            Assert.False(Has(tree, new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}));
            Assert.AreEqual(9, tree.ForceGetSuccessor(3));
            Assert.AreEqual(3, tree.ForceGetPredecessor(9));
        }
    }
}