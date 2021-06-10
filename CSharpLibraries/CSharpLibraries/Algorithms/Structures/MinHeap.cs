#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using static CSharpLibraries.Utils.Extension;

namespace CSharpLibraries.Algorithms.Structures{
  public sealed class MinHeap<TKey, TValue> : IEnumerable<TValue>{
    #region InnerClass

    private sealed class Node{
      internal TKey Key;
      internal readonly TValue Value;
      internal int Index;

      internal Node(TKey key, TValue value, int index){
        Key = key;
        Value = value;
        Index = index;
      }

      public override string ToString() => $"Node(key: {Key}, value: {Value}), index:{Index}";
    }

    #endregion

    private readonly List<Node> _array = new();
    private readonly Dictionary<TValue, Node> _valueToNodeMap = new();
    public int HeapSize => _array.Count;
    private readonly Func<TKey, TKey, int> _keyComparer;

    public MinHeap(Func<TKey, TKey, int> keyComparer){
      _keyComparer = keyComparer;
    }

    public MinHeap(IEnumerable<TValue> values, Func<TValue, TKey> getKey, Func<TKey, TKey, int> keyComparer){
      _keyComparer = keyComparer ?? throw new ArgumentNullException(nameof(keyComparer));
      values.RequireNotNullArg(nameof(values));
      getKey.RequireNotNullArg(nameof(getKey));
      foreach (var i in values){
        if (i == null) throw new ArgumentException("null in container", nameof(values));
        if (_valueToNodeMap.ContainsKey(i)) throw new ArgumentException("duplicate value", nameof(values));
        var n = new Node(getKey(i), i, HeapSize);
        _array.Add(n);
        _valueToNodeMap[n.Value!] = n;
      }

      BuildMinHeap();
    }

    public TValue ExtractMin(){
      if (HeapSize <= 0) throw new InvalidOperationException("Null heap.");
      var res = _array[0];
      UpdateArrayAndNode(0, _array[HeapSize - 1]);
      _array.RemoveAt(HeapSize - 1);
      MinHeapify(0);
      _valueToNodeMap.Remove(res.Value!);
      return res.Value;
    }

    public void UpdateKey(TValue value, TKey newKey){
      value.RequireNotNullArg(nameof(value));
      newKey.RequireNotNullArg(nameof(newKey));
      var node = _valueToNodeMap[value!];
      if (node == null) throw new InvalidOperationException("No such value.");
      if (_keyComparer(newKey, node.Key) < 0){
        node.Key = newKey;
        DecreaseKey(node.Index);
      }
      else if (_keyComparer(newKey, node.Key) > 0){
        node.Key = newKey;
        MinHeapify(node.Index);
      }
    }

    /// <summary>
    /// use when only one key is updated lower or add in the last of array
    /// </summary>
    /// <param name="idx"></param>
    private void DecreaseKey(int idx){
      while (idx > 0 && _keyComparer(_array[Parent(idx)].Key, _array[idx].Key) > 0){
        var t = _array[idx];
        UpdateArrayAndNode(idx, _array[Parent(idx)]);
        UpdateArrayAndNode(Parent(idx), t);
        idx = Parent(idx);
      }
    }

    public bool Contains(TValue value) =>
      _valueToNodeMap.ContainsKey(value ?? throw new ArgumentNullException(nameof(value)));

    public void Add(TKey key, TValue value){
      key.RequireNotNullArg(nameof(key));
      value.RequireNotNullArg(nameof(value));
      if (_valueToNodeMap.ContainsKey(value!)) throw new ArgumentException("duplicate value", nameof(value));

      var n = new Node(key, value, HeapSize);
      _array.Add(n);
      _valueToNodeMap[value] = n;
      DecreaseKey(HeapSize - 1);
    }

    private void MinHeapify(int idx){
      while (true){
        int lIdx = Left(idx);
        int rIdx = Right(idx);
        int minIdx = idx;
        if (lIdx < HeapSize && _keyComparer(_array[lIdx].Key, _array[minIdx].Key) < 0){
          minIdx = lIdx;
        }

        if (rIdx < HeapSize && _keyComparer(_array[rIdx].Key, _array[minIdx].Key) < 0){
          minIdx = rIdx;
        }

        if (minIdx != idx){
          var t = _array[minIdx];
          UpdateArrayAndNode(minIdx, _array[idx]);
          UpdateArrayAndNode(idx, t);
          idx = minIdx;
          continue;
        }

        break;
      }
    }

    private void UpdateArrayAndNode(int index, Node node){
      _array[index] = node;
      node.Index = index;
    }

    /// <summary>
    /// init MinHeap from middle of array
    /// </summary>
    private void BuildMinHeap(){
      for (int i = Parent(HeapSize - 1); i >= 0; i--)
        MinHeapify(i);
    }

    private static int Parent(int idx) => (idx + 1) / 2 - 1;
    private static int Left(int idx) => 2 * (idx + 1) - 1;
    private static int Right(int idx) => 2 * (idx + 1);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<TValue> GetEnumerator(){
      while (HeapSize > 0){
        yield return ExtractMin();
      }
    }
  }
}