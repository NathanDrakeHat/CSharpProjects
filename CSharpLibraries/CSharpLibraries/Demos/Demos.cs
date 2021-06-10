#nullable disable
using System;
using System.IO;
using System.Threading.Tasks;
using Range = System.Range;


// ReSharper disable UnusedMember.Global
// ReSharper disable HeuristicUnreachableCode
// ReSharper disable UnusedMember.Local

namespace CSharpLibraries.Demos{
  // ReSharper disable once UnusedType.Global
  public static class Demos{
    public static void StringFormatDemo(){
      string applesText = "Apples";
      int applesCount = 1234;
      string bananasText = "Bananas";
      int bananasCount = 56789;
      // {idx}
      // {idx,alignment} '-':means left alignment
      // {idx:format} // C: currency, N0: 1,000,000
      // {idx,alignment:format}
      Console.WriteLine("{0,-8} {1,6}", "Name", "Count");
      Console.WriteLine("{0,-8} {1,6:N0}", applesText, applesCount);
      Console.WriteLine("{0,-8} {1,6:N0}", bananasText, bananasCount);

      Console.WriteLine(new string('=', 20));
      int numberOfApples = 12;
      decimal pricePerApple = 0.35M;
      // keyword: format, arg0, arg1, ...
      Console.WriteLine("{0} apples costs {1:C}", numberOfApples, pricePerApple * numberOfApples);
      string formatted = $"{numberOfApples} apples costs {pricePerApple * numberOfApples:C}";
      Console.WriteLine(formatted);
    }

    public static void SwitchAssignmentDemo(){
      string path = @"C:\Users\Nathan\HOME\projects\csharp";
      Stream fs = File.Open(Path.Combine(path, "temp_file.txt"), FileMode.CreateNew);

      string message = fs switch{
        FileStream _ when fs.CanWrite => "The stream is a file that I can write to.",
        FileStream _ => "The stream is a read-only file.",
        MemoryStream _ => "The stream is a memory address.",
        null => "The stream is null.",
        _ => "The stream is some other type."
      };
      Console.WriteLine(message);
    }

    public static void CheckedIntDemo(){
      int x = int.MaxValue - 1;
      unchecked{
        Console.WriteLine($"Initial value: {x}");
        x++;
        Console.WriteLine($"After incrementing: {x}");
        x++;
        Console.WriteLine($"After incrementing: {x}");
        x++;
        Console.WriteLine($"After incrementing: {x}");
      }

      Console.WriteLine();
      try{
        checked{
          x = int.MaxValue - 1;
          Console.WriteLine($"Initial value: {x}");
          x++;
          Console.WriteLine($"After incrementing: {x}");
          // ReSharper disable once IntVariableOverflowInCheckedContext
          x++;
          Console.WriteLine($"After incrementing: {x}");
          x++;
          Console.WriteLine($"After incrementing: {x}");
        }
      }
      catch (OverflowException){
        Console.WriteLine("The code overflowed but I caught the exception.");
      }
    }

    public static void IndexRangeDemo(){
      Index a = ^5;
      Index b = ^2;
      int[] array ={1, 2, 3, 4, 5, 6};
      Range r = new Range(a, b);
      int[] array1 = array[r];
      int[] array2 = array[a..];
      Console.WriteLine($"origin: {string.Join(" ", array)}");
      Console.WriteLine(string.Join(" ", array1));
      Console.WriteLine(string.Join(" ", array2));
    }

    public static void MultiDimensionalArraySample(){
      // ReSharper disable once UnusedVariable
      int[,] matrix ={
        {1, 2, 3},
        {4, 5, 6}
      };
      // ReSharper disable once UnusedVariable
      int[][] jagged ={
        new[]{1, 2},
        new[]{4, 5, 6}
      };
    }

