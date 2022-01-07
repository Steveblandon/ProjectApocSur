namespace Projapocsur.Engine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class QueuedRoutineExecutor : MonoBehaviour
    {
        private bool isBusy;
        private Queue<(Func<Func<bool>, IEnumerator>, Func<bool>)> queuedRoutine = new Queue<(Func<Func<bool>, IEnumerator>, Func<bool>)>();

        protected void StartOrQueueRoutine(Func<Func<bool>, IEnumerator> coroutine, Func<bool> abortCallback)
        {
            this.queuedRoutine.Enqueue((coroutine, abortCallback));

            if (!this.isBusy)
            {
                this.StartCoroutine(this.RunQueuedRoutines());
            }
        }

        protected IEnumerator RunQueuedRoutines()
        {
            this.isBusy = true;

            while (this.queuedRoutine.Count > 0)
            {
                var(coroutine, abortCallback) = this.queuedRoutine.Dequeue();
                yield return this.StartCoroutine(coroutine(abortCallback));
            }

            this.isBusy = false;
        }
    }
}
