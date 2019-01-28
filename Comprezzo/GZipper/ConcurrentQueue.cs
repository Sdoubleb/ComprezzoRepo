using System.Collections.Generic;

namespace GZipper
{
    class ConcurrentQueue<T>
    {
        private Queue<T> _queue = new Queue<T>();

        private object _locker = new object();

        public void Enqueue(T element)
        {
            lock (_locker)
            {
                _queue.Enqueue(element);
            }
        }

        public bool TryDequeue(out T element)
        {
            lock (_locker)
            {
                if (_queue.Count > 0)
                {
                    element = _queue.Dequeue();
                    return true;
                }
            }
            element = default(T);
            return false;
        }
    }
}