namespace Projapocsur
{
    using System.Collections;
    using UnityEngine;

    public interface ICoroutineHandler
    {
        Coroutine StartCoroutine(IEnumerator routine);

        void StopCoroutine(IEnumerator routine);
    }
}
