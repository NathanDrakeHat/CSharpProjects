using System.Collections.Generic;
using CSharpLibraries.Algorithms.Graph;
using NUnit.Framework;

namespace CSharpLibrariesTest.Algorithms.Graph
{
    public static class ApShortestPathTest
    {
        [Test]
        public static void SlowAllPairsShortestPathsTest()
        {
            var res = ApShortestPath.SlowAllPairsShortestPaths(new[]
            {
                new[] {0, double.PositiveInfinity, double.PositiveInfinity, 2, double.PositiveInfinity},
                new[] {3, 0, 4, double.PositiveInfinity, double.PositiveInfinity},
                new[] {8, double.PositiveInfinity, 0, -5, double.PositiveInfinity},
                new[] {double.PositiveInfinity, 1, double.PositiveInfinity, 0, 6},
                new[] {-4, 7, double.PositiveInfinity, double.PositiveInfinity, 0}
            });
            var answer = new[]
            {
                new double[] {0, 3, 7, 2, 8},
                new double[] {1, 0, 4, -1, 5},
                new double[] {-3, -4, 0, -5, 1},
                new double[] {2, 1, 5, 0, 6},
                new double[] {-4, -1, 3, -2, 0}
            };
            Assert.AreEqual(answer, res);
        }

        [Test]
        public static void FasterAllPairsShortestPathsTest()
        {
            var res = ApShortestPath.FasterAllPairsShortestPaths(new[]
            {
                new[] {0, double.PositiveInfinity, double.PositiveInfinity, 2, double.PositiveInfinity},
                new[] {3, 0, 4, double.PositiveInfinity, double.PositiveInfinity},
                new[] {8, double.PositiveInfinity, 0, -5, double.PositiveInfinity},
                new[] {double.PositiveInfinity, 1, double.PositiveInfinity, 0, 6},
                new[] {-4, 7, double.PositiveInfinity, double.PositiveInfinity, 0}
            });
            var answer = new[]
            {
                new double[] {0, 3, 7, 2, 8},
                new double[] {1, 0, 4, -1, 5},
                new double[] {-3, -4, 0, -5, 1},
                new double[] {2, 1, 5, 0, 6},
                new double[] {-4, -1, 3, -2, 0}
            };
            Assert.AreEqual(answer, res);
        }


        [Test]
        // ReSharper disable once IdentifierTypo
        public static void AlgorithmFloydWarshallTest()
        {
            var res = ApShortestPath.FloydWarshall(new[]
            {
                new[] {0, double.PositiveInfinity, double.PositiveInfinity, 2, double.PositiveInfinity},
                new[] {3, 0, 4, double.PositiveInfinity, double.PositiveInfinity},
                new[] {8, double.PositiveInfinity, 0, -5, double.PositiveInfinity},
                new[] {double.PositiveInfinity, 1, double.PositiveInfinity, 0, 6},
                new[] {-4, 7, double.PositiveInfinity, double.PositiveInfinity, 0}
            });
            var answer = new[]
            {
                new double[] {0, 3, 7, 2, 8},
                new double[] {1, 0, 4, -1, 5},
                new double[] {-3, -4, 0, -5, 1},
                new double[] {2, 1, 5, 0, 6},
                new double[] {-4, -1, 3, -2, 0}
            };
            Assert.True(MatrixEqual(res, answer));
        }

        [Test]
        public static void TransitiveClosureTest()
        {
            var res = ApShortestPath.TransitiveClosure(new[]
            {
                new[] {0, double.PositiveInfinity, double.PositiveInfinity, 1},
                new[] {double.PositiveInfinity, 0, 1, double.PositiveInfinity},
                new[] {double.PositiveInfinity, 1, 0, 1},
                new[] {double.PositiveInfinity, 1, double.PositiveInfinity, 0}
            });

            var answer = new[,]
            {
                {true, true, true, true},
                {false, true, true, true},
                {false, true, true, true},
                {false, true, true, true}
            };
            Assert.AreEqual(answer, res);
        }

        [Test]
        public static void AlgorithmJohnsonTest()
        {
            var res = ApShortestPath.Johnson(Build(), SsShortestPath.Heap.Fibonacci);
            var answer = new[,]
            {
                {0.0, 1.0, -3.0, 2.0, -4.0},
                {3.0, 0.0, -4.0, 1.0, -1.0},
                {7.0, 4.0, 0.0, 5.0, 3.0},
                {2.0, -1.0, -5.0, 0.0, -2.0},
                {8.0, 5.0, 1.0, 6.0, 0.0},
            };
            Assert.AreEqual(answer, res);

            res = ApShortestPath.Johnson(Build(), SsShortestPath.Heap.MinHeap);
            Assert.AreEqual(answer, res);
        }

        static LinkedGraph<Bfs.BfsVertex<string>> Build()
        {
            string[] names = "1,2,3,4,5".Split(",");
            var vertices = new List<Bfs.BfsVertex<string>>();
            foreach (var name in names)
            {
                vertices.Add(new Bfs.BfsVertex<string>(name));
            }

            var res = new LinkedGraph<Bfs.BfsVertex<string>>(vertices, GraphDirection.Directed);
            res.SetNeighbor(vertices[0], vertices[1], 3);
            res.SetNeighbor(vertices[0], vertices[2], 8);
            res.SetNeighbor(vertices[0], vertices[4], -4);

            res.SetNeighbor(vertices[1], vertices[3]);
            res.SetNeighbor(vertices[1], vertices[4], 7);

            res.SetNeighbor(vertices[2], vertices[1], 4);

            res.SetNeighbor(vertices[3], vertices[0], 2);
            res.SetNeighbor(vertices[3], vertices[2], -5);

            res.SetNeighbor(vertices[4], vertices[3], 6);
            return res;
        }

        private static bool MatrixEqual(double[][] a, double[][] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < a[0].Length; j++)
                {
                    if (!a[i][j].Equals(b[i][j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}