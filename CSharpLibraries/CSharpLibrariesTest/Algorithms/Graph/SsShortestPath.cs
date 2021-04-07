using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CSharpLibraries.Algorithms.Graph;
using NUnit.Framework;

namespace CSharpLibrariesTest.Algorithms.Graph{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public static class SsShortestPathTest{
        [Test]
        public static void BellmanFordTest(){
            var graph = BuildBellmanFordCase();
            var b = SsShortestPath.BellmanFord(graph, _targetBellmanFordCaseS);
            Bfs.BfsVertex<string> target = _targetBellmanFordCaseZ;
            var vertices = graph.AllVertices();
            foreach (var v in vertices){
                if (v == target){
                    target = v;
                }
            }

            Assert.True(-2 == target.Distance);
            var res = new List<string>();
            while (target != null){
                res.Add(target.Id);
                target = target.Parent;
            }

            Assert.True(b);
            Assert.AreEqual(new List<string>{"z", "t", "x", "y", "s"}, res);
        }

        static Bfs.BfsVertex<string> _targetBellmanFordCaseS;
        static Bfs.BfsVertex<string> _targetBellmanFordCaseZ;

        static LinkedGraph<Bfs.BfsVertex<string>> BuildBellmanFordCase(){
            string[] names = "s,t,x,y,z".Split(",");
            var vertices = new List<Bfs.BfsVertex<string>>();
            foreach (var n in names){
                vertices.Add(new Bfs.BfsVertex<string>(n));
            }

            var res = new LinkedGraph<Bfs.BfsVertex<string>>(vertices, GraphDirection.Directed);
            int[] index1 = new int[]{0, 0, 1, 1, 1, 2, 3, 3, 4, 4};
            int[] index2 = new int[]{1, 3, 2, 3, 4, 1, 2, 4, 0, 2};
            double[] weights = new double[]{6, 7, 5, 8, -4, -2, -3, 9, 2, 7};
            for (int i = 0; i < index1.Length; i++){
                res.SetNeighbor(vertices[index1[i]], vertices[index2[i]], weights[i]);
            }

            _targetBellmanFordCaseS = vertices[0];
            _targetBellmanFordCaseZ = vertices[4];
            return res;
        }

        [Test]
        public static void ShortestPathOfDagTest(){
            var twoGraph = BuildShortestPathOfDagForBfs();
            var res = SsShortestPath.ShortestPathOfDag(twoGraph.DfsG, twoGraph.BfsG, _targetShortestPathOfDagForBfs);
            var vertices = res.AllVertices();
            var l = vertices.ToList();
            l.Sort(Comparer<Bfs.BfsVertex<string>>.Create(
                (a, b) => String.Compare(a.Id, b.Id, StringComparison.Ordinal)));
            Assert.IsNull(l[0].Parent);
            Assert.IsNull(l[1].Parent);
            Assert.True(l[1] == l[2].Parent);
            Assert.True(2 == l[2].Distance);

            Assert.True(l[1] == l[3].Parent);
            Assert.True(6 == l[3].Distance);

            Assert.True(l[3] == l[4].Parent);
            Assert.True(5 == l[4].Distance);

            Assert.True(l[4] == l[5].Parent);
            Assert.True(3 == l[5].Distance);
        }

        static Bfs.BfsVertex<string> _targetShortestPathOfDagForBfs;

