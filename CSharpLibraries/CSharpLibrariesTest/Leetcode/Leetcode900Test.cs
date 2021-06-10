using NUnit.Framework;
using static CSharpLibraries.Leetcode.Leetcode900;

namespace CSharpLibrariesTest.Leetcode{
  public static class Leetcode900Test{
    //5,5,10,20,5,5,5,5,5,5,5,5,5,10,5,5,20,5,20,5

    [Test]
    public static void LemonadeChangeTest(){
      Assert.True(LemonadeChange(new[]{5, 5, 10, 20, 5, 5, 5, 5, 5, 5, 5, 5, 5, 10, 5, 5, 20, 5, 20, 5}));
    }
  }
}