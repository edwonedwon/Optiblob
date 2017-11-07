using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoCube : MonoBehaviour
{
    public AnimationCurve upDownCurve;
    public float heightScale = 3f;
    public float animDuration = 2f;

    [HideInInspector]
    new public Rigidbody rigidbody;
    Vector3 startPos;
    float animStartTime;
    float animTimeNormalised;

    [HideInInspector]
    public bool updateAnim = true;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (!updateAnim)
            return;

        if (Time.time - animStartTime >= animDuration)
        {
            animStartTime = Time.time;
        }
        animTimeNormalised = (Time.time - animStartTime) / animDuration;
        float yNew = startPos.y + (upDownCurve.Evaluate(animTimeNormalised) * heightScale);
        Vector3 posNew = new Vector3(startPos.x, yNew, startPos.z);
        rigidbody.MovePosition(posNew);
    }
}
