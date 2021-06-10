using System.Collections.Generic;
using CSharpLibraries.Algorithms.Structures;
using NUnit.Framework;

namespace CSharpLibrariesTest.Algorithms.Structures{
  public static class FibonacciHeapTest{
    static void AddChild(FibonacciHeap<double, double>.Node n, int t){
      var x = new FibonacciHeap<double, double>.Node(t, t){Mark = false};
      var listLeft = n.Left;
      n.Left = x;
      x.Right = n;
      listLeft.Right = x;
      x.Left = listLeft;
      x.Parent = n.Parent;
    }

    static void AddChildren(FibonacciHeap<double, double>.Node n, params int[] t){
      foreach (var i in t){
        AddChild(n, i);
      }
    }

    private static FibonacciHeap<double, double> BuildExample(){
      var heap = new FibonacciHeap<double, double>(
        (a, b) => a - b > 0 ? 1 : a - b < 0 ? -1 : 0);
      heap.Insert(3, 3.0);
      var m = heap.RootList;
      AddChildren(m, 17, 24, 23, 7, 21);

      var ptr = new FibonacciHeap<double, double>.Node(18, 0){Mark = true};
      m.ChildList = ptr;
      ptr.Parent = m;
      m.Degree = 2;

      var mChild = m.ChildList;
      mChild.Degree = 1;
      ptr = new FibonacciHeap<double, double>.Node(39, 0){Mark = true};
      mChild.ChildList = ptr;
      ptr.Parent = mChild;
      AddChildren(mChild, 52, 38);

      ptr = new FibonacciHeap<double, double>.Node(41, 0);
      mChild.Left.ChildList = ptr;
      ptr.Parent = mChild.Left;
      mChild.Left.Degree = 1;

      ptr = new FibonacciHeap<double, double>.Node(30, 0);
      m.Right.ChildList = ptr;
      ptr.Parent = m.Right;
      m.Right.Degree = 1;

      ptr = new FibonacciHeap<double, double>.Node(35, 0);
      var t = new FibonacciHeap<double, double>.Node(26, 0){
        Mark = true,
        Degree = 1,
        ChildList = ptr
      };
      ptr.Parent = t;
      m.Right.Right.ChildList = t;
      t.Parent = m.Right.Right;

      m.Right.Right.Degree = 2;
      AddChild(t, 46);
      heap.Count = 15;
      return heap;
    }

    private static IList<double> Bcl(FibonacciHeap<double, double>.Node t){
      var res = new List<double>();
      var p = t;
      do{
        res.Add(p.Key);
        p = p.Right;
      } while (!ReferenceEquals(p, t));

      p = t;
      do{
        res.Add(p.Key);
        p = p.Left;
      } while (!ReferenceEquals(p, t));

      return res;
    }

    [Test]
    public static void Example1(){
      var heap = BuildExample();
      double o = heap.ExtractMin();
      Assert.AreEqual(o, 3.0);
      //see <<introduction to  algorithm>> to find this test sample.
      Assert.AreEqual(new List<double>{7.0, 18.0, 38.0, 7.0, 38.0, 18.0}, Bcl(heap.RootList));
      Assert.AreEqual(new List<double>{39.0, 21.0, 39.0, 21.0}, Bcl(heap.RootList.Right.ChildList));
      Assert.AreEqual(new List<double>{52.0, 52.0}, Bcl(heap.RootList.Right.ChildList.Left.ChildList));
      Assert.AreEqual(new List<double>{23.0, 17.0, 24.0, 23.0, 24.0, 17.0}, Bcl(heap.RootList.ChildList));
      Assert.AreEqual(new List<double>{26.0, 46.0, 26.0, 46.0}, Bcl(heap.RootList.ChildList.Left.ChildList));
      Assert.AreEqual(new List<double>{30.0, 30.0}, Bcl(heap.RootList.ChildList.Left.Left.ChildList));
      Assert.AreEqual(new List<double>{35.0, 35.0}, Bcl(heap.RootList.ChildList.Left.ChildList.ChildList));
      Assert.AreEqual(new List<double>{41.0, 41.0}, Bcl(heap.RootList.Right.Right.ChildList));

      heap.DecreaseKey(heap.RootList.ChildList.Left.ChildList.Left, 15);
      heap.DecreaseKey(heap.RootList.ChildList.Left.ChildList.ChildList, 5);

      Assert.AreEqual(
        new List<double>{5.0, 26.0, 24.0, 7.0, 18.0, 38.0, 15.0, 5.0, 15.0, 38.0, 18.0, 7.0, 24.0, 26.0},
        Bcl(heap.RootList));
      Assert.AreEqual(new List<double>{23.0, 17.0, 23.0, 17.0}, Bcl(heap.RootList.Right.Right.Right.ChildList));
      Assert.AreEqual(new List<double>{30.0, 30.0},
        Bcl(heap.RootList.Right.Right.Right.ChildList.Right.ChildList));
      Assert.AreEqual(new List<double>{39.0, 21.0, 39.0, 21.0},
        Bcl(heap.RootList.Right.Right.Right.Right.ChildList));
    }
  }
}