namespace Projapocsur.Scripts
{
    using System;
    using UnityEngine;

    public interface IMoveable
    {
        event Action OnDestinationReachedEvent;

        void LookAt(ITargetable target);

        void MoveForward(float distance, float speed);

        void MoveToMousePosition(float speed);

        void MoveToPosition(Vector3 position, float speed);

        void StopLookingAtTarget();
    }
}