#nullable disable
using System.Collections.Generic;
using System.Text;
using static CSharpLibraries.Utils.Extension;

namespace CSharpLibraries.Algorithms.DP
{
    /// <summary>
    /// optimal matrices multiply order
    /// </summary>
    public static class MatrixChain
    {
        public class MatrixChainResult
        {
            public int MinCost;
            internal Node Res;

            internal class Node
            {
                // arithmetic tree
                // only leave store numbers
                public readonly int Number; // a * b
                public Node Left;
                public Node Right;

                // medium node without number
                public Node()
                {
                }

                // number node initial
                public Node(int number)
                {
                    Number = number;
                }
            }

            private static void Walk(Node p, StringBuilder res)
            {
                if (p.Left != null || p.Right != null)
                {
                    res.Append('(');
                }

                if (p.Left != null)
                {
                    Walk(p.Left, res);
                }

                if (p.Right != null)
                {
                    Walk(p.Right, res);
                }

                if (p.Right == null && p.Left == null)
                {
                    res.Append(p.Number + 1);
                }

                if (p.Left != null || p.Right != null)
                {
                    res.Append(')');
                }
            }

            public override string ToString()
            {
                var t = new StringBuilder();
                Walk(Res!, t);
                return t.ToString();
            }
        }

        public static MatrixChainResult OptimalMatrixChainOrder(IList<MatrixIndex> p)
        {
            MatrixChainResult[,] m = new MatrixChainResult[p.Count, p.Count]; 
            for (int i = 0; i < p.Count; i++)
            {
                m[i, i] = new MatrixChainResult {MinCost = 0, Res = new MatrixChainResult.Node(i)};
            }

            for (int l = 2; l <= p.Count; l++)
            {
                // len
                for (int s = 0; s + l - 1 < p.Count; s++)
                {
                    // start
                    int e = s + l - 1; // end

                    m[s, e] = new MatrixChainResult();
                    if (l == 2)
                    {
                        m[s, e].MinCost = p[s].Row * p[s].Col * p[e].Col;
                        m[s, e].Res = new MatrixChainResult.Node {Left = m[s, s].Res, Right = m[e, e].Res};
                    }
                    else
                    {
                        m[s, e].MinCost = p[s].Row * p[s].Col * p[e].Col + m[s, s].MinCost +
                                          m[s + 1, e].MinCost;
                        m[s, e].Res = new MatrixChainResult.Node {Left = m[s, s].Res, Right = m[s + 1, e].Res};
                        for (int i = 1; i < l - 1; i++)
                        {
                            int cost = m[s, s + i].MinCost + m[s + i + 1, e].MinCost +
                                       p[s].Row * p[s + i + 1].Row * p[e].Col;
                            if (cost < m[s, e].MinCost)
                            {
                                m[s, e].MinCost = cost;
                                m[s, e].Res = new MatrixChainResult.Node
                                {
                                    Left = m[s, s + i].Res, Right = m[s + i + 1, e].Res
                                };
                            }
                        }
                    }
                }
            }

            return m[0, p.Count - 1];
        }
    }
}