        static Result BuildShortestPathOfDagForBfs(){
            string[] names = "r,s,t,x,y,z".Split(",");
            var bfsVertex = new List<Bfs.BfsVertex<string>>();
            foreach (string name in names){
                bfsVertex.Add(new Bfs.BfsVertex<string>(name));
            }

            var bfsG = new LinkedGraph<Bfs.BfsVertex<string>>(bfsVertex, GraphDirection.Directed);
            int[] index1 = new int[]{0, 0, 1, 1, 2, 2, 2, 3, 3, 4};
            int[] index2 = new int[]{1, 2, 2, 3, 3, 4, 5, 4, 5, 5};
            double[] weights = new double[]{5, 3, 2, 6, 7, 4, 2, -1, 1, -2};
            for (int i = 0; i < index1.Length; i++){
                bfsG.SetNeighbor(bfsVertex[index1[i]], bfsVertex[index2[i]], weights[i]);
            }

            var dfsVertices = bfsVertex.Select(bfs => new Dfs.DfsVertex<Bfs.BfsVertex<string>>(bfs)).ToList();
            var dfsG = new LinkedGraph<Dfs.DfsVertex<Bfs.BfsVertex<string>>>(dfsVertices, GraphDirection.Directed);
            int len = dfsVertices.Count;
            for (int i = 0; i < len - 1; i++){
                dfsG.SetNeighbor(dfsVertices[i], dfsVertices[i + 1]);
            }

            var t = new Result{BfsG = bfsG, DfsG = dfsG};
            _targetShortestPathOfDagForBfs = bfsVertex[1];
            return t;
        }

        sealed class Result{
            public LinkedGraph<Bfs.BfsVertex<string>> BfsG;
            public LinkedGraph<Dfs.DfsVertex<Bfs.BfsVertex<string>>> DfsG;
        }

        [Test]
        public static void DijkstraFibonacciHeapTest(){
            var g = BuildDijkstraCase();
            SsShortestPath.DijkstraFibonacciHeap(g, _targetDijkstraCase);
            var vertices = g.AllVertices().ToList();
            vertices.Sort(Comparer<Bfs.BfsVertex<string>>.Create(((a, b) =>
                String.Compare(a.Id, b.Id, StringComparison.Ordinal))));
            Assert.IsNull(vertices[0].Parent);

            Assert.True(vertices[3] == vertices[1].Parent);
            Assert.True(8 == vertices[1].Distance);

            Assert.True(vertices[1] == vertices[2].Parent);
            Assert.True(9 == vertices[2].Distance);

            Assert.True(vertices[0] == vertices[3].Parent);
            Assert.True(5 == vertices[3].Distance);

            Assert.True(vertices[3] == vertices[4].Parent);
            Assert.True(7 == vertices[4].Distance);
        }

        [Test]
        public static void DijkstraMinHeapTest(){
            var g = BuildDijkstraCase();
            SsShortestPath.DijkstraMinHeap(g, _targetDijkstraCase);
            var vertices = g.AllVertices().ToList();
            vertices.Sort(Comparer<Bfs.BfsVertex<string>>.Create(((a, b) =>
                String.Compare(a.Id, b.Id, StringComparison.Ordinal))));
            Assert.IsNull(vertices[0].Parent);

            Assert.True(vertices[3] == vertices[1].Parent);
            Assert.True(8 == vertices[1].Distance);

            Assert.True(vertices[1] == vertices[2].Parent);
            Assert.True(9 == vertices[2].Distance);

            Assert.True(vertices[0] == vertices[3].Parent);
            Assert.True(5 == vertices[3].Distance);

            Assert.True(vertices[3] == vertices[4].Parent);
            Assert.True(7 == vertices[4].Distance);
        }

        static Bfs.BfsVertex<string> _targetDijkstraCase;

        static LinkedGraph<Bfs.BfsVertex<string>> BuildDijkstraCase(){
            string[] names = "s,t,x,y,z".Split(",");
            var vertices = new List<Bfs.BfsVertex<string>>();
            foreach (var n in names){
                vertices.Add(new Bfs.BfsVertex<string>(n));
            }

            var graph = new LinkedGraph<Bfs.BfsVertex<string>>(vertices, GraphDirection.Directed);
            int[] indices1 = new int[]{0, 0, 1, 1, 2, 3, 3, 3, 4, 4};
            int[] indices2 = new int[]{1, 3, 2, 3, 4, 1, 2, 4, 0, 2};
            double[] weights = new double[]{10, 5, 1, 2, 4, 3, 9, 2, 7, 6};
            for (int i = 0; i < indices1.Length; i++){
                graph.SetNeighbor(vertices[indices1[i]], vertices[indices2[i]], weights[i]);
            }

            _targetDijkstraCase = vertices[0];
            return graph;
        }
    }
}