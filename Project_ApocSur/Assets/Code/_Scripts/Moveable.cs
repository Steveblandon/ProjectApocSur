namespace Projapocsur.Scripts
{
    using System;
    using UnityEngine;

    public enum MoveDirective
    {
        None,
        moveToDestination,
        moveInDirection
    }

    public class Moveable : MonoBehaviour
    {
        public event Action OnStoppedMovingEvent;
        
        public static readonly string CompName = nameof(Moveable);

        private Vector3 destination;
        private Vector3 direction;
        private MoveDirective moveDirective;
        private float speed;

        void Start()
        {
            this.destination = this.transform.position;
            this.direction = Vector3.zero;
        }

        void Update()
        {
            switch (this.moveDirective)
            {
                case MoveDirective.moveToDestination:
                    this.transform.position = Vector3.MoveTowards(this.transform.position, this.destination, this.speed * Time.deltaTime);
                    if (this.transform.position != this.destination)
                    {
                        this.transform.rotation = Quaternion.LookRotation(Vector3.forward, this.destination - this.transform.position);
                    }
                    else
                    {
                        this.OnDestinationReached();
                    }
                    break;
                case MoveDirective.moveInDirection:
                    this.transform.Translate(this.direction * this.speed * Time.deltaTime, Space.World);    // NOTE: must be Space.World to have it account for rotation, otherwise it won't move in the proper direction.
                    break;
            }
        }

        public void MoveInDirection(Vector3 direction, float speed)
        {
            this.speed = speed;
            this.direction = direction;
            this.moveDirective = MoveDirective.moveInDirection;
            this.enabled = true;
            Debug.Log($"{this.name} moving towards {this.direction}");
        }

        public void MoveToPosition(Vector3 position, float speed)
        {
            this.speed = speed;
            this.destination = position;
            this.moveDirective = MoveDirective.moveToDestination;
            this.enabled = true;
            Debug.Log($"{this.name} moving from {this.transform.position} to {this.destination} [current Up direction: {this.transform.up}]");
        }

        public void MoveToMousePosition(float speed)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPoint.z = this.transform.position.z;
            this.MoveToPosition(worldPoint, speed);
        }

        private void OnDestinationReached()
        {
            this.moveDirective = MoveDirective.None;
            this.OnStoppedMovingEvent?.Invoke();
            this.enabled = false;
        }
    }
}