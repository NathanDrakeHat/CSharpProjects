using System;
using System.Diagnostics;
using System.IO;
using CSharpLibraries.Interpreters;

namespace CSharpLibraries{
    public static class Program{
        public static void Main(){
            var w = new Stopwatch();
            w.Start();
            var lisp = new Lisp();
            lisp.LoadLib(new FileInfo("resources/functions.ss"));
            lisp.RunFile(new FileInfo("resources/lispytest.ss"));
            Console.WriteLine($"finish: {w.ElapsedTicks / (double) Stopwatch.Frequency * 1000}ms");
        }
    }
}