namespace Projapocsur
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

        public Coroutine StartCoroutine(IEnumerator routine) => this.startCoroutine(routine);

        public void StopCoroutine(Coroutine routine) => this.stopCoroutine(routine);

        public WaitForSeconds WaitForSeconds(float seconds) => new WaitForSeconds(seconds);
    }
}
