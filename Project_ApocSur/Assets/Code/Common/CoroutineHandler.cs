namespace Projapocsur
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class CoroutineHandler : ICoroutineHandler
    {
        private Func<IEnumerator, Coroutine> startCoroutine;
        private Action<IEnumerator> stopCoroutine;

        public CoroutineHandler(Func<IEnumerator, Coroutine> startCoroutine, Action<IEnumerator> stopCoroutine)
        {
            this.startCoroutine = startCoroutine;
            this.stopCoroutine = stopCoroutine;
        }

        public Coroutine StartCoroutine(IEnumerator routine) => this.startCoroutine(routine);

        public void StopCoroutine(IEnumerator routine) => this.stopCoroutine(routine);

        public WaitForSeconds WaitForSeconds(float seconds) => new WaitForSeconds(seconds);
    }
}
