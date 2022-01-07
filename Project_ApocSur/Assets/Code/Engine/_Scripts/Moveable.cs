namespace Projapocsur.Engine
{
    using System;
    using UnityEngine;

    public enum MoveDirective
    {
        None,
        MoveToDestination,
        MoveForward,
        FollowTarget
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class Moveable : MonoBehaviour, IMoveable
    {
        public event Action OnDestinationReachedEvent;

        private const string CompName = nameof(Moveable);

        private Vector3 startingPoint;
        private Vector3 destination;
        private float speed;
        private float distanceToTravel;
        private ITargetable target;
        private float maxDistanceFromTarget;
        private SpriteRenderer spriteRenderer;

        public bool IsTargetWithinDistance { get; private set; }

        public MoveDirective CurrentDirective { get; private set; }

        public bool HasTarget => target != null;

        void Start()
        {
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
            this.CancelCurrentDirective();
        }

        void Update()
        {
            if (this.target != null)
            {
                this.transform.rotation = Quaternion.LookRotation(Vector3.forward, this.target.Position - this.transform.position);
            }

            switch (this.CurrentDirective)
            {
                case MoveDirective.MoveToDestination:
                    this.MoveToDestinationUpdate();
                    break;
                case MoveDirective.MoveForward:
                    this.MoveForwardUpdate();
                    break;
                case MoveDirective.FollowTarget:
                    this.FollowTargetUpdate();
                    break;
            }
        }

        public void LookAt(ITargetable target) => this.target = target;

        public void StopLookingAtTarget() => this.target = null;

        public void FollowTarget(ITargetable target, float speed)
        {
            this.CancelCurrentDirective();
            this.maxDistanceFromTarget = (this.spriteRenderer.bounds.size.x + target.Size.x) / 2 + GameMaster.Instance.Config.TouchingDistance;
            this.target = target;
            this.speed = speed;
            this.CurrentDirective = MoveDirective.FollowTarget;
        }

        public void MoveForward(float distance, float speed)
        {
            this.CancelCurrentDirective();
            this.startingPoint = this.transform.position;
            this.speed = speed;
            this.distanceToTravel = distance;
            this.CurrentDirective = MoveDirective.MoveForward;
            Debug.Log($"{this.name} moving forward for a distance of {distance}");
        }

        public void MoveToPosition(Vector3 position, float speed)
        {
            this.CancelCurrentDirective();
            this.startingPoint = this.transform.position;
            this.speed = speed;
            this.destination = position;
            this.CurrentDirective = MoveDirective.MoveToDestination;
            Debug.Log($"{this.name} moving from {this.transform.position} to {this.destination} [current Up direction: {this.transform.up}]");
        }

        public void MoveToMousePosition(float speed)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPoint.z = this.transform.position.z;
            this.MoveToPosition(worldPoint, speed);
        }

        public void CancelCurrentDirective()
        {
            this.CurrentDirective = MoveDirective.None;
            this.distanceToTravel = 0;
            this.maxDistanceFromTarget = 0;
            this.destination = this.transform.position;
        }

        private void MoveToDestinationUpdate()
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.destination, this.speed * Time.deltaTime);

            if (this.transform.position == this.destination)
            {
                this.CancelCurrentDirective();
                this.OnDestinationReachedEvent?.Invoke();
            }
            else if (this.target == null)
            {
                this.transform.rotation = Quaternion.LookRotation(Vector3.forward, this.destination - this.transform.position);
            }
        }

        private void MoveForwardUpdate()
        {
            this.transform.Translate(this.transform.up * this.speed * Time.deltaTime, Space.World);    // NOTE: must be Space.World to have it account for rotation, otherwise it won't move in the proper direction.
            float distanceTravelled = Vector3.Distance(startingPoint, this.transform.position);
            
            if (distanceTravelled >= distanceToTravel)
            {
                this.CancelCurrentDirective();
                this.OnDestinationReachedEvent?.Invoke();
            }
        }

        private void FollowTargetUpdate()
        {
            float distanceFromTarget = Vector3.Distance(this.transform.position, this.target.Position);

            if (distanceFromTarget > this.maxDistanceFromTarget)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, this.target.Position, this.speed * Time.deltaTime);
                this.IsTargetWithinDistance = false;
            }
            else
            {
                bool hasTargetJustBeenReached = this.IsTargetWithinDistance == false;
                this.IsTargetWithinDistance = true;

                if (hasTargetJustBeenReached)
                {
                    this.OnDestinationReachedEvent?.Invoke();
                }
            }
        }
    }
}