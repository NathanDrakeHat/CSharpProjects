using NUnit.Framework;
using static CSharpLibraries.Leetcode.Leetcode350;

namespace CSharpLibrariesTest.Leetcode
{
    public static class Leetcode350Test
    {
        [Test]
        public static void IntersectionTest()
        {
            Assert.AreEqual(new int[]{2}, Intersection(new int[]{1,2,2,1}, new int[]{2,2}));
        }
    }
}
