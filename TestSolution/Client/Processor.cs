using System;
using System.Collections.Concurrent;

namespace Client
{
    public class Processor
    {
        private readonly BlockingCollection<Action> _processingQueue = new BlockingCollection<Action>();

        public void Enqueue(Action action)
        {
            _processingQueue.Add(action);
        }

        public void Start()
        {
            foreach (var action in _processingQueue.GetConsumingEnumerable())
                action();
        }
    }
}
