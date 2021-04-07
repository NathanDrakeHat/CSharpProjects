namespace CSharpLibraries.Leetcode{
    public static class Leetcode900{
        /// <summary>
        /// #860
        /// </summary>
        /// <param name="bills"></param>
        /// <returns></returns>
        public static bool LemonadeChange(int[] bills){
            int[] drawer = new int[2];

            foreach (int money in bills){
                switch (money){
                    case 5:
                        drawer[0]++;
                        break;
                    case 10:
                        if (drawer[0] <= 0){
                            return false;
                        }
                        else{
                            drawer[0]--;
                            drawer[1]++;
                        }

                        break;
                    case 20:
                        if (drawer[1] <= 0){
                            if (drawer[0] < 3){
                                return false;
                            }
                            else{
                                drawer[0] -= 3;
                            }
                        }
                        else{
                            if (drawer[0] <= 0){
                                return false;
                            }
                            else{
                                drawer[1]--;
                                drawer[0]--;
                            }
                        }

                        break;
                }
            }

            return true;
        }
    }
}