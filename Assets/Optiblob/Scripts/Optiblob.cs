using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optiblob : MonoBehaviour
{
    List<OptiblobPoint> points = new List<OptiblobPoint>();
    
    public int neighborConnections = 24;
    public float neighborSpring = 1000f;
    public float drag = 10f;
    float dragLast;
    public float rootSpring = 10f;
    float rootSpringLast;
    public float rootSpringDamp = 1f;
    float rootSpringDampLast;

    Dictionary<OptiblobPoint, List<OptiblobPoint>> neighbors;
    TwoKeyDictionary<OptiblobPoint, OptiblobPoint, float> restingDistances;

    public bool debugDraw;

    private void Awake()
    {
        neighbors = new Dictionary<OptiblobPoint, List<OptiblobPoint>>();

        points = new List<OptiblobPoint>(transform.parent.GetComponentsInChildren<OptiblobPoint>());
        restingDistances = new TwoKeyDictionary<OptiblobPoint, OptiblobPoint, float>();
        
        for (int i = 0; i < points.Count; i++)
        {
            OptiblobPoint a = points[i];
            a.Init(this, drag);
            for (int j = i+1; j < points.Count; j++)
            {
                OptiblobPoint b = points[j];
                restingDistances.Add(a, b, (a.transform.position - b.transform.position).sqrMagnitude);
            }
        }

        for (int i = 0; i < points.Count; i++)
        {
            OptiblobPoint point = points[i];
            neighbors.Add(point, NearestNeighbors(point, Mathf.Min(neighborConnections, points.Count)));
        }
    }

    private void FixedUpdate()
    {
        UpdateDrag();
        UpdateRootSprings();

        for (int i = 0; i < points.Count; i++) {
            OptiblobPoint a = points[i];
            List<OptiblobPoint> pointNeighbors = neighbors[a];

            for (int j = 0; j < pointNeighbors.Count; j++) {
                OptiblobPoint b = pointNeighbors[j];
                Relax(a, b);
            }
        }
    }

    void UpdateRootSprings()
    {
        if (rootSpring != rootSpringLast)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].rootSpring.spring = rootSpring;
            }
        }
        rootSpringLast = rootSpring;

        if (rootSpringDamp != rootSpringDampLast)
        {
            for (int i =0; i < points.Count; i++)
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
            for (int i = 0; i < points.Count; i++)
            {
                points[i].rigidbody.drag = drag;
            }
        }
        dragLast = drag;
    }

    void Relax(OptiblobPoint a, OptiblobPoint b)
    {
        Vector3 delta = a.transform.position - b.transform.position;
        float sqrDist = delta.sqrMagnitude;
        Vector3 force = Vector3.zero;
        if (sqrDist > Mathf.Epsilon)
        {
            force = delta.normalized * ((restingDistances.Get(a, b) - sqrDist) / sqrDist) * neighborSpring * Time.deltaTime;
        }
        a.AddForce(force);
        b.AddForce(-force);
    }

    List<OptiblobPoint> NearestNeighbors(OptiblobPoint point, int count)
    {
        List<OptiblobPoint> neighbors = new List<OptiblobPoint>(points);
        neighbors.Remove(point);
        neighbors.Sort((OptiblobPoint a, OptiblobPoint b) =>
        {
            return Vector3.Distance(point.transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(point.transform.position, b.transform.position));
        });
        return neighbors.GetRange(0, count);
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw)
            return;

        if (points.Count > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < points.Count; i++)
            {
                OptiblobPoint point = points[i];
                List<OptiblobPoint> pointNeighbors = neighbors[point];

                for (int j = 0; j < pointNeighbors.Count; j++) {
                    Gizmos.DrawLine(point.transform.position, pointNeighbors[j].transform.position);
                }
            }
        }
    }
}
