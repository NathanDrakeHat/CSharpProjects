using System;
using System.IO;
using static CSharpLibraries.Interpreters.Symbol;

namespace CSharpLibraries.Interpreters{
    public sealed partial class Lisp{
        public void Repl(){
            const string prompt = "lisp>";
            InputPort inPort = new InputPort(Console.In);
            Console.Out.WriteLineAsync("Lispy version 2.0").Wait();
            while (true){
                try{
                    Console.Out.WriteLineAsync(prompt).Wait();
                    var x = Parse(inPort);
                    if (x == null){
                        continue;
                    }
                    else if (x.Equals(SymEof)){
                        continue;
                    }

                    EvalAndPrint(x);
                }
                catch (Exception e){
                    Console.Error.WriteLineAsync(e.StackTrace).Wait();
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public void RunFile(FileInfo file){
            using var inPort = new InputPort(file);
            while (true){
                try{
                    var x = Parse(inPort);
                    if (x == null){
                        continue;
                    }
                    else if (x.Equals(SymEof)){
                        return;
                    }

                    EvalAndPrint(x);
                }
                catch (Exception e){
                    Console.Error.WriteLineAsync(e.StackTrace).Wait();
                }
            }
        }

        public object EvalScripts(string program){
            return Eval(Parse(program), _globalEnv);
        }

        public void LoadLib(FileInfo file){
            using var inPort = new InputPort(file);
            while (true){
                try{
                    var x = Parse(inPort);
                    if (x == null){
                        continue;
                    }
                    else if (x.Equals(SymEof)){
                        return;
                    }

                    Eval(x, _globalEnv);
                }
                catch (Exception e){
                    Console.Error.WriteLineAsync(e.StackTrace).Wait();
                }
            }
        }
    }
}