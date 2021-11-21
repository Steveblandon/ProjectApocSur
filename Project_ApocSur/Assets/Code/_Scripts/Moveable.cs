namespace Projapocsur.Scripts
{
    using System;
    using UnityEngine;

    public enum MoveDirective
    {
        None,
        MoveToDestination,
        MoveInDirection
    }

    public class Moveable : MonoBehaviour
    {
        public static readonly string CompName = nameof(Moveable);
        public event Action OnDestinationReachedEvent;        

        private Vector3 startingPoint;
        private Vector3 destination;
        private Vector3 direction;
        private MoveDirective moveDirective;
        private float speed;
        private float distanceToTravel;

        void Start()
        {
            this.OnDestinationReached(false);
        }

        void Update()
        {
            switch (this.moveDirective)
            {
                case MoveDirective.MoveToDestination:
                    this.transform.position = Vector3.MoveTowards(this.transform.position, this.destination, this.speed * Time.deltaTime);
                    if (this.transform.position != this.destination)
                    {
                        this.transform.rotation = Quaternion.LookRotation(Vector3.forward, this.destination - this.transform.position);
                    }
                    else
                    {
                        this.OnDestinationReached(true);
                    }

                    break;
                case MoveDirective.MoveInDirection:
                    this.transform.Translate(this.direction * this.speed * Time.deltaTime, Space.World);    // NOTE: must be Space.World to have it account for rotation, otherwise it won't move in the proper direction.
                    float distanceTravelled = Vector3.Distance(startingPoint, this.transform.position);
                    if (distanceTravelled >= distanceToTravel)
                    {
                        this.OnDestinationReached(true);
                    }

                    break;
            }
        }

        public void MoveInDirection(Vector3 direction, float distance, float speed)
        {
            this.startingPoint = this.transform.position;
            this.speed = speed;
            this.direction = direction;
            this.distanceToTravel = distance;
            this.moveDirective = MoveDirective.MoveInDirection;
            this.enabled = true;
            Debug.Log($"{this.name} moving towards {this.direction}");
        }

        public void MoveToPosition(Vector3 position, float speed)
        {
            this.startingPoint = this.transform.position;
            this.speed = speed;
            this.destination = position;
            this.moveDirective = MoveDirective.MoveToDestination;
            this.enabled = true;
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
            this.direction = Vector3.zero;
            this.enabled = false;

            if (invokeEvent)
            {
                this.OnDestinationReachedEvent?.Invoke();
            }
        }
    }
}