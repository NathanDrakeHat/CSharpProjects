using System;
using System.Collections.Generic;

namespace CSharpLibraries.Algorithms.Graph
{
    /// <summary>
    /// all pair shortest path
    /// </summary>
    public static class ApShortestPath
    {
        // O(V^4)
        public static double[][] SlowAllPairsShortestPaths(double[][] weight)
        {
            var n = weight.Length;
            var l = weight;
            for (int m = 2; m <= n - 1; m++)
            {
                l = ExtendedShortestPath(l, weight);
            }

            // L^(n-1)
            return l;
        }

        private static double[][] ExtendedShortestPath(double[][] lOrigin, double[][] weight)
        {
            var n = weight.Length;
            var lNext = new double[n][];
            for (int i = 0; i < n; i++)
            {
                lNext[i] = new double[n];
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    lNext[i][j] = double.PositiveInfinity;
                    for (int k = 0; k < n; k++)
                    {
                        lNext[i][j] = Math.Min(lNext[i][j], lOrigin[i][k] + weight[k][j]);
                    }
                }
            }

            return lNext;
        }

        // O(V^3*lgV)
        public static double[][] FasterAllPairsShortestPaths(double[][] weight)
        {
            var n = weight.Length;
            var l = weight;
            int m = 1;
            for (; m < n - 1; m *= 2)
            {
                l = ExtendedShortestPath(l, l);
            }

            return l;
        }

        // no negative-Weight cycles
        public static double[][] FloydWarshall(double[][] weight)
        {
            var n = weight.Length;
            var dOrigin = weight;
            for (int k = 0; k < n; k++)
            {
                var dCurrent = new double[n][];
                for (int i = 0; i < n; i++)
                {
                    dCurrent[i] = new double[n];
                }

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        dCurrent[i][j] = Math.Min(dOrigin[i][j], dOrigin[i][k] + dOrigin[k][j]);
                    }
                }

                dOrigin = dCurrent;
            }

            return dOrigin;
        }


        public static bool[,] TransitiveClosure(double[][] weight)
        {
            if (weight == null) throw new ArgumentNullException(nameof(weight));
            var n = weight.Length;
            var T = new bool[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    T[i, j] = (i == j) || !weight[i][j].Equals(double.PositiveInfinity);
                }
            }

            for (int k = 0; k < n; k++)
            {
                var tK = new bool[n, n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        tK[i, j] = T[i, j] || (T[i, k] && T[k, j]);
                    }
                }

                T = tK;
            }

            return T;
        }


        /// <summary>
        /// sparse graph
        /// <br/>Fibonacci heap: O(V^2*lgV + V*E)
        /// <br/>Min heap: O(V*E*lgV)
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="type"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static double[,] Johnson<T>(LinkedGraph<Bfs.BfsVertex<T>> graph, SsShortestPath.Heap type)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            var h = new Dictionary<Bfs.BfsVertex<T>, double>();
            var n = graph.Size;
            var verticesNew = graph.AllVertices();
            var s = new Bfs.BfsVertex<T>();
            verticesNew.Add(s);
            var newGraph = BuildGraph(graph, verticesNew, s);
            if (!SsShortestPath.BellmanFord(newGraph, s))
            {
                throw new InvalidOperationException();
            }
            else
            {
                var edgesNew = newGraph.AllEdges();
                foreach (var vertex in verticesNew)
                {
                    h[vertex] = vertex.Distance;
                }

                foreach (var edge in edgesNew)
                {
                    edge.Weight = edge.Weight + edge.FormerVertex.Distance - edge.LaterVertex.Distance;
                }

                var d = new double[n, n];
                int idxU = 0;
                foreach (var u in verticesNew)
                {
                    if (u != s)
                    {
                        int idxV = 0;
                        SsShortestPath.Dijkstra(graph, u, type);
                        foreach (var v in verticesNew)
                        {
                            if (v != s)
                            {
                                d[idxU, idxV] = v.Distance + h[v] - h[u];
                                idxV++;
                            }
                        }

                        idxU++;
                    }
                }

                return d;
            }
        }

        private static LinkedGraph<Bfs.BfsVertex<T>> BuildGraph<T>(LinkedGraph<Bfs.BfsVertex<T>> graph,
            IEnumerable<Bfs.BfsVertex<T>> vertices, Bfs.BfsVertex<T> s)
        {
            var newGraph = new LinkedGraph<Bfs.BfsVertex<T>>(graph);
            newGraph.AddVertex(s);
            foreach (var vertex in vertices)
            {
                if (vertex != s)
                {
                    newGraph.SetNeighbor(s, vertex);
                }
            }

            return newGraph;
        }
    }
}