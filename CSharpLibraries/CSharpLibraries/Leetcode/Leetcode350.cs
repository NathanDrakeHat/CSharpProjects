using System.Collections.Generic;
using System.Linq;

namespace CSharpLibraries.Leetcode
{
    public static class Leetcode350
    {
        /// <summary>
        /// #328
        /// <br/>奇偶链表
        /// <br/>输入: 1->2->3->4->5->NULL
        /// <br/>输出: 1->3->5->2->4->NULL
        /// <br/>给定一个单链表，把所有的奇数节点和偶数节点分别排在一起。
        /// <br/>请注意，这里的奇数节点和偶数节点指的是节点编号的奇偶性，
        /// <br/>而不是节点的值的奇偶性。
        /// <br/>应当保持奇数节点和偶数节点的相对顺序。
        /// <br/>链表的第一个节点视为奇数节点，第二个节点视为偶数节点，以此类推。
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        public static ListNode OddEvenList(ListNode head)
        {
            if (head == null)
            {
                return null;
            }
            ListNode oddTailPtr = head, evenTailPtr = head.next;

            if (evenTailPtr?.next == null)
            {
                return head;
            }

            while (evenTailPtr?.next != null)
            {
                var t = ExtractAndJumpToEvenTail(ref evenTailPtr);
                if (t != null)
                {
                    InsertAndJumpToOddTail(ref oddTailPtr, t);
                }
            }
            
            return head;

            //
            static void InsertAndJumpToOddTail(ref ListNode list, ListNode n)
            {
                n.next = list.next;
                list.next = n;
                list = n;
            }

            static ListNode ExtractAndJumpToEvenTail(ref ListNode n)
            {
                var target = n.next;
                var origin = n;
                n = n?.next?.next;
                origin.next = n;
                return target;
            }
        }
        
        /// <summary>
        /// #349
        /// <br/>
        /// 两个数组的交集
        /// </summary>
        /// <param name="nums1"></param>
        /// <param name="nums2"></param>
        /// <returns></returns>
        public static int[] Intersection(int[] nums1, int[] nums2)
        {
            // TODO better solution
            var set = new HashSet<int>(nums1);
            var res = new HashSet<int>();
            foreach (int i in nums2)
            {
                if (set.Contains(i))
                {
                    res.Add(i);
                }
            }

            return res.ToArray();
        }
    }
}
