#nullable disable
using System;
using System.Collections.Generic;
using static CSharpLibraries.Utils.Extension;

namespace CSharpLibraries.Algorithms.Graph{
    public static class Dfs{
        #region InnerClass

        internal enum Color{
            White,
            Gray,
            Black
        }

        public sealed class DfsVertex<TId>{
            public DfsVertex<TId> Parent{ get; internal set; }
            internal Color Color;
            public int Discover{ get; internal set; } //d
            public int Finish{ get; internal set; } // f
            public readonly TId Id;

            public DfsVertex(TId id){
                Id = id ?? throw new ArgumentNullException(nameof(id));
            }
        }

        #endregion

        public static void DepthFirstSearch<T>(LinkedGraph<DfsVertex<T>> graph){
            graph.RequireNotNullArg(nameof(graph));
            var vertices = graph.AllVertices();
            foreach (var v in vertices){
                v.Color = Color.White;
                v.Parent = null;
            }

            int time = 0;
            foreach (var v in vertices){
                if (v.Color == Color.White) time = DfsVisit(graph, v, time);
            }
        }

        private static int DfsVisit<T>(LinkedGraph<DfsVertex<T>> graph, DfsVertex<T> u, int time){
            time++;
            u.Discover = time;
            u.Color = Color.Gray;
            var uEdges = graph.EdgesAt(u);
            foreach (var edge in uEdges){
                var v = edge.AnotherSide(u);
                if (v.Color == Color.White){
                    v.Parent = u;
                    time = DfsVisit(graph, v, time);
                }
            }

            u.Color = Color.Black;
            time++;
            u.Finish = time;
            return time;
        }

        public static List<DfsVertex<T>> TopologicalSort<T>(LinkedGraph<DfsVertex<T>> graph){
            graph.RequireNotNullArg(nameof(graph));
            DepthFirstSearch(graph);
            var l = new List<DfsVertex<T>>(graph.AllVertices());
            l.Sort(Comparer<DfsVertex<T>>.Create(((a, b) => b.Finish - a.Finish))); // descend order
            return l;
        }

        public static void StronglyConnectedComponents<T>(LinkedGraph<DfsVertex<T>> graph){
            graph.RequireNotNullArg(nameof(graph));
            var l = TopologicalSort(graph);
            var gT = TransposeGraph(graph);
            DepthFirstSearchOrderly(gT, l);
        }

        private static void DepthFirstSearchOrderly<T>(LinkedGraph<DfsVertex<T>> graph, List<DfsVertex<T>> order){
            var vertices = graph.AllVertices();
            foreach (var v in vertices){
                v.Color = Color.White;
                v.Parent = null;
            }

            int time = 0;
            foreach (var v in order){
                if (v.Color == Color.White){
                    time = DfsVisit(graph, v, time);
                }
            }
        }

        private static LinkedGraph<DfsVertex<T>> TransposeGraph<T>(LinkedGraph<DfsVertex<T>> graph){
            var newGraph = new LinkedGraph<DfsVertex<T>>(graph.AllVertices(), GraphDirection.Directed);
            var vertices = graph.AllVertices();
            foreach (var v in vertices){
                var edges = graph.EdgesAt(v);
                foreach (var edge in edges){
                    var n = edge.AnotherSide(v);
                    newGraph.SetNeighbor(n, v);
                }
            }

            return newGraph;
        }
    }
}