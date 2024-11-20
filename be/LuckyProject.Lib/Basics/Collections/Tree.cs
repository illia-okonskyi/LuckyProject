using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Collections
{
    #region Interfaces
    public interface ITreeNode<T>
        where T : ITreeNode<T>
    {
        List<T> Childs { get; }
    }

    public interface ITreeNodeWithParent<T> : ITreeNode<T>
        where T : ITreeNode<T>
    {
        T Parent { get; }
    }
    #endregion

    #region Extensions
    public static class TreeExtensions
    {
        public static IEnumerable<(T Parent, T Item)> EnumerateInWidth<T>(this ITreeNode<T> root)
            where T : ITreeNode<T>
        {
            var q = new Queue<(ITreeNode<T>, ITreeNode<T>)>();
            q.Enqueue((null, root));
            while (q.Count > 0)
            {
                var (parent, item) = q.Dequeue();
                if (item.Childs != null && item.Childs.Count > 0)
                {
                    item.Childs.ForEach(i => q.Enqueue((item, i)));
                }
                yield return ((T)parent, (T)item);
            }
        }

        public static IEnumerable<(T Parent, T Item)> EnumerateInDepth<T>(this ITreeNode<T> root)
            where T : ITreeNode<T>
        {
            var s = new Stack<(ITreeNode<T>, ITreeNode<T>)>();
            s.Push((null, root));
            while (s.Count > 0)
            {
                var (parent, item) = s.Pop();
                if (item.Childs != null && item.Childs.Count > 0)
                {
                    item.Childs.ForEach(i => s.Push((item, i)));
                }
                yield return ((T)parent, (T)item);
            }
        }

        public static IEnumerable<(T Parent, T Item)> EnumerateParents<T>(
            this ITreeNodeWithParent<T> root)
            where T : ITreeNodeWithParent<T>
        {
            var item = root;
            while (item.Parent != null)
            {
                yield return ((T)item.Parent, (T)item);
                item = item.Parent;
            }
        }
    }
    #endregion
}
