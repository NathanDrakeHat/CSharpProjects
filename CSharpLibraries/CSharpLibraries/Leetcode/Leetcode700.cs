
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
namespace CSharpLibraries.Leetcode
{
    public sealed class DpMatrix<TAny>
    {
        private readonly TAny[][] _matrix;

        public DpMatrix(int size)
        {
            _matrix = new TAny[size][];
            for (int i = 0; i < size; i++)
                _matrix[i] = new TAny[size - i];
        }

        public TAny this[int r, int c]
        {
            get => _matrix[r][c - r];
            set => _matrix[r][c - r] = value;
        }
    }

    public static class Leetcode700
    {
        /// <summary>
        /// #674
        /// <br/>回文子串个数
        /// <br/>输入："aaa"
        /// <br/>输出：6
        /// <br/>解释：6个回文子串: "a", "a", "a", "aa", "aa", "aaa"
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns>回文子串个数</returns>
        public static int CountSubstrings(string s)
        {
            DpMatrix<bool> store = new DpMatrix<bool>(s.Length);
            int res = s.Length;
            // two
            for (int i = 0; i + 2 - 1 < s.Length; i++)
            {
                if (s[i] == s[i + 1])
                {
                    store[i, i] = true;
                    res++;
                }
            }

            //three
            for (int i = 0; i + 3 - 1 < s.Length; i++)
            {
                if (s[i] == s[i + 2])
                {
                    store[i, i + 1] = true;
                    res++;
                }
            }

            // other
            for (int l = 4; l <= s.Length; l++)
            {
                for (int i = 0; i + l - 1 < s.Length; i++)
                {
                    if (store[i + 1, i + l - 3])
                    {
                        if (s[i] == s[i + l - 1])
                        {
                            store[i, i + l - 2] = true;
                            res++;
                        }
                    }
                }
            }

            return res;
        }
    }
}