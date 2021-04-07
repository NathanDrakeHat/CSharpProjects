#nullable disable
namespace CSharpLibraries.Leetcode{
    public static class Leetcode400{
        /// <summary>
        /// #376
        /// <br/>最长摆动序列
        /// <br/>
        /// <code>
        /// 输入: [1,17,5,10,13,15,10,5,16,8]
        /// 输出: 7
        /// 解释: 这个序列包含几个长度为 7 摆动序列，其中一个可为[1,17,10,13,10,16,8]。
        /// 
        /// 输入: [1,2,3,4,5,6,7,8,9]
        /// 输出: 2
        /// </code>
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public static int WiggleMaxLength(int[] nums){
            if (nums.Length < 2){
                return nums.Length;
            }

            Gradient grad = Gradient.Level;
            int res = 1;

            if (nums[0] < nums[1]){
                res++;
                grad = Gradient.Ascend;
            }
            else if (nums[0] > nums[1]){
                res++;
                grad = Gradient.Descend;
            }

            for (int i = 2; i < nums.Length; i++){
                var t = GetGradient(nums[i - 1], nums[i]);
                if (t != Gradient.Level && grad != t){
                    res++;
                    grad = t;
                }
            }

            return res;

            static Gradient GetGradient(int first, int second){
                if (first == second){
                    return Gradient.Level;
                }
                else if (first < second){
                    return Gradient.Ascend;
                }
                else{
                    return Gradient.Descend;
                }
            }
        }

        private enum Gradient{
            Ascend,
            Level,
            Descend
        }
    }
}