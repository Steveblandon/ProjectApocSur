namespace Projapocsur.Scripts
{
    using System;
    using UnityEngine;

    public interface IMoveable
    {
        event Action OnDestinationReachedEvent;

        MoveDirective CurrentDirective { get; }

        bool HasTarget { get; }

        void LookAt(ITargetable target);

        void StopLookingAtTarget();

        void FollowTarget(ITargetable target, float speed, float distance);

        void MoveForward(float distance, float speed);

        void MoveToMousePosition(float speed);

        void MoveToPosition(Vector3 position, float speed);

        void CancelCurrentDirective();
    }
}