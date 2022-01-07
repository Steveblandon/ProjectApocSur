namespace Projapocsur.Engine
{
    using System.Collections;
    using UnityEngine;

    public interface ICoroutineHandler
    {
        public Coroutine LatestActiveCoroutine { get; }

        Coroutine StartCoroutine(IEnumerator routine);

        void StopCoroutine(Coroutine routine);  //NOTE: to be able to stop coroutine individually, must use string or Coroutine type, IEnumerator as input won't work.

        WaitForSeconds Wait(float seconds);
    }
}
