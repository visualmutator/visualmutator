namespace VisualMutator.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;

    public class WorkerCollection<T> where T : class 
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly LinkedList<T> _toProcessList;
        private bool _requestedStop;
        private readonly int _maxCount;
        private int _currentCount;
        private readonly Func<T, Task> _workAction;

        public WorkerCollection(ICollection<T> items, int maxCount, Func<T, Task> workAction)
        {
            _maxCount = maxCount;
            _workAction = workAction;
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

            if(_toProcessList.Count == 0)
            {
                endCallback();
                return;
            }

            while (!emptyStop && !_requestedStop)
            {
                lock (this)
                {
                    while (_currentCount == _maxCount)
                    {
                        Monitor.Wait(this);
                    }
                    _currentCount++;
                }
                
                T item = LockingRemoveFirst();
                if (item != null)
                {
                    Task.Run(() =>
                    {
                        _workAction(item)
                            .ContinueWith(t =>
                            {
                                if(t.Exception != null)
                                {
                                    _log.Error(t.Exception);
                                }
                                lock (this)
                                {
                                    _currentCount--;
                                    Monitor.Pulse(this);

                                    if (_currentCount == 0
                                        && (_requestedStop || _toProcessList.Count == 0))
                                    {
                                        endCallback();
                                    }
                                }
                            });
                    });
                }
                else
                {
                    emptyStop = true;
                    _currentCount--;
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