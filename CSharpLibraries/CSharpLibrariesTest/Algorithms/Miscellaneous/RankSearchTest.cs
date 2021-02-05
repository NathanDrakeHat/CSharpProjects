using System.Collections.Generic;
using CSharpLibraries.Algorithms.Miscellaneous;
using NUnit.Framework;
using static CSharpLibraries.Extensions.Extension;
using static CSharpLibraries.Algorithms.Miscellaneous.RankSearch;

namespace CSharpLibrariesTest.Algorithms.Miscellaneous
{
    public static class RankSearchTest
    {
        [Test]
        public static void RandomTest()
        {
            foreach(var _ in new Enumeration(1,10))
            {
                var l = ShuffledArithmeticSequence(0, 32, 1);
                List<int> ll = new List<int>(l);
                ll.Sort();
                Assert.AreEqual(ll[0], RandomSelect(l,0));
                Assert.AreEqual(ll[19],RandomSelect(l,19));
            }
        }
    }
}