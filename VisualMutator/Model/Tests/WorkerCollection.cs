namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
//
//    public class WorkerCollection2<T> where T : class
//    {
//        private bool _requestedStop;
//        private readonly SemaphoreSlim _semaphore;
//        private readonly int _maxCount;
//        private readonly Action<T> _workAction;
//
//        public WorkerCollection2(int maxCount, Action<T> workAction)
//        {
//            _maxCount = maxCount;
//            _workAction = workAction;
//            _semaphore = new SemaphoreSlim(_maxCount);
//        }
//
//        public void Stop()
//        {
//            _requestedStop = true;
//        }
//
//        public void Start(Action endCallback)
//        {
//            _requestedStop = false;
//
//            while (!_requestedStop)
//            {
//                _semaphore.Wait();
//                    Task.Run(() => _workAction())
//                        .ContinueWith(t =>
//                        {
//                            _semaphore.Release();
//                            lock (this)
//                            {
//                                if (_semaphore.CurrentCount == _maxCount
//                                    && (_requestedStop ))
//                                {
//                                    endCallback();
//                                }
//                            }
//
//                        });
//            }
//        }
//
//    }

    public class WorkerCollection<T> where T : class 
    {
        private readonly LinkedList<T> _toProcessList;
        private bool _requestedStop;
        private readonly SemaphoreSlim _semaphore;
        private readonly int _maxCount;
        private readonly Action<T> _workAction;

        public WorkerCollection(ICollection<T> items, int maxCount, Action<T> workAction)
        {
            _maxCount = maxCount;
            _workAction = workAction;
            _semaphore = new SemaphoreSlim(_maxCount);
            _toProcessList = new LinkedList<T>(items);
        }

        public void Stop()
        {
            _requestedStop = true;
        }

        public void Start(Action endCallback)
        {
            _requestedStop = false;

            bool emptyStop = false;
            while (!emptyStop && !_requestedStop)
            {
                _semaphore.Wait();
                T item = LockingRemoveFirst();
                if (item != null)
                {
                    Task.Run(() => _workAction(item))
                        .ContinueWith(t =>
                        {
                            _semaphore.Release();
                            lock (this)
                            {
                                if (_semaphore.CurrentCount == _maxCount
                                    && (_requestedStop || _toProcessList.Count == 0))
                                {
                                    endCallback();
                                }
                            }

                        });
                }
                else
                {
                    emptyStop = true;
                    _semaphore.Release();
                }
            }
        }


        private T LockingRemoveFirst()
        {
            lock (_toProcessList)
            {
                T toReturn = _toProcessList.Count != 0 ? _toProcessList.First.Value : null;
                if (toReturn != null)
                {
                    _toProcessList.RemoveFirst();
                }
                return toReturn;
            }
        }

        public void LockingMoveToFront(T mutant)
        {
            lock (_toProcessList)
            {
                if (_toProcessList.Remove(mutant))
                {
                    _toProcessList.AddFirst(mutant);
                }
            }
        }

        public void Enqueue(T mutant)
        {
            lock (_toProcessList)
            {
                _toProcessList.AddLast(mutant);
            }
        }
    }
}