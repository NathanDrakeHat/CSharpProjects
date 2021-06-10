using NUnit.Framework;
using static CSharpLibraries.Leetcode.Leetcode250;

namespace CSharpLibrariesTest.Leetcode{
  public static class Leetcode250Test{
    [Test]
    public static void IsAnagramTest(){
      Assert.True(IsAnagram("anagram", "nagaram"));
    }
  }
}