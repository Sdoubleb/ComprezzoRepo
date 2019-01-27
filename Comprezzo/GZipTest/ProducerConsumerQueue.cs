﻿using System.Collections.Generic;

namespace GZipTest
{
    class ProducerConsumerQueue<T>
    {
        private Queue<T> _producerQueue = new Queue<T>();
        private Queue<T> _consumerQueue = new Queue<T>();

        private object _locker = new object();

        public void Enqueue(T element)
        {
            lock (_locker)
            {
                _producerQueue.Enqueue(element);
            }
        }

        public bool TryDequeue(out T element)
        {
            if (_consumerQueue.Count == 0)
            {
                lock (_locker)
                {
                    while (_producerQueue.Count > 0)
                        _consumerQueue.Enqueue(_producerQueue.Dequeue());
                }
            }
            if (_consumerQueue.Count > 0)
            {
                element = _consumerQueue.Dequeue();
                return true;
            }
            element = default(T);
            return false;
        }
    }
}