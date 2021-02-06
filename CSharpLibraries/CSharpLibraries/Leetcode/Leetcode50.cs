using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CSharpLibraries.Utils.Extension;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace CSharpLibraries.Leetcode
{
    public static class Leetcode50
    {
        

        /// <summary>
        /// #37
        /// <br/>solve sudoku problem
        /// <br/><code>
        ///  char[][] t =
        ///  {
        ///      new[] {'5', '3', '.', '.', '7', '.', '.', '.', '.'},
        ///      new[] {'6', '.', '.', '1', '9', '5', '.', '.', '.'},
        ///      new[] {'.', '9', '8', '.', '.', '.', '.', '6', '.'},
        ///      new[] {'8', '.', '.', '.', '6', '.', '.', '.', '3'},
        ///      new[] {'4', '.', '.', '8', '.', '3', '.', '.', '1'},
        ///      new[] {'7', '.', '.', '.', '2', '.', '.', '.', '6'},
        ///      new[] {'.', '6', '.', '.', '.', '.', '2', '8', '.'},
        ///      new[] {'.', '.', '.', '4', '1', '9', '.', '.', '5'},
        ///      new[] {'.', '.', '.', '.', '8', '.', '.', '7', '9'}
        ///   };
        /// </code>
        /// </summary>
        /// <param name="board">two-dimension char array. char '.' indicate space </param>
        public static void SolveSudoku(char[][] board)
        {
            var spaces = new List<MatrixIndex>();
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    char f = board[r][c];
                    if (f == '.')
                    {
                        spaces.Add(new MatrixIndex(r, c));
                    }
                }
            }

            var relatedSpaces = new Dictionary<MatrixIndex, List<MatrixIndex>>();
            foreach (var spacePtr1 in spaces)
            {
                relatedSpaces[spacePtr1] = new List<MatrixIndex>();
                foreach (var spacePtr2 in spaces.Where(spacePtr2 =>
                    !spacePtr2.Equals(spacePtr1) && Related(spacePtr1, spacePtr2)))
                {
                    relatedSpaces[spacePtr1].Add(spacePtr2);
                }
            }

            spaces.Sort((m1, m2) =>
            {
                var rc1 = relatedSpaces[m1].Count;
                var rc2 = relatedSpaces[m2].Count;
                return rc1 - rc2;
            });

            var sortedGroupedSpaces = new List<MatrixIndex>(spaces.Count);
            var spacesSet = new HashSet<MatrixIndex>(spaces);
            foreach (var s in spaces.Where(s => spacesSet.Contains(s)))
            {
                sortedGroupedSpaces.Add(s);
                spacesSet.Remove(s);
                var rs = relatedSpaces[s];
                foreach (var e in rs.Where(e => spacesSet.Contains(e)))
                {
                    sortedGroupedSpaces.Add(e);
                    spacesSet.Remove(e);
                }
            }

            var availableChars = new Dictionary<MatrixIndex, HashSet<char>>();
            foreach (var space in spaces)
                availableChars[space] = GetAvailableChars(space.Row, space.Col, board);

            DepthFirstSearchSudoku(sortedGroupedSpaces, 0, board, availableChars, relatedSpaces,
                new HashSet<MatrixIndex>());


            #region Methods

            static bool DepthFirstSearchSudoku(List<MatrixIndex> spaces, int spaceIdx, char[][] board,
                Dictionary<MatrixIndex, HashSet<char>> availableChars,
                Dictionary<MatrixIndex, List<MatrixIndex>> relatedSpaces,
                HashSet<MatrixIndex> encountered)
            {
                if (spaceIdx == spaces.Count)
                {
                    return true;
                }

                var currentSpace = spaces[spaceIdx];
                encountered.Add(currentSpace);
                int rIdx = currentSpace.Row;
                int cIdx = currentSpace.Col;
                foreach (var chr in availableChars[currentSpace])
                {
                    board[rIdx][cIdx] = chr;
                    List<bool> record = new List<bool>();
                    foreach (var relatedSpace in relatedSpaces[currentSpace])
                    {
                        if (!encountered.Contains(relatedSpace))
                        {
                            record.Add(availableChars[relatedSpace].Remove(chr));
                        }
                    }

                    bool success = DepthFirstSearchSudoku(spaces, spaceIdx + 1, board, availableChars, relatedSpaces,
                        encountered);
                    if (success) return true;
                    int accIdx = 0;
                    foreach (var relatedSpace in relatedSpaces[currentSpace])
                    {
                        if (!encountered.Contains(relatedSpace))
                        {
                            if (record[accIdx++])
                            {
                                availableChars[relatedSpace].Add(chr);
                            }
                        }
                    }
                }

                board[rIdx][cIdx] = '.';
                encountered.Remove(currentSpace);
                return false;
            }

            static bool Related(MatrixIndex a, MatrixIndex b)
            {
                static int BoxIndex(int row, int columns) => (row / 3) * 3 + columns / 3;
                (int ai1, int ai2) = a;
                (int bi1, int bi2) = b;
                return ai1 == bi1 || ai2 == bi2 || BoxIndex(ai1, ai2) == BoxIndex(bi1, bi2);
            }

            static HashSet<char> GetAvailableChars(int rIdx, int cIdx, char[][] board)
            {
                Dictionary<int, int[][]> groupIdxToBoxIdx = new Dictionary<int, int[][]> // rows and cols
                {
                    {0, new[] {new[] {0, 1, 2}, new[] {0, 1, 2}}},
                    {1, new[] {new[] {0, 1, 2}, new[] {3, 4, 5}}},
                    {2, new[] {new[] {0, 1, 2}, new[] {6, 7, 8}}},
                    {3, new[] {new[] {3, 4, 5}, new[] {0, 1, 2}}},
                    {4, new[] {new[] {3, 4, 5}, new[] {3, 4, 5}}},
                    {5, new[] {new[] {3, 4, 5}, new[] {6, 7, 8}}},
                    {6, new[] {new[] {6, 7, 8}, new[] {0, 1, 2}}},
                    {7, new[] {new[] {6, 7, 8}, new[] {3, 4, 5}}},
                    {8, new[] {new[] {6, 7, 8}, new[] {6, 7, 8}}}
                };
                var availableChars = new HashSet<char>();
                for (int i = 1; i <= 9; i++)
                {
                    availableChars.Add((char) (i + '0'));
                }

                for (int j = 0; j < 9; j++)
                {
                    char chr = board[rIdx][j];
                    if (chr != '.')
                    {
                        availableChars.Remove(chr);
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    char chr = board[i][cIdx];
                    if (chr != '.')
                    {
                        availableChars.Remove(chr);
                    }
                }

                int groupIdx = (rIdx / 3) * 3 + cIdx / 3;
                int[][] groupIndices = groupIdxToBoxIdx[groupIdx];
                int[] rowIndices = groupIndices[0];
                int[] colIndices = groupIndices[1];
                foreach (var i in rowIndices)
                {
                    foreach (var j in colIndices)
                    {
                        char chr = board[i][j];
                        if (chr != '.')
                        {
                            availableChars.Remove(chr);
                        }
                    }
                }

                return availableChars;
            }

            #endregion
        }


        /// <summary>
        /// #38
        /// <br/>外观数列<br/>
        /// <code>
        ///  1.     1
        ///  2.     11
        ///  3.     21
        ///  4.     1211
        ///  5.     111221
        /// </code>
        /// <br/>第一项是数字 1
        /// <br/>描述前一项，这个数是 1 即 “一个 1 ”，记作 11
        /// <br/>描述前一项，这个数是 11 即 “两个 1 ” ，记作 21
        /// <br/>描述前一项，这个数是 21 即 “一个 2 一个 1 ” ，记作 1211
        /// <br/>描述前一项，这个数是 1211 即 “一个 1 一个 2 两个 1 ” ，记作 111221
        /// </summary>
        /// <param name="n">index</param>
        /// <returns>result of index</returns>
        public static string CountAndSay(int n)
        {
            string start = "1";
            for (int i = 2; i <= n; i++)
            {
                start = Next(start);
            }

            return start;
        }

        private static string Next(string s)
        {
            int i = 0;
            StringBuilder res = new StringBuilder();
            while (i < s.Length)
            {
                int count = 1;
                while (i + 1 < s.Length && s[i] == s[i + 1])
                {
                    count++;
                    i++;
                }

                res.Append(count);
                res.Append(s[i]);
                i++;
            }

            return res.ToString();
        }

        /// <summary>
        /// #41
        /// <br/>第一个缺失的正数
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public static int FirstMissingPositive(int[] nums)
        {
            if (nums.Length == 0)
            {
                return 1;
            }
            
            for (int i = 0; i < nums.Length; i++)
            {
                int targetIdx = nums[i] - 1;
                while (targetIdx != i &&
                       targetIdx >= 0 && targetIdx < nums.Length &&
                       nums[i] > 0 && 
                       nums[targetIdx] != targetIdx + 1)
                {
                    var t = nums[targetIdx];
                    nums[targetIdx] = nums[i];
                    nums[i] = t;
                    targetIdx = nums[i] - 1;
                }
            }

            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i] != i + 1)
                {
                    return i + 1;
                }
            }

            return nums.Length + 1;
        }

        /// <summary>
        /// #45
        /// <br/>全排列<br/>
        /// <code>
        /// 输入: [1,1,2]
        /// 输出:
        /// [
        /// [1,1,2],
        /// [1,2,1],
        /// [2,1,1]
        /// ]
        /// </code>
        /// </summary>
        /// <param name="nums">int array</param>
        /// <returns>unique permutation list</returns>
        public static IList<IList<int>> PermuteUnique(int[] nums)
        {
            IList<IList<int>> finalRes = new List<IList<int>>();
            Array.Sort(nums);
            bool[] encountered = new bool[nums.Length];
            RecursivePermuteUnique(nums, encountered, new List<int>(), finalRes);
            return finalRes;
        }

        private static void RecursivePermuteUnique(
            int[] nums,
            bool[] inCache,
            List<int> currentCache,
            IList<IList<int>> finalRes)
        {
            if (currentCache.Count == nums.Length)
            {
                finalRes.Add(new List<int>(currentCache));
                return;
            }

            for (int i = 0; i < nums.Length; i++)
            {
                if (i + 1 < nums.Length && nums[i] == nums[i + 1] && !inCache[i + 1])
                {
                    continue;
                }
                else if (!inCache[i])
                {
                    inCache[i] = true;
                    currentCache.Add(nums[i]);
                    RecursivePermuteUnique(nums, inCache, currentCache, finalRes);
                    inCache[i] = false;
                    currentCache.RemoveAt(currentCache.Count - 1);
                }
            }
        }
    }
}