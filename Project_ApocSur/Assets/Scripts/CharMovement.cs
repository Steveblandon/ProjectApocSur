using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMovement : MonoBehaviour
{
    public float speed = 1;
    private Vector3 target;
    private Rigidbody2D rigidbodyComp;

    void Start()
    {
        target = this.transform.position;
        rigidbodyComp = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = this.transform.position.z;
            Debug.Log($"mouse down detected at {Input.mousePosition}, transformed to world space: {target}");
        }

        this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.fixedDeltaTime);

        /*if (target != Vector3.zero)
        {
            float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }*/

        if (target != this.transform.position)
        {
            this.transform.rotation = Quaternion.LookRotation(Vector3.forward, target - this.transform.position);
        }
    }
}
