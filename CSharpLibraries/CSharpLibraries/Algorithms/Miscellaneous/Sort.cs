#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpLibraries.Algorithms.Miscellaneous
{
    public static class Sort
    {
        public sealed class RawDate
        {
            public readonly int Year;
            public readonly int Month;
            public readonly int Day;

            public RawDate(int y, int m, int d)
            {
                Year = y;
                Month = m;
                Day = d;
            }
        }

        public static void IterativeMergeSort<T>(this IList<T> list)
            where T : IComparable<T>, new()
        {
            if (list == null) throw new ArgumentNullException();
            if (list.Count <= 1) return;


            int expTimes = (int) Math.Floor(Math.Log(list.Count) / Math.Log(2));
            int groupSize = 2;
            int lastRestLen = 0;
            bool notExpOf2 = (((int) Math.Pow(2, expTimes)) != list.Count);
            if (notExpOf2)
            {
                lastRestLen = list.Count % 2 == 0 ? 2 : 1;
            }

            for (int i = 0; i < expTimes; i++)
            {
                int groupIterationTimes = list.Count / groupSize;
                var cache1 = new T[groupSize / 2];
                var cache2 = new T[groupSize / 2];
                for (int j = 0; j < groupIterationTimes; j++)
                    Merge(list, j * groupSize, cache1, cache2);


                int currentRestLen = list.Count - groupIterationTimes * groupSize;
                if (currentRestLen > lastRestLen)
                {
                    var restCache1 = new T[currentRestLen - lastRestLen];
                    var restCache2 = new T[lastRestLen];
                    Merge(list, list.Count - currentRestLen, restCache1, restCache2);
                    lastRestLen = currentRestLen;
                }

                groupSize *= 2;
            }

            if (notExpOf2)
            {
                var restCache1 = new T[groupSize / 2];
                var restCache2 = new T[list.Count - groupSize / 2];
                Merge(list, 0, restCache1, restCache2);
            }
        }

        public static void RecursiveMergeSort<T>(this IList<T> list) where T : IComparable<T>, new()
        {
            Sort(list, 0, list.Count);

            #region InnerMethods

            static void Sort(IList<T> array, int start, int end)
            {
                if ((end - start) > 1)
                {
                    int middle = (start + end) / 2;
                    Sort(array, start, middle);
                    Sort(array, middle, end);
                    int leftLen = middle - start;
                    int rightLen = end - middle;
                    var leftCache = new T[leftLen];
                    var rightCache = new T[rightLen];
                    Merge(array, start, leftCache, rightCache);
                }

                #endregion
            }
        }

        private static void ArrayCopy<T>(IList<T> src, int srcPos, IList<T> dest, int destPos, int length)
            where T : IComparable<T>
        {
            for (int i = srcPos; i < srcPos + length; i++)
                dest[destPos++] = src[i];
        }

        private static void Merge<T>(IList<T> array, int start, T[] cache1, T[] cache2) where T : IComparable<T>
        {
            int rightIdx = 0;
            int leftIdx = 0;
            ArrayCopy(array, start, cache1, 0, cache1.Length);
            ArrayCopy(array, start + cache1.Length, cache2, 0, cache2.Length);
            for (int i = start;
                i < start + cache1.Length + cache2.Length && (rightIdx < cache2.Length) &&
                (leftIdx < cache1.Length);
                i++)
            {
                if (cache1[leftIdx].CompareTo(cache2[rightIdx]) <= 0) array[i] = cache1[leftIdx++];
                else array[i] = cache2[rightIdx++];
            }

            if (leftIdx < cache1.Length)
                ArrayCopy(cache1, leftIdx, array, start + leftIdx + rightIdx, cache1.Length - leftIdx);

            else if (rightIdx < cache2.Length)
                ArrayCopy(cache2, rightIdx, array, start + leftIdx + rightIdx, cache2.Length - rightIdx);
        }

        public static void QuickSort<T>(this IList<T> list) where T : IComparable<T>
        {
            Sort(list, 0, list.Count);

            #region InnerMethods

            static void Sort(IList<T> list, int start, int end)
            {
                if ((end - start) > 1)
                {
                    int middle = Partition(list, start, end);
                    Sort(list, start, middle);
                    Sort(list, middle, end);
                }
            }

            static int Partition(IList<T> list, int start, int end)
            {
                var pivot = list[end - 1];
                int i = start - 1;
                for (int j = start; j < end - 1; j++)
                {
                    if (list[j].CompareTo(pivot) <= 0)
                    {
                        var t = list[j];
                        list[j] = list[++i];
                        list[i] = t;
                    }
                }

                list[end - 1] = list[++i];
                list[i] = pivot;
                return i;
            }

            #endregion
        }

        public static void RandomQuickSort<T>(this IList<T> list) where T : IComparable<T>
        {
            Sort(list, 0, list.Count);

            #region InnerMethods

            static void Sort(IList<T> list, int start, int end)
            {
                if ((end - start) > 1)
                {
                    int middle = RandomPartition(list, start, end);
                    Sort(list, start, middle);
                    Sort(list, middle, end);
                }
            }
            #endregion
        }
        
        internal static int RandomPartition<T>(IList<T> a, int start, int end) where T : IComparable<T>
        {
            var rand = new Random();
            int pivotIdx = rand.Next(start, end);
            var pivot = a[pivotIdx];

            var temp = a[end - 1];
            a[end - 1] = pivot;
            a[pivotIdx] = temp;

            int i = start - 1;
            for (int j = start; j < end - 1; j++)
            {
                if (a[j].CompareTo(pivot) <= 0)
                {
                    var t = a[j];
                    a[j] = a[++i];
                    a[i] = t;
                }
            }

            a[end - 1] = a[++i];
            a[i] = pivot;
            return i;
        }

        public static void HeapSort<T>(this IList<T> list) where T : IComparable<T>
        {
            BuildMaxHeap(list);
            int heapSize = list.Count;
            for (int i = list.Count - 1; i >= 1; i--)
            {
                var t = list[0];
                list[0] = list[i];
                list[i] = t;
                MaxHeapify(list, 0, --heapSize);
            }

            #region InnerMethods

            static void MaxHeapify(IList<T> arr, int idx, int heapSize)
            {
                while (true)
                {
                    int l = 2 * (idx + 1);
                    int lIdx = l - 1;
                    int r = 2 * (idx + 1) + 1;
                    int rIdx = r - 1;
                    int maxIdx = idx;
                    if (lIdx < heapSize && arr[lIdx].CompareTo(arr[maxIdx]) > 0)
                    {
                        maxIdx = lIdx;
                    }

                    if (rIdx < heapSize && arr[rIdx].CompareTo(arr[maxIdx]) > 0)
                    {
                        maxIdx = rIdx;
                    }

                    if (maxIdx != idx)
                    {
                        var t = arr[maxIdx];
                        arr[maxIdx] = arr[idx];
                        arr[idx] = t;
                        idx = maxIdx;
                        continue;
                    }

                    break;
                }
            }

            static void BuildMaxHeap(IList<T> arr)
            {
                for (int i = arr.Count / 2 - 1; i >= 0; i--)
                {
                    MaxHeapify(arr, i, arr.Count);
                }
            }

            #endregion
        }

        /// <summary>
        /// int mean distributed data only
        /// </summary>
        /// <param name="list"></param>
        public static void CountingSort(this IList<int> list)
        {
            var b = new int[list.Count];
            int min = list[0], max = list[0];
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i] > max)
                {
                    max = list[i];
                }

                if (list[i] < min)
                {
                    min = list[i];
                }
            }

            int range = max - min + 1;
            int[] c = new int[range];
            foreach (int value in list)
            {
                c[(value - min)]++;
            }

            for (int i = 1; i < range; i++)
            {
                c[i] = c[i] + c[i - 1];
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                b[c[(list[i] - min)] - 1] = list[i];
                c[(list[i] - min)]--;
            }

            ArrayCopy(b, 0, list, 0, b.Length);
        }

        /// <summary>
        /// mean distribution, input[0,1)
        /// </summary>
        /// <param name="a"></param>
        public static void BucketSort(this IList<double> a)
        {
            var b = new List<List<double>>();
            int n = a.Count;
            for (int i = 0; i < n; i++)
            {
                b.Add(new List<double>());
            }

            foreach (var ai in a)
            {
                b[(int) Math.Floor(n * ai)].Add(ai);
            }

            var resList = new List<double>();
            foreach (var list in b)
            {
                list.Sort();
                resList.AddRange(list);
            }

            for (int i = 0; i < n; i++)
            {
                a[i] = resList[i];
            }
        }

        public static void BucketSort(this IList<float> list)
        {
            var b = new List<List<float>>();
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                b.Add(new List<float>());
            }

            foreach (var ai in list)
            {
                b[(int) Math.Floor(n * ai)].Add(ai);
            }

            var resList = new List<float>();
            foreach (var l in b)
            {
                l.Sort();
                resList.AddRange(l);
            }

            for (int i = 0; i < n; i++)
            {
                list[i] = resList[i];
            }
        }

        public static void BucketSort(this IList<decimal> list)
        {
            var b = new List<List<decimal>>();
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                b.Add(new List<decimal>());
            }

            foreach (var ai in list)
            {
                b[(int) Math.Floor(n * ai)].Add(ai);
            }

            var resList = new List<decimal>();
            foreach (var l in b)
            {
                l.Sort();
                resList.AddRange(l);
            }

            for (int i = 0; i < n; i++)
            {
                list[i] = resList[i];
            }
        }

        /// <summary>
        /// sort properties from smaller to bigger
        /// </summary>
        /// <param name="list"></param>
        public static void RadixSort(this IList<RawDate> list)
        {
            var t = list.OrderBy(d => d.Day).ThenBy(d => d.Month).ThenBy(d => d.Year);
            int i = 0;
            foreach (var date in t)
            {
                list[i] = date;
                i++;
            }
        }
    }
}