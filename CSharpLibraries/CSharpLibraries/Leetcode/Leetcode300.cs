using System.Collections.Generic;

namespace CSharpLibraries.Leetcode{
  public static class Leetcode300{
    /// <summary>
    /// #290
    /// <br/>单词规律
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool WordPattern(string pattern, string s){
      var words = s.Split(" ");
      if (words.Length != pattern.Length){
        return false;
      }

      var charToString = new Dictionary<char, string>();
      var stringSet = new HashSet<string>();
      for (int i = 0; i < pattern.Length; i++){
        if (!charToString.ContainsKey(pattern[i])){
          if (stringSet.Contains(words[i])){
            return false;
          }
          else{
            charToString[pattern[i]] = words[i];
            stringSet.Add(words[i]);
          }
        }
        else{
          if (words[i] != charToString[pattern[i]]){
            return false;
          }
        }
      }

      return true;
    }
  }
}