namespace CSharpLibraries.Leetcode
{
    public static class Leetcode950
    {
        /// <summary>
        /// #922
        /// <br/>按奇偶排序数组 II
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static int[] SortArrayByParityII(int[] array)
        {
            int evenIdx = 0, oddIdx = 1;
            while(evenIdx < array.Length && oddIdx < array.Length)
            {
                if (evenIdx < array.Length && array[evenIdx] % 2 == 0)
                {
                    evenIdx += 2;
                }

                if (oddIdx < array.Length && array[oddIdx] % 2 == 1)
                {
                    oddIdx += 2;
                }

                if (evenIdx < array.Length && oddIdx < array.Length)
                {
                    int t = array[oddIdx];
                    array[oddIdx] = array[evenIdx];
                    array[evenIdx] = t;
                }
            }

            return array;
        }
        
        /// <summary>
        /// #941
        /// <br/>山脉数组
        /// <br/>A.length >= 3
        /// <br/>在 0 &lt; i &lt; A.length - 1 条件下，存在 i 使得：
        /// <br/>A[0] &lt; A[1] &lt; ... A[i-1] &lt; A[i]
        /// <br/>A[i] &gt; A[i+1] &gt; ... &gt; A[A.length - 1]
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool ValidMountainArray(int[] a) {
            if (a.Length < 3)
            {
                return false;
            }

            int length = a.Length;
            int i = 0;

            while (i + 1 < length && a[i] < a[i + 1]) {
                i++;
            }

            if (i == 0 || i == length - 1) {
                return false;
            }

            while (i + 1 < length && a[i] > a[i + 1]) {
                i++;
            }

            return i == length - 1;

        }
    }
}
