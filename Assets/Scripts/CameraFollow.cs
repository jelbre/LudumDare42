using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    float minHeight;
    public Transform objectToFollow;
    public float heightOffset;

    private void Start()
    {
        minHeight = transform.position.y;
    }

    private void Update()
    {
        if (objectToFollow.position.y + heightOffset >= minHeight)
        {
            transform.position = new Vector3(transform.position.x, objectToFollow.position.y + heightOffset, transform.position.z);
        }
    }

}
