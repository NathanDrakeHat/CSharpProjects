using System;
using System.IO;
using static CSharpLibraries.Interpreters.Symbol;

namespace CSharpLibraries.Interpreters{
    public sealed partial class Lisp{
        public void Repl(){
            const string prompt = "lisp>";
            InputPort inPort = new InputPort(Console.In);
            Console.WriteLine("Lispy version 2.0");
            while (true){
                try{
                    Console.Write(prompt);
                    var x = Parse(inPort);
                    if (x == null){
                        continue;
                    }
                    else if (x.Equals(SymEof)){
                        continue;
                    }

                    EvalAndPrint(x, true);
                }
                catch (Exception e){
                    Console.Error.WriteLine(e.Message);
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
                        ConsoleBuffer.Flush();
                        return;
                    }

                    EvalAndPrint(x);
                }
                catch (Exception e){
                    ConsoleBuffer.WriteLine(e.Message);
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
                        ConsoleBuffer.Flush();
                        return;
                    }

                    Eval(x, _globalEnv);
                }
                catch (Exception e){
                    ConsoleBuffer.WriteLine(e.Message);
                }
            }
        }
    }
}