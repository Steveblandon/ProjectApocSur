namespace Projapocsur.Engine
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class CoroutineHandler : ICoroutineHandler
    {
        private Func<IEnumerator, Coroutine> startCoroutine;
        private Action<Coroutine> stopCoroutine;

        public CoroutineHandler(Func<IEnumerator, Coroutine> startCoroutine, Action<Coroutine> stopCoroutine)
        {
            this.startCoroutine = startCoroutine;
            this.stopCoroutine = stopCoroutine;
        }

        public Coroutine LatestActiveCoroutine { get; protected set; }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            Coroutine coroutine = this.startCoroutine(routine);
            this.LatestActiveCoroutine = coroutine;
            return coroutine;
        }

        public void StopCoroutine(Coroutine routine)
        {
            this.stopCoroutine(routine);

            if (routine == this.LatestActiveCoroutine)
            {
                this.LatestActiveCoroutine = null;
            }
        }

        public WaitForSeconds Wait(float seconds) => new WaitForSeconds(seconds);
    }
}
