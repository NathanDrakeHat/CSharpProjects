using System.Collections.Generic;
using CSharpLibraries.Algorithms.Graph;
using NUnit.Framework;

namespace CSharpLibrariesTest.Algorithms.Graph{
    static class MinSpanTreeTest{
        [Test]
        // ReSharper disable once IdentifierTypo
        public static void KruskalTest(){
            var graph = BuildKruskalExample();
            var t = MinSpanTree.Kruskal(graph);
            int i = 0;
            foreach (var e in t){
                i += (int) e.Weight;
            }

            Assert.AreEqual(37, i);
        }

        // ReSharper disable once IdentifierTypo
        static LinkedGraph<MinSpanTree.KruskalVertex<string>> BuildKruskalExample(){
            string n = "a,b,c,d,e,f,g,h,i";
            string[] names = n.Split(",");
            int len = names.Length;
            var vertices = new List<MinSpanTree.KruskalVertex<string>>(len);
            for (int i = 0; i < len; i++){
                vertices.Add(new MinSpanTree.KruskalVertex<string>(names[i]));
            }

            var res = new LinkedGraph<MinSpanTree.KruskalVertex<string>>(vertices, GraphDirection.NonDirected);
            int[] indexes1 ={0, 1, 2, 3, 4, 5, 6, 7, 1, 2, 8, 8, 2, 3};
            int[] indexes2 ={1, 2, 3, 4, 5, 6, 7, 0, 7, 8, 7, 6, 5, 5};
            double[] weights ={4, 8, 7, 9, 10, 2, 1, 8, 11, 2, 7, 6, 4, 14};
            len = indexes1.Length;
            for (int i = 0; i < len; i++){
                res.SetNeighbor(vertices[indexes1[i]], vertices[indexes2[i]], weights[i]);
            }

            return res;
        }

        [Test]
        public static void PrimTest(){
            var graph = BuildPrimExample();
            BuildPrimAnswer1();
            BuildPrimAnswer2();
            RunFibonacciHeap(graph);
            graph = BuildPrimExample();
            RunMinHeap(graph);
        }

        static void RunFibonacciHeap(LinkedGraph<MinSpanTree.PrimVertex<string>> graph){
            MinSpanTree.PrimFibonacciHeap(graph, _targetPrimA);
            var vertices = graph.AllVertices();
            var res = new HashSet<HashSet<string>>();
            foreach (var vertex in vertices){
                if (vertex.Parent != null){
                    var t = new HashSet<string>{vertex.Id, vertex.Parent.Id};
                    res.Add(t);
                }
            }

            Assert.True(SetSetEqual(res, _primAnswer1) || SetSetEqual(res, _primAnswer2));
        }

        static void RunMinHeap(LinkedGraph<MinSpanTree.PrimVertex<string>> graph){
            MinSpanTree.PrimMinHeap(graph, _targetPrimA);
            var vertices = graph.AllVertices();
            var res = new HashSet<HashSet<string>>();
            foreach (var vertex in vertices){
                if (vertex.Parent != null){
                    var t = new HashSet<string>{vertex.Id, vertex.Parent.Id};
                    res.Add(t);
                }
            }

            Assert.True(SetSetEqual(res, _primAnswer1) || SetSetEqual(res, _primAnswer2));
        }

        static bool SetSetEqual(HashSet<HashSet<string>> a, HashSet<HashSet<string>> b){
            foreach (var set in a){
                bool contain = false;
                foreach (var setB in b){
                    if (set.SetEquals(setB)){
                        contain = true;
                        break;
                    }
                }

                if (contain == false) return false;
            }

            return true;
        }

        static MinSpanTree.PrimVertex<string> _targetPrimA;

        static LinkedGraph<MinSpanTree.PrimVertex<string>> BuildPrimExample(){
            string n = "a,b,c,d,e,f,g,h,i";
            string[] names = n.Split(",");
            int len = names.Length;
            var vertices = new List<MinSpanTree.PrimVertex<string>>(len);
            for (int i = 0; i < len; i++){
                vertices.Add(new MinSpanTree.PrimVertex<string>(names[i]));
            }

            var res = new LinkedGraph<MinSpanTree.PrimVertex<string>>(vertices, GraphDirection.NonDirected);
            int[] indices1 ={0, 1, 2, 3, 4, 5, 6, 7, 1, 2, 8, 8, 2, 3};
            int[] indices2 ={1, 2, 3, 4, 5, 6, 7, 0, 7, 8, 7, 6, 5, 5};
            double[] weights ={4, 8, 7, 9, 10, 2, 1, 8, 11, 2, 7, 6, 4, 14};
            len = indices1.Length;
            for (int i = 0; i < len; i++){
                res.SetNeighbor(vertices[indices1[i]], vertices[indices2[i]], weights[i]);
            }

            _targetPrimA = vertices[0];
            return res;
        }

        private static HashSet<HashSet<string>> _primAnswer1;

        static void BuildPrimAnswer1(){
            string n = "a,b,c,d,e,f,g,h,i";
            string[] names = n.Split(",");
            int[] indexes1 ={0, 1, 2, 3, 4, 5, 6, 7, 1, 2, 8, 8, 2, 3};
            int[] indexes2 ={1, 2, 3, 4, 5, 6, 7, 0, 7, 8, 7, 6, 5, 5};
            var res = new HashSet<HashSet<string>>();
            int[] answers ={0, 1, 2, 3, 5, 6, 9, 12};
            foreach (int answer in answers){
                var t = new HashSet<string>{names[indexes1[answer]], names[indexes2[answer]]};
                res.Add(t);
            }

            _primAnswer1 = res;
        }

        private static HashSet<HashSet<string>> _primAnswer2;

        static void BuildPrimAnswer2(){
            string n = "a,b,c,d,e,f,g,h,i";
            string[] names = n.Split(",");
            int[] indexes1 ={0, 1, 2, 3, 4, 5, 6, 7, 1, 2, 8, 8, 2, 3};
            int[] indexes2 ={1, 2, 3, 4, 5, 6, 7, 0, 7, 8, 7, 6, 5, 5};
            var res = new HashSet<HashSet<string>>();
            int[] answers ={0, 7, 2, 3, 5, 6, 9, 12};
            foreach (int answer in answers){
                var t = new HashSet<string>{names[indexes1[answer]], names[indexes2[answer]]};
                res.Add(t);
            }

            _primAnswer2 = res;
        }
    }
}