using System;
using System.Collections.Generic;
using System.Linq;
using CSharpLibraries.Algorithms.Structures;
using NUnit.Framework;
using static CSharpLibraries.Extensions.Extension;

namespace CSharpLibrariesTest.Algorithms.Structures
{
    public static class MinHeapTest
    {
        [Test]
        public static void RandomAddTest()
        {
            for (int i = 0; i < 20; i++){
                var l = ShuffledArithmeticSequence(1,128,2);
                var m = new MinHeap<int,string>((a,b)=>a-b);
                foreach (var integer in l) {
                    m.Add(integer,integer.ToString());
                }
                var res = new List<int>();
                while (m.HeapSize > 0){
                    res.Add(int.Parse(m.ExtractMin()));
                }
                for(int j = 0; j < res.Count - 1; j++){
                    if(res[j] <= res[j+1])
                    {
                        Assert.True(true);
                    }
                    else{
                        Console.WriteLine(res);
                        Assert.Fail();
                    }
                }
            }
        }

        [Test]
        public static void RandomUpdateKeyTest()
        {
            for (int i = 0; i < 10; i++) {
                var l = ShuffledArithmeticSequence(1, 128,2);
                var rand = new Random();
                var heap = new MinHeap<int, string>(
                    l.Select(e=>e.ToString()).ToList(),
                    s => rand.Next(127 - 1) + 1,
                    (a,b)=>a-b);
                var res = new List<int>();
                foreach(var elem in l){
                    heap.UpdateKey(elem.ToString(),elem);
                }
                while (heap.HeapSize > 0) {
                    res.Add(int.Parse(heap.ExtractMin()));
                }
                for (int j = 0; j < res.Count - 1; j++) {
                    if (res[j].CompareTo(res[j + 1]) <= 0) {
                        Assert.True(true);
                    }
                    else {
                        Assert.Fail();
                    }
                }
            }
        }
    }
}