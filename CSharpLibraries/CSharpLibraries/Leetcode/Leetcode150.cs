#nullable disable
using System;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
namespace CSharpLibraries.Leetcode{
    public static class Leetcode150{
        /// <summary>
        /// #111
        /// <br/>给定一个二叉树，找出其最小深度。
        /// </summary>
        /// <param name="root">二叉树</param>
        /// <returns>最小深度</returns>
        public static int MinDepth(TreeNode root){
            if (root == null) return 0;
            int left = MinDepth(root.left);
            int right = MinDepth(root.right);
            int depth = 1;
            switch (left){
                case 0:
                    depth += right;
                    break;
                default:
                    switch (right){
                        case 0:
                            depth += left;
                            break;
                        default:
                            depth += Math.Min(left, right);
                            break;
                    }

                    break;
            }

            return depth;
        }
    }
}