using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Optiblob;

[RequireComponent(typeof(Rigidbody))]
public class AutoCube : MonoBehaviour
{
    public float heightScale = 3f;
    public float animDuration = 2f;

    [HideInInspector]
    public Rigidbody rb;

    Vector3 startPosition;

    public bool updateAnim = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = rb.position;
    }


    private void FixedUpdate()
    {
        if (!updateAnim) { return; }

        float newMagnitude = Mathf.Sin(Time.time * Mathf.PI / animDuration) * heightScale;

        rb.position = startPosition + transform.up * newMagnitude;
    }
}
