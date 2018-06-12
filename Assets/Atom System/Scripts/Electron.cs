using UnityEngine;
using System.Collections;

public class Electron : MonoBehaviour {

    public Transform centre;
    public Vector3 axis = Vector3.up;
    public float radius = 2.0f;
    public float radiusSpeed = 0.5f;
    public float rotationSpeed = 80.0f;

    public void Start()
    {
        transform.position = (transform.position - centre.position).normalized * radius + centre.position;
    }

    public void Update()
    {
        transform.RotateAround(centre.position, axis, rotationSpeed * Time.deltaTime);
        var desiredPosition = (transform.position - centre.position).normalized * radius + centre.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
    }
}
