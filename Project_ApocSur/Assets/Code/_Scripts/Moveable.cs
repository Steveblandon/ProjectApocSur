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

    public class Moveable : MonoBehaviour
    {
        public static readonly string CompName = nameof(Moveable);
        public event Action OnDestinationReachedEvent;        

        private Vector3 startingPoint;
        private Vector3 destination;
        private MoveDirective moveDirective;
        private float speed;
        private float distanceToTravel;
        private ITargetable target;

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

            switch (this.moveDirective)
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

        public void TrackTarget(ITargetable target) => this.target = target;

        public void DisengageTarget() => this.target = null;

        public void MoveForward(float distance, float speed)
        {
            this.startingPoint = this.transform.position;
            this.speed = speed;
            this.distanceToTravel = distance;
            this.moveDirective = MoveDirective.MoveForward;
            Debug.Log($"{this.name} moving forward for a distance of {distance}");
        }

        public void MoveToPosition(Vector3 position, float speed)
        {
            this.startingPoint = this.transform.position;
            this.speed = speed;
            this.destination = position;
            this.moveDirective = MoveDirective.MoveToDestination;
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
            this.moveDirective = MoveDirective.None;
            this.distanceToTravel = 0;
            this.destination = this.transform.position;

            if (invokeEvent)
            {
                this.OnDestinationReachedEvent?.Invoke();
            }
        }
    }
}