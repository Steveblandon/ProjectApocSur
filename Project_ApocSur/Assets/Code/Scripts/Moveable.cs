namespace Projapocsur.Scripts
{
    using UnityEngine;

    public class Moveable : MonoBehaviour
    {
        public static readonly string CompName = nameof(Moveable);

        [SerializeField]
        private float speed = 1;
        [SerializeField]
        private Vector3 destination;

        void Start()
        {
            destination = this.transform.position;
        }

        void Update()
        {
            if (this.transform.position != destination)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, destination, speed * Time.fixedDeltaTime);
                this.transform.rotation = Quaternion.LookRotation(Vector3.forward, destination - this.transform.position);
            }
            else
            {
                this.enabled = false;
            }
        }

        public void MoveTo(Vector3 destination)
        {
            this.destination = destination;
            this.enabled = true;
            Debug.Log($"{this.name} moving from {this.transform.position} to {this.destination}");
        }

        public void MoveToMousePosition()
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPoint.z = this.transform.position.z;
            this.MoveTo(worldPoint);
        }
    }
}