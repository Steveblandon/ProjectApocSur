namespace Projapocsur.Scripts
{
    using UnityEngine;

    public class Draggable : MonoBehaviour
    {
        void OnMouseDrag()
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPoint.z = this.transform.position.z;
            this.transform.position = worldPoint;
        }
    }
}
