#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using CSharpLibraries.Algorithms.Structures;
using static CSharpLibraries.Extensions.Extension;

namespace CSharpLibraries.Algorithms.Graph
{
    /// <summary>
    /// single source shortest path
    /// </summary>
    public static class SsShortestPath
    {
        // general case algorithm: negative weight, cyclic
        public static bool BellmanFord<T>(LinkedGraph<Bfs.BfsVertex<T>> graph, Bfs.BfsVertex<T> s)
        {
            NotNullArg(s,nameof(s));
            NotNullArg(graph,nameof(graph));
            InitializeSingleSource(graph, s);
            int verticesCount = graph.Size;
            var edges = graph.AllEdges();
            for (int i = 1; i < verticesCount; i++)
            {
                foreach (var edge in edges) Relax(edge);
            }

            foreach (var edge in edges)
            {
                if (edge.LaterVertex.Distance > edge.FormerVertex.Distance + edge.Weight)
                    return false;
            }

            return true;
        }

        #region Private

        private static void InitializeSingleSource<T>(LinkedGraph<Bfs.BfsVertex<T>> graph, Bfs.BfsVertex<T> s)
        {
            var vertices = graph.AllVertices();
            foreach (var v in vertices)
            {
                v.Distance = double.PositiveInfinity;
                v.Parent = null;
                if (s == v) s.Distance = 0;
            }
        }

        private static void Relax<T>(LinkedGraph<Bfs.BfsVertex<T>>.Edge edge)
        {
            var weight = edge.Weight;
            var u = edge.FormerVertex;
            var v = edge.LaterVertex;
            var sum = u.Distance + weight;
            if (v.Distance > sum)
            {
                v.Distance = sum;
                v.Parent = u;
            }
        }

        #endregion

        // shortest paths of directed acyclic graph
        public static LinkedGraph<Bfs.BfsVertex<T>> ShortestPathOfDag<T>(
            LinkedGraph<Dfs.DfsVertex<Bfs.BfsVertex<T>>> dfsLinkedGraph,
            LinkedGraph<Bfs.BfsVertex<T>> bfsLinkedGraph, Bfs.BfsVertex<T> s)
        {
            NotNullArg(s,nameof(s));
            NotNullArg(dfsLinkedGraph,nameof(dfsLinkedGraph));
            NotNullArg(bfsLinkedGraph,nameof(bfsLinkedGraph));
            var dfsList = Dfs.TopologicalSort(dfsLinkedGraph);
            InitializeSingleSource(bfsLinkedGraph, s);
            dfsList.Sort(Comparer<Dfs.DfsVertex<Bfs.BfsVertex<T>>>.Create(
                (d1, d2) => d2.Finish - d1.Finish));
            var bfsList = dfsList.Select((vertex => vertex.Id)).ToList();
            foreach (var u in bfsList)
            {
                var uEdges = bfsLinkedGraph.EdgesAt(u);
                foreach (var edge in uEdges) Relax(edge);
            }

            return bfsLinkedGraph;
        }

        
        // non-negative weight
        // fibonacci heap, time complexity: O(V^2*lgV + V*E)
        public static void DijkstraFibonacciHeap<T>(LinkedGraph<Bfs.BfsVertex<T>> graph, Bfs.BfsVertex<T> s)
        {
            InitializeSingleSource(graph, s);
            var vertices = graph.AllVertices();
            var priorityQueue = new FibonacciHeap<double, Bfs.BfsVertex<T>>(
                (a, b) => a - b > 0 ? 1 : a - b < 0 ? -1 : 0);
            foreach (var vertex in vertices) priorityQueue.Insert(vertex.Distance, vertex);
            while (priorityQueue.Count > 0)
            {
                var u = priorityQueue.ExtractMin();
                var uEdges = graph.EdgesAt(u);
                foreach (var edge in uEdges)
                {
                    var v = edge.AnotherSide(u);
                    var original = v.Distance;
                    Relax(edge);
                    if (v.Distance < original) priorityQueue.DecreaseKey(v, v.Distance);
                }
            }
        }

        // min heap, time complexity: O(V*E*lgV)
        public static void DijkstraMinHeap<T>(LinkedGraph<Bfs.BfsVertex<T>> graph, Bfs.BfsVertex<T> s)
        {
            InitializeSingleSource(graph, s);
            var vertices = graph.AllVertices();
            var priorityQueue = new MinHeap<double, Bfs.BfsVertex<T>>(vertices, vertex => vertex.Distance,
                (a, b) => a - b > 0 ? 1 : a - b < 0 ? -1 : 0);
            while (priorityQueue.HeapSize > 0)
            {
                var u = priorityQueue.ExtractMin();
                var uEdges = graph.EdgesAt(u);
                foreach (var edge in uEdges)
                {
                    var v = edge.AnotherSide(u);
                    var original = v.Distance;
                    Relax(edge);
                    if (v.Distance < original)
                    {
                        priorityQueue.UpdateKey(v, v.Distance);
                    }
                }
            }
        }
    }
}