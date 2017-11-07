using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpringJoint))]
[RequireComponent(typeof(Rigidbody))]
public class OptiblobPoint : MonoBehaviour {

    Optiblob optiblob;
    public new Rigidbody rigidbody;
    Vector3 initialLocalPosition;
    public SpringJoint rootSpring;

    public void Init(Optiblob blob, float drag)
    {
        rootSpring = GetComponent<SpringJoint>();
        optiblob = blob;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.drag = drag;
        initialLocalPosition = transform.localPosition;
    }

    private void FixedUpdate()
    {
        //TODO: scale this based on distance
        Vector3 toInitialPositionVector = initialLocalPosition - transform.localPosition;
        float returnToInitialPositionPower = toInitialPositionVector.sqrMagnitude;
        Vector3 returnToInitialPositionForce = toInitialPositionVector * returnToInitialPositionPower;
        rigidbody.AddForce(returnToInitialPositionForce);
    }

    public void AddForce(Vector3 force)
    {
        rigidbody.AddForce(force);
    }
}
