using NUnit.Framework;
using static CSharpLibraries.Leetcode.Leetcode400;

namespace CSharpLibrariesTest.Leetcode{
  public static class Leetcode400Test{
    [Test]
    public static void WiggleMaxLengthTest(){
      Assert.AreEqual(1, WiggleMaxLength(new[]{0, 0}));
    }
  }
}