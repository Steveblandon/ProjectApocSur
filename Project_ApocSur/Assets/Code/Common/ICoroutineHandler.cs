namespace Projapocsur
{
    using System.Collections;
    using UnityEngine;

    public interface ICoroutineHandler
    {
        Coroutine StartCoroutine(IEnumerator routine);

        void StopCoroutine(Coroutine routine);  //NOTE: to be able to stop coroutine individually, must use string or Coroutine type, IEnumerator as input won't work.
    }
}
