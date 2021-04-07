using System;
using System.Collections.Generic;

namespace CSharpLibraries.Algorithms.Miscellaneous{
    /// <summary>
    /// get an element according its rank without sorting
    /// </summary>
    public static class RankSearch{
        public static T RandomSelect<T>(IList<T> a, int ith) where T : IComparable<T>{
            return Select(a, 0, a.Count, ith);

            #region Inner

            static T Select(IList<T> a, int start, int end, int ith){
                while (true){
                    if (start - end == 1){
                        return a[start];
                    }

                    int pivotIdx = Sort.RandomPartition(a, start, end);
                    int leftTotal = pivotIdx - start;
                    if (ith == leftTotal){
                        return a[pivotIdx];
                    }
                    else if (ith < leftTotal + 1){
                        end = pivotIdx;
                    }
                    else{
                        start = pivotIdx + 1;
                        ith = ith - leftTotal - 1;
                    }
                }
            }

            #endregion
        }
    }
}