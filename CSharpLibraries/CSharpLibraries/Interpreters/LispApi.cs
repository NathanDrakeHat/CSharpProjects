using System;

namespace CSharpLibraries.Interpreters{
    public sealed partial class Lisp{
        public object EvalScripts(string program) => Eval(Parse(program), _globalEnv);

        public void Repl(string prompt = "NScheme>"){
            while (true){
                var s = ReadLine(prompt);
                if (s.Equals("")){
                    continue;
                }

                object val = null;
                try{
                    val = Eval(Parse(s), _globalEnv);
                }
                catch (Exception e){
                    Console.WriteLine($"{e.GetType().Name}: {e.Message}\n");
                }

                if (val != null){
                    Console.WriteLine(val);
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

       
    }
}