using System.Collections.Generic;
using System.Linq;
using CSharpLibraries.Algorithms.Graph;
using NUnit.Framework;

namespace CSharpLibrariesTest.Algorithms.Graph{
    public static class DfsTest{
        // ReSharper disable once StringLiteralTypo
        static string names = "uvwxyz";

        static LinkedGraph<Dfs.DfsVertex<char>> MakeGraph(){
            var vs = new List<Dfs.DfsVertex<char>>(6);

            for (int i = 0; i < 6; i++){
                vs.Add(new Dfs.DfsVertex<char>(names[i]));
            }

            var graph = new LinkedGraph<Dfs.DfsVertex<char>>(vs, GraphDirection.Directed);
            graph.SetNeighbor(vs[0], vs[1]);
            graph.SetNeighbor(vs[0], vs[3]);

            graph.SetNeighbor(vs[1], vs[4]);

            graph.SetNeighbor(vs[2], vs[4]);
            graph.SetNeighbor(vs[2], vs[5]);

            graph.SetNeighbor(vs[3], vs[1]);

            graph.SetNeighbor(vs[4], vs[3]);

            graph.SetNeighbor(vs[5], vs[5]);

            return graph;
        }

        static LinkedGraph<Dfs.DfsVertex<string>> MakeTopographicalDemo(){
            var a = new List<Dfs.DfsVertex<string>>(9);
            string t = "undershorts,pants,belt,shirt,tie,jacket,socks,shoes,watch";
            var strings = t.Split(",");
            for (int i = 0; i < 9; i++){
                a.Add(new Dfs.DfsVertex<string>(strings[i]));
            }

            var graph = new LinkedGraph<Dfs.DfsVertex<string>>(a, GraphDirection.Directed);
            graph.SetNeighbor(a[0], a[1]);
            graph.SetNeighbor(a[0], a[6]);

            graph.SetNeighbor(a[1], a[2]);
            graph.SetNeighbor(a[1], a[6]);

            graph.SetNeighbor(a[2], a[5]);

            graph.SetNeighbor(a[3], a[2]);
            graph.SetNeighbor(a[3], a[4]);

            graph.SetNeighbor(a[4], a[5]);

            graph.SetNeighbor(a[6], a[7]);

            return graph;
        }


        [Test]
        public static void DepthFirstSearchTest(){
            var graph = MakeGraph();
            Dfs.DepthFirstSearch(graph);
            var vertices = graph.AllVertices();
            var l = new List<Dfs.DfsVertex<char>>(vertices);
            l.Sort(Comparer<Dfs.DfsVertex<char>>.Create(((a, b) => a.Id.CompareTo(b.Id))));
            Assert.True(1 == l[0].Discover);
            Assert.True(8 == l[0].Finish);

            if (2 == l[1].Discover){
                Assert.True(7 == l[1].Finish);
                if (l[3].Discover == 4){
                    Assert.True(5 == l[3].Finish);
                    Assert.True(3 == l[4].Discover);
                    Assert.True(6 == l[4].Finish);
                }
                else{
                    Assert.True(3 == l[3].Discover);
                    Assert.True(6 == l[3].Finish);
                    Assert.True(4 == l[4].Discover);
                    Assert.True(5 == l[4].Finish);
                }
            }
            else{
                Assert.True(2 == l[3].Discover);
                Assert.True(7 == l[3].Finish);
                if (3 == l[4].Discover){
                    Assert.True(6 == l[4].Finish);
                    Assert.True(4 == l[1].Discover);
                    Assert.True(5 == l[1].Finish);
                }
                else{
                    Assert.True(4 == l[4].Discover);
                    Assert.True(5 == l[4].Finish);
                    Assert.True(3 == l[1].Discover);
                    Assert.True(6 == l[1].Finish);
                }
            }

            if (l[2].Discover == 9){
                Assert.True(12 == l[2].Finish);
                Assert.True(10 == l[5].Discover);
                Assert.True(11 == l[5].Finish);
            }
            else{
                Assert.True(11 == l[2].Discover);
                Assert.True(12 == l[2].Finish);
                Assert.True(9 == l[5].Discover);
                Assert.True(10 == l[5].Finish);
            }
        }

