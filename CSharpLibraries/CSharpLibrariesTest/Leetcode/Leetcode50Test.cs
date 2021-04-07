using NUnit.Framework;
using static CSharpLibraries.Leetcode.Leetcode50;

namespace CSharpLibrariesTest.Leetcode{
    public static class Leetcode50Test{
        [Test]
        public static void FirstMissingPositiveTest(){
            Assert.AreEqual(2, FirstMissingPositive(new[]{3, 4, -1, 1}));
            Assert.AreEqual(1, FirstMissingPositive(new[]{7, 8, 9, 11, 12}));
            Assert.AreEqual(2, FirstMissingPositive(new[]{1, 1}));
        }
    }
}