    public static void ParameterModifierDemo(){
      int x = 2;
      // ref: x is initialized
      Pow2(ref x);
      Console.WriteLine(x);

      // out: s initialized by function
      GetPassWord(out var s);
      Console.WriteLine(s);

      var d = new MyData();
      // class: pass reference
      IncreaseData(d);
      Console.WriteLine(d.X);

      var p = new MyPoint();
      // struct: pass value
      IncreasePoint(p);
      Console.WriteLine(p.X);

      // in: readonly
      PrintReadOnly(in d, in p);

      // random args
      Console.WriteLine(Sum(1, 2, 3, 4, 5, 6, 7, 8, 9));

      // named default parameters
      Console.WriteLine(ThreeDimensionPoint(z: 2));

      static void Pow2(ref int x){
        x = x * x;
      }

      static void GetPassWord(out string passWord){
        passWord = "passwd";
      }

      static void IncreaseData(MyData d){
        d.X++;
      }

      static void IncreasePoint(MyPoint p){
        p.X++;
        p.Y++;
      }

      static void PrintReadOnly(in MyData x, in MyPoint p){
        x.X = 3;
        Console.WriteLine($"class can be modified: {x.X}");
        //p.X = 3; // compiler error
        Console.WriteLine($"struct cannot be modified: {p.X}");
      }

      static int Sum(params int[] a){
        int t = 0;
        foreach (var i in a){
          t += i;
        }

        return t;
      }

      static string ThreeDimensionPoint(int x = 0, int y = 0, int z = 0){
        return $"x:{x},y:{y}, z:{z}";
      }
    }

    private struct MyPoint{
      public int X;

      // ReSharper disable once NotAccessedField.Local
      public int Y;
    }

    private sealed class MyData{
      public int X;
      public readonly double Y;

      public MyData(){
      }

      public MyData(int x, int y) => (X, Y) = (x, y);

      public void Deconstruct(out int x, out double y){
        x = X;
        y = Y;
      }
    }

    public static void TypeMatchDemo(object o){
      switch (o){
        case int _:
          Console.WriteLine("is int");
          break;
        case double _:
          Console.WriteLine("is double");
          break;
        case null:
          Console.WriteLine("is null");
          break;
        default:
          Console.WriteLine("other type");
          break;
      }

      throw new Exception();
    }

    public static void DeconstructDemo(){
      var d = new MyData(10, 11);
      var (x, y) = d;
      Console.WriteLine($"{x}, {y}");
    }


    public static void EventDemo(){
      Group group = new Group(1);
      Person bob = new Person(1);
      Person alice = new Person(1);
      group.OnMoneyChanged += (_, arg) => {
        bob.Salary = arg.NewSalary;
        Console.WriteLine($"Bob new salary: {bob.Salary}");
      };
      group.OnMoneyChanged += (_, arg) => {
        alice.Salary = arg.NewSalary;
        Console.WriteLine($"Alice new salary: {alice.Salary}");
      };
      group.Salary = 32;
      group.Salary = 31;
    }

    private sealed class Person{
      public int Salary;

      public Person(int initSalary){
        Salary = initSalary;
      }
    }

    private sealed class MoneyChangedArg : EventArgs{
      public readonly int NewSalary;

      public MoneyChangedArg(int newSalary){
        NewSalary = newSalary;
      }
    }

    private sealed class Group{
      public event EventHandler<MoneyChangedArg> OnMoneyChanged{
        add => _onMoneyChanged = (EventHandler<MoneyChangedArg>) Delegate.Combine(value, _onMoneyChanged);
        remove => _onMoneyChanged = (EventHandler<MoneyChangedArg>) Delegate.Remove(_onMoneyChanged, value);
      }

      private EventHandler<MoneyChangedArg> _onMoneyChanged = delegate{ };

      public int Salary{
        get => _salary;
        set{
          if (_salary != value){
            _salary = value;
            _onMoneyChanged?.Invoke(this, new MoneyChangedArg(value));
          }
        }
      }

      private int _salary;

      public Group(int initSalary){
        _salary = initSalary;
      }
    }

    public static void AsyncDemo(){
      var task = PrintResultAsync();
      Console.WriteLine("This line should be printed first since it's not blocked.");
      task.Wait();
    }

    private static async Task PrintResultAsync(){
      long res = await Task.Run(() => {
        long r = 0;
        for (int i = 0; i < 10000000; i++){
          r += i;
        }

        return r;
      });
      Console.WriteLine($"res:{res}");
    }
  }
}