        [Test]
        public static void TopologicalSortTest(){
            var graph = MakeTopographicalDemo();
            var l = Dfs.TopologicalSort(graph);
            bool flag = true;
            for (int i = 1; i < l.Count; i++){
                for (int j = 0; j < i; j++){
                    flag = RecursiveTopologicalTest(l[j], l[i], graph);
                    if (!flag){
                        break;
                    }
                }

                if (!flag){
                    break;
                }
            }

            Assert.True(flag);
        }

        static bool RecursiveTopologicalTest(Dfs.DfsVertex<string> target, Dfs.DfsVertex<string> current,
            LinkedGraph<Dfs.DfsVertex<string>> graph){
            if (current.Equals(target)){
                return false;
            }

            var edges = graph.EdgesAt(current);
            if (!edges.Any()){
                return true;
            }
            else{
                bool t = true;
                foreach (var edge in edges){
                    var i = edge.AnotherSide(current);
                    t = RecursiveTopologicalTest(target, i, graph);
                    if (!t){
                        break;
                    }
                }

                return t;
            }
        }

        [Test]
        public static void RecursiveTopologicalTest(){
            var graph = MakeTopographicalDemo();
            bool flag = true;
            var t = new List<Dfs.DfsVertex<string>>(graph.AllVertices());
            for (int i = 1; i < t.Count; i++){
                for (int j = 0; j < i; j++){
                    flag = RecursiveTopologicalTest(t[j], t[i], graph);
                    if (!flag){
                        break;
                    }
                }

                if (!flag){
                    break;
                }
            }

            Assert.False(flag);
        }

        private static LinkedGraph<Dfs.DfsVertex<string>> MakeStronglyConnectedComponentsDemo(){
            string t = "a,b,c,d,e,f,g,h";
            var strings = t.Split(",");
            var array = new List<Dfs.DfsVertex<string>>(strings.Length);
            foreach (var t1 in strings){
                array.Add(new Dfs.DfsVertex<string>(t1));
            }

            var graph = new LinkedGraph<Dfs.DfsVertex<string>>(array, GraphDirection.Directed);
            graph.SetNeighbor(array[0], array[1]);

            graph.SetNeighbor(array[1], array[2]);
            graph.SetNeighbor(array[1], array[4]);
            graph.SetNeighbor(array[1], array[5]);

            graph.SetNeighbor(array[2], array[3]);
            graph.SetNeighbor(array[2], array[6]);

            graph.SetNeighbor(array[3], array[2]);
            graph.SetNeighbor(array[3], array[7]);

            graph.SetNeighbor(array[4], array[0]);
            graph.SetNeighbor(array[4], array[5]);

            graph.SetNeighbor(array[5], array[6]);

            graph.SetNeighbor(array[6], array[5]);
            graph.SetNeighbor(array[6], array[7]);

            graph.SetNeighbor(array[7], array[7]);

            return graph;
        }

        [Test]
        public static void StronglyConnectedComponentsTest(){
            var graph = MakeStronglyConnectedComponentsDemo();
            Dfs.StronglyConnectedComponents(graph);
            var vertices = graph.AllVertices();
            var vs = new List<Dfs.DfsVertex<string>>(vertices);
            Assert.True((GetRoot(vs[0]) == GetRoot(vs[1])) & (GetRoot(vs[1]) == GetRoot(vs[4])));
            Assert.AreSame(GetRoot(vs[2]), GetRoot(vs[3]));
            Assert.AreSame(GetRoot(vs[5]), GetRoot(vs[6]));
            Assert.AreSame(GetRoot(vs[7]), vs[7]);
        }

        static Dfs.DfsVertex<string> GetRoot(Dfs.DfsVertex<string> v){
            var t = v;
            while (t.Parent != null){
                t = t.Parent;
            }

            return t;
        }
    }
}