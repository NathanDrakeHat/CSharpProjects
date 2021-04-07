using System.Collections.Generic;

namespace CSharpLibraries.Interpreters{
    internal static class Utils{
        internal static void AddRange(this IList<object> l, IEnumerable<object> other){
            if (l is List<object> ll){
                ll.AddRange(other);
            }
            else{
                foreach (var o in other){
                    l.Add(o);
                }
            }
        }
    }
}