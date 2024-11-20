using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LuckyProject.Lib.Basics.Collections
{
    public class LpPriorityQueue<T> : IEnumerable<T>, IEnumerable
    {
        #region Internals
        private readonly Dictionary<int, Queue<T>> items = new();
        #endregion

        #region ctors
        public LpPriorityQueue() { }

        public LpPriorityQueue(Dictionary<int, List<T>> items)
        {
            foreach (var kvp in items)
            {
                var queue = new Queue<T>();
                foreach (var item in kvp.Value)
                {
                    queue.Enqueue(item);
                }

                this.items.Add(kvp.Key, queue);
            }
        }
        #endregion

        #region Public interface
        public int TotalCount => items.Sum(kvp => kvp.Value.Count);

        public int Count(int priority)
        {
            return items.TryGetValue(priority, out var queue) ? 0 : queue.Count;
        }

        public void Enqueue(T item, int priority)
        {
            if (!items.TryGetValue(priority, out var queue))
            {
                queue = new Queue<T>();
                items.Add(priority, queue);
            }

            queue.Enqueue(item);
        }

        public T Dequeue(int priority)
        {
            if (!items.TryGetValue(priority, out var queue))
            {
                throw new InvalidOperationException($"Queue with priority {priority} is empty");
            }

            var item = queue.Dequeue();
            if (queue.Count == 0)
            {
                items.Remove(priority);
            }

            return item;
        }

        public T Dequeue()
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException($"PriorityQueue is empty");
            }

            var highestPriority = items.Keys.Min();
            return Dequeue(highestPriority);
        }
        #endregion

        #region IEnumerable
        public IEnumerator<T> GetEnumerator()
        {
            var priorities = items.Keys.ToList();
            priorities.Sort();

            foreach (var priority in priorities)
            {
                var queue = items[priority];
                foreach (var item in queue)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
