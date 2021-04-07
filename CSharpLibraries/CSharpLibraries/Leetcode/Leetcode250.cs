using System.Collections.Generic;
using System.Linq;

namespace CSharpLibraries.Leetcode{
    public static class Leetcode250{
        /// <summary>
        /// #217
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public static bool ContainsDuplicate(int[] nums){
            HashSet<int> t = new HashSet<int>();
            foreach (int num in nums){
                if (t.Contains(num)){
                    return true;
                }
                else{
                    t.Add(num);
                }
            }

            return false;
        }

        /// <summary>
        /// #242
        /// <br/>有效的字母异位词
        /// <br/>输入: s = "anagram", t = "nagaram"
        /// <br/>输出: true
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsAnagram(string s, string t){
            char[] s1 = new char[26];
            char[] s2 = new char[26];
            foreach (char c in s){
                s1[c - 'a']++;
            }

            foreach (char c in t){
                s2[c - 'a']++;
            }

            return s1.SequenceEqual(s2);
        }
    }
}