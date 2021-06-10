using System.Collections.Generic;
using CSharpLibraries.Algorithms.Graph;
using NUnit.Framework;

namespace CSharpLibrariesTest.Algorithms.Graph{
  public static class BfsTest{
    private static class Data{
      // ReSharper disable once StringLiteralTypo
      private static string names = "rstuvwxy";

      public static List<Bfs.BfsVertex<char>> MakeVertexes(){
        var vs = new List<Bfs.BfsVertex<char>>(8);
        for (int i = 0; i < 8; i++){
          vs.Add(new Bfs.BfsVertex<char>(names[i]));
        }

        return vs;
      }

      public static LinkedGraph<Bfs.BfsVertex<char>> MakeGraph(List<Bfs.BfsVertex<char>> vs){
        var graph = new LinkedGraph<Bfs.BfsVertex<char>>(vs, GraphDirection.NonDirected);
        graph.SetNeighbor(vs[0], vs[1]);
        graph.SetNeighbor(vs[0], vs[4]);

        graph.SetNeighbor(vs[1], vs[5]);

        graph.SetNeighbor(vs[2], vs[3]);
        graph.SetNeighbor(vs[2], vs[5]);
        graph.SetNeighbor(vs[2], vs[6]);

        graph.SetNeighbor(vs[3], vs[6]);

        graph.SetNeighbor(vs[5], vs[6]);

        graph.SetNeighbor(vs[6], vs[7]);

        return graph;
      }
    }

    [Test]
    public static void BreathFirstSearchTest(){
      var vs = Data.MakeVertexes();
      var t = Data.MakeGraph(vs);
      Bfs.BreathFirstSearch(t, vs[1]);
      Assert.AreEqual(new List<char>{
        's', 'w', 'x', 'y'
      }, Bfs.GetPath(vs[1], vs[7]));
    }
  }
}