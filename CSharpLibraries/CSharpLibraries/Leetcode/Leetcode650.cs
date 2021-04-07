using System.Collections.Generic;

namespace CSharpLibraries.Leetcode{
    public static class Leetcode650{
        /// <summary>
        /// #649
        /// </summary>
        /// <param name="senate"></param>
        /// <returns></returns>
        public static string PredictPartyVictory(string senate){
            int n = senate.Length;
            var radiant = new Queue<int>();
            var dire = new Queue<int>();
            for (int i = 0; i < n; ++i){
                if (senate[i] == 'R'){
                    radiant.Enqueue(i);
                }
                else{
                    dire.Enqueue(i);
                }
            }

            while (radiant.Count != 0 && dire.Count != 0){
                if (radiant.Peek() < dire.Peek()){
                    radiant.Enqueue(radiant.Peek() + n);
                }
                else{
                    dire.Enqueue(dire.Peek() + n);
                }

                radiant.Dequeue();
                dire.Dequeue();
            }

            return radiant.Count != 0 ? "Radiant" : "Dire";
        }
    }
}