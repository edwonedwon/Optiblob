using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optiblob : MonoBehaviour
{
    OptiblobPoint[] points = new OptiblobPoint[0];
    
    float[,] restingDistances;

    public float neighborSpring = 1000f;
    public float drag = 10f;
    float dragLast;
    public float rootSpring = 10f;
    float rootSpringLast;
    public float rootSpringDamp = 1f;
    float rootSpringDampLast;

    public bool debugDraw;

    private void Awake()
    {
        points = transform.parent.GetComponentsInChildren<OptiblobPoint>();
        restingDistances = new float[points.Length, points.Length];
        
        for (int i = 0; i < points.Length; i++)
        {
            OptiblobPoint child = points[i];
            child.Init(this, drag);
            for (int j = i+1; j < points.Length; j++)
            {
                restingDistances[i, j] = (points[i].transform.position - points[j].transform.position).sqrMagnitude;
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateDrag();
        UpdateRootSprings();

        for (int i = 0; i < points.Length; i++) {
            OptiblobPoint c1 = points[i];

            for (int j = i+1; j < points.Length; j++)
            {
                OptiblobPoint c2 = points[j];
                Vector3 delta = c1.transform.position - c2.transform.position;
                float sqrDist = delta.sqrMagnitude;
                Vector3 force = Vector3.zero;
                if (sqrDist > Mathf.Epsilon)
                {
                    force = delta.normalized * ((restingDistances[i, j] - sqrDist) / sqrDist) * neighborSpring * Time.deltaTime;
                }
                c1.AddForce(force);
                c2.AddForce(-force);
            }
        }
    }

    void UpdateRootSprings()
    {
        if (rootSpring != rootSpringLast)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].rootSpring.spring = rootSpring;
            }
        }
        rootSpringLast = rootSpring;

        if (rootSpringDamp != rootSpringDampLast)
        {
            for (int i =0; i < points.Length; i++)
            {
                points[i].rootSpring.damper = rootSpringDamp;
            }
        }
        rootSpringDampLast = rootSpringDamp;
    }

    void UpdateDrag()
    {
        if (drag != dragLast)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].rigidbody.drag = drag;
            }
        }
        dragLast = drag;
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw)
            return;

        if (points.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < points.Length; i++)
            {
                for (int j = i + 1; j < points.Length; j++)
                {
                    Gizmos.DrawLine(points[i].transform.position, points[j].transform.position);
                }
            }
        }
    }
}
