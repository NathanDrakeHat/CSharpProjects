using NUnit.Framework;
using static CSharpLibraries.Leetcode.Leetcode950;

namespace CSharpLibrariesTest.Leetcode
{
    public static class Leetcode950Test
    {
        [Test]
        public static void ValidMountainArrayTest()
        {
            Assert.True(ValidMountainArray(new int[]{0,3,2,1}));
        }
    }
}
