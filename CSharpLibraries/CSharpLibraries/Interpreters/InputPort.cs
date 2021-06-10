using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CSharpLibraries.Interpreters{
  public sealed class InputPort : IDisposable{
    private readonly TextReader _reader;
    private string _line = "";
    private readonly Queue<string> Queue = new();
    private const string Tokenizer = "\\s*(,@|[('`,)]|\"(?:[\\\\].|[^\\\\\"])*\"|;.*|[^\\s('\"`,;)]*)";
    private static readonly Regex Pattern = new Regex(Tokenizer);

    public InputPort(string s){
      _reader = new StringReader(s);
    }

    public InputPort(TextReader console){
      _reader = console;
    }

    public InputPort(FileInfo file){
      _reader = new StreamReader(file.OpenRead());
    }

    public object NextToken(){
      while (true){
        if (Queue.Count != 0){
          return Queue.Dequeue();
        }
        else if (_line.Equals("")){
          _line = _reader.ReadLine();
        }

        if (_line == null){
          return Symbol.SymEof;
        }
        else if (_line.Equals("")){
          continue;
        }

        var matcher = Pattern.Match(_line);
        while (matcher.Success){
          var token = matcher.Groups[1].Value;
          if (!token.StartsWith(";") && !token.Equals("")){
            Queue.Enqueue(token);
          }

          matcher = matcher.NextMatch();
        }

        _line = "";
      }
    }


    public void Dispose(){
      _reader.Dispose();
    }
  }
}