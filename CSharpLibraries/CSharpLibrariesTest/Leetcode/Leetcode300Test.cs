using NUnit.Framework;
using static CSharpLibraries.Leetcode.Leetcode300;

namespace CSharpLibrariesTest.Leetcode{
    public static class Leetcode300Test{
        [Test]
        public static void WordPatternTest(){
            Assert.IsFalse(WordPattern("abba", "dog dog dog dog"));
            Assert.IsFalse(WordPattern("abba", "dog cat cat fish"));
            Assert.IsFalse(WordPattern("aaa", "aa aa aa aa"));
        }
    }
}