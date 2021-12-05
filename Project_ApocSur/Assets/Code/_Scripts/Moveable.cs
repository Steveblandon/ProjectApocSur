namespace Projapocsur.Scripts
{
    using System;
    using UnityEngine;

    public enum MoveDirective
    {
        None,
        MoveToDestination,
        MoveForward
    }

    public class Moveable : MonoBehaviour, IMoveable
    {
        public event Action OnDestinationReachedEvent;

        private const string CompName = nameof(Moveable);

        private Vector3 startingPoint;
        private Vector3 destination;
        private float speed;
        private float distanceToTravel;
        private ITargetable target;

        public MoveDirective CurrentDirective { get; private set; }

        public bool HasTarget => target != null;

        void Start()
        {
            this.OnDestinationReached(false);
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
                    this.transform.position = Vector3.MoveTowards(this.transform.position, this.destination, this.speed * Time.deltaTime);
                    if (this.transform.position == this.destination)
                    {
                        this.OnDestinationReached(true);
                    }
                    else if (this.target == null)
                    {
                        this.transform.rotation = Quaternion.LookRotation(Vector3.forward, this.destination - this.transform.position);
                    }

                    break;
                case MoveDirective.MoveForward:
                    this.transform.Translate(this.transform.up * this.speed * Time.deltaTime, Space.World);    // NOTE: must be Space.World to have it account for rotation, otherwise it won't move in the proper direction.
                    float distanceTravelled = Vector3.Distance(startingPoint, this.transform.position);
                    if (distanceTravelled >= distanceToTravel)
                    {
                        this.OnDestinationReached(true);
                    }

                    break;
            }
        }

        public void LookAt(ITargetable target) => this.target = target;

        public void StopLookingAtTarget() => this.target = null;

        public void MoveForward(float distance, float speed)
        {
            this.startingPoint = this.transform.position;
            this.speed = speed;
            this.distanceToTravel = distance;
            this.CurrentDirective = MoveDirective.MoveForward;
            Debug.Log($"{this.name} moving forward for a distance of {distance}");
        }

        public void MoveToPosition(Vector3 position, float speed)
        {
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

        private void OnDestinationReached(bool invokeEvent)
        {
            this.CurrentDirective = MoveDirective.None;
            this.distanceToTravel = 0;
            this.destination = this.transform.position;

            if (invokeEvent)
            {
                this.OnDestinationReachedEvent?.Invoke();
            }
        }

        public void CancelCurrentDirective()
        {
            this.OnDestinationReached(false);
        }
    }
}