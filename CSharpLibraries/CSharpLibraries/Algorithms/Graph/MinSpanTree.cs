#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using CSharpLibraries.Algorithms.Structures;
using static CSharpLibraries.Extensions.Extension;

namespace CSharpLibraries.Algorithms.Graph
{
    public static class MinSpanTree
    {
        #region InnerClass

        // ReSharper disable once IdentifierTypo
        public sealed class KruskalVertex<TId> : DisjointSet
        {
            public readonly TId Id;

            public KruskalVertex(TId n)
            {
                Id = n ?? throw new ArgumentNullException(nameof(n));
            }


            // ReSharper disable twice StringLiteralTypo
            public override string ToString() => $"KruskalVertex: {Id}";
        }

        #endregion

        // ReSharper disable once IdentifierTypo
        public static HashSet<LinkedGraph<KruskalVertex<T>>.Edge> AlgorithmOfKruskal<T>(
            LinkedGraph<KruskalVertex<T>> graph)
        {
           NotNullArg(graph,nameof(graph));
            var res = new HashSet<LinkedGraph<KruskalVertex<T>>.Edge>();
            var edgesSet = graph.AllEdges();
            var edgesList = edgesSet.ToList();
            edgesList.Sort(Comparer<LinkedGraph<KruskalVertex<T>>.Edge>.Create(
                (edge1, edge2) => edge1.Weight.CompareTo(edge2.Weight)));
            foreach (var edge in edgesList)
            {
                var v1 = edge.FormerVertex;
                var v2 = edge.LaterVertex;
                if (DisjointSet.FindSet(v1) != DisjointSet.FindSet(v2))
                {
                    res.Add(edge);
                    DisjointSet.Union(v1, v2);
                }
            }

            return res;
        }

        #region InnerClass

        public sealed class PrimVertex<TId>
        {
            public readonly TId Id;
            public PrimVertex<TId> Parent { get; internal set; }
            internal double Key;

            public PrimVertex(TId id)
            {
                Id = id ?? throw new ArgumentNullException();
            }


            public override string ToString() => $"PrimVertex: ({Id})";
        }

        #endregion

        public static void PrimFibonacciHeap<T>(LinkedGraph<PrimVertex<T>> graph, PrimVertex<T> r)
        {
            NotNullArg(r,nameof(r));
            NotNullArg(graph,nameof(graph));
            var priorityQueue = new FibonacciHeap<double, PrimVertex<T>>((a, b) => a - b > 0 ? 1 : a - b < 0 ? -1 : 0);
            var vertices = graph.AllVertices();
            foreach (var u in vertices)
            {
                u.Key = u != r ? double.PositiveInfinity : 0.0;
                priorityQueue.Insert(u.Key, u);
                u.Parent = null;
            }

            while (priorityQueue.Count > 0)
            {
                var u = priorityQueue.ExtractMin();
                var uEdges = graph.EdgesAt(u);
                foreach (var edge in uEdges)
                {
                    var v = edge.AnotherSide(u);
                    if (priorityQueue.Contains(v) && edge.Weight < v.Key)
                    {
                        v.Parent = u;
                        v.Key = edge.Weight;
                        priorityQueue.DecreaseKey(v, v.Key);
                    }
                }
            }
        }

        public static void PrimMinHeap<T>(LinkedGraph<PrimVertex<T>> graph, PrimVertex<T> r)
        {
            NotNullArg(r,nameof(r));
            NotNullArg(graph,nameof(graph));
            var vertices = graph.AllVertices();
            foreach (var u in vertices)
            {
                u.Key = u != r ? double.PositiveInfinity : 0.0;
                u.Parent = null;
            }

            var minHeap = new MinHeap<double, PrimVertex<T>>(vertices, vertex => vertex.Key,
                (a, b) => a - b > 0 ? 1 : a - b < 0 ? -1 : 0);
            while (minHeap.HeapSize > 0)
            {
                var u = minHeap.ExtractMin();
                var uEdges = graph.EdgesAt(u);
                foreach (var edge in uEdges)
                {
                    var v = edge.AnotherSide(u);
                    if (minHeap.Contains(v) && edge.Weight < v.Key)
                    {
                        v.Parent = u;
                        v.Key = edge.Weight;
                        minHeap.UpdateKey(v, v.Key);
                    }
                }
            }
        }
    }
}