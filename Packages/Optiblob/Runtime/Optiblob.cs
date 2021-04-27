using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edwon.Tools;

public class Optiblob : MonoBehaviour
{
    protected List<OptiblobPoint> blobPoints = new List<OptiblobPoint>();
    List<Transform> blobPointsTransforms = new List<Transform>();

    public Vector3 BlobCenter 
    {
        get 
        {
            return Utils.GetCenterPoint(blobPointsTransforms);
        }
    }

    [Tooltip("the transform parent of all the optiblob points, can be different than the root rigidbody")]
    public Transform pointsParent;

    [Header("ROOT RIGIDBODY")]
    public Rigidbody rootRigidbody; // the center that all the points are attached to with springs
    public float rootMass = 1;
    float rootMassLast;
    public float rootDrag;
    float rootDragLast;
    public float rootAngularDrag;
    float rootAngularDragLast;

    [Header("POINTS RIGIDBODY")]
    public float pointMass = 1f;
    float pointMassLast;
    public float pointDrag = 10f;
    float pointDragLast;
    public float pointAngularDrag = 1f;
    float pointAngularDragLast;
    public bool constrainPointRotation;
    bool constrainPointRotationLast;

    [Header("ALL SPRINGS/RIGIDBODIES")]
    public float allRigidbodySleepThreshold;
    float allRigidbodySleepThresholdLast;
    public float allSpringsTolerance;
    float allSpringsToleranceLast;

    [Header("NEIGHBOR SPRINGS")]
    public int neighborConnections = 24;
    public float neighborSpring = 1000f;
    float neighborSpringLast;

    [Header("ROOT SPRINGS")]
    public float rootSpring = 10f;
    float rootSpringLast;
    public float rootSpringDamp = 1f;
    float rootSpringDampLast;
    float rootSpringToleranceLast;

    Dictionary<OptiblobPoint, List<OptiblobPoint>> neighbors;
    Dictionary<OptiblobPoint, List<OptiblobPoint>> neighborsByDistance;
    TwoKeyDictionary<OptiblobPoint, OptiblobPoint, float> restingDistances;

    [Header("DEBUG")]
    public bool debugLog;
    public bool debugDraw;

    public void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        neighbors = new Dictionary<OptiblobPoint, List<OptiblobPoint>>();
        blobPoints = new List<OptiblobPoint>(pointsParent.GetComponentsInChildren<OptiblobPoint>());
        foreach (OptiblobPoint point in blobPoints)
            blobPointsTransforms.Add(point.transform);
        restingDistances = new TwoKeyDictionary<OptiblobPoint, OptiblobPoint, float>();
        InitNeighborsByDistance();

        for (int i = 0; i < blobPoints.Count; i++)
        {
            OptiblobPoint a = blobPoints[i];
            a.Init(this, pointDrag);
            for (int j = i + 1; j < blobPoints.Count; j++)
            {   
                OptiblobPoint b = blobPoints[j];
                restingDistances.Add(a, b, (a.transform.position - b.transform.position).sqrMagnitude);
            }
        }

        for (int i = 0; i < blobPoints.Count; i++)
        {
            OptiblobPoint point = blobPoints[i];
            neighbors.Add(point, NearestNeighbors(point, Mathf.Min(neighborConnections, blobPoints.Count)));
        }

        for (int i = 0; i < blobPoints.Count; i++)
        {
            blobPoints[i].InitNeighborSprings();
        }

        UpdateOptiblobPointSettings();

        if (debugLog)
        {
            Debug.Log("Optiblob Points: " + blobPoints.Count);
        }

    }

    protected virtual void Update()
    {
        CheckAndUpdateOptiblobSettings();
    }

    void CheckAndUpdateOptiblobSettings()
    {
        if (allSpringsTolerance != rootSpringToleranceLast ||
            rootSpring != rootSpringLast ||
            rootSpringDamp != rootSpringDampLast ||
            pointDragLast != pointDrag ||
            pointAngularDragLast != pointAngularDrag ||
            rootDragLast != rootDrag ||
            rootAngularDragLast != rootAngularDrag ||
            rootMassLast != rootMass ||
            pointMassLast != pointMass ||
            allRigidbodySleepThresholdLast != allRigidbodySleepThreshold ||
            allSpringsToleranceLast != allSpringsTolerance ||
            neighborSpringLast != neighborSpring ||
            constrainPointRotationLast != constrainPointRotation
            )
        {
            UpdateOptiblobPointSettings();
        }

        rootSpringLast = rootSpring;
        rootSpringToleranceLast = allSpringsTolerance;
        rootSpringDampLast = rootSpringDamp;
        pointMassLast = pointMass;
        pointDragLast = pointDrag;
        pointAngularDragLast = pointAngularDrag;
        rootMassLast = rootMass;
        rootDragLast = rootDrag;
        rootAngularDragLast = rootAngularDrag;
        allRigidbodySleepThresholdLast = allRigidbodySleepThreshold;
        allSpringsToleranceLast = allSpringsTolerance;
        neighborSpringLast = neighborSpring;
        constrainPointRotationLast = constrainPointRotation;
    }

    void UpdateOptiblobPointSettings()
    {
        rootRigidbody.mass = rootMass;
        rootRigidbody.drag = rootDrag;
        rootRigidbody.angularDrag = rootAngularDrag;
        rootRigidbody.sleepThreshold = allRigidbodySleepThreshold;

        for (int i = 0; i < blobPoints.Count; i++)
        {
            blobPoints[i].rootSpring.spring = rootSpring;
            blobPoints[i].rootSpring.tolerance = allSpringsTolerance;
            blobPoints[i].rootSpring.damper = rootSpringDamp;
            blobPoints[i].UpdateNeighborSpringSettings();
            blobPoints[i].rb.drag = pointDrag;
            blobPoints[i].rb.angularDrag = pointAngularDrag;
            blobPoints[i].rb.mass = pointMass;
            if (constrainPointRotation)
                blobPoints[i].rb.constraints = RigidbodyConstraints.FreezeRotation;
            else
                blobPoints[i].rb.constraints = RigidbodyConstraints.None;
        }
    }

    public List<OptiblobPoint> NeighborsOfPoint(OptiblobPoint point)
    {
        return neighbors[point];
    }

    public List<OptiblobPoint> NearestNeighbors(OptiblobPoint point, int count)
    {
        return neighborsByDistance[point].GetRange(0, count-1);
    }

    void InitNeighborsByDistance()
    {
        neighborsByDistance = new Dictionary<OptiblobPoint, List<OptiblobPoint>>();
        for (int i = 0; i < blobPoints.Count; i++)
        {
            OptiblobPoint point = blobPoints[i];
            List<OptiblobPoint> neighbors = new List<OptiblobPoint>(blobPoints);
            neighbors.Remove(point);
            neighbors.Sort((OptiblobPoint a, OptiblobPoint b) =>
            {
                return Vector3.Distance(point.transform.position, a.transform.position)
                .CompareTo(Vector3.Distance(point.transform.position, b.transform.position));
            });
            neighborsByDistance[point] = neighbors;
        }
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw)
            return;

        if (blobPoints.Count > 0)
        {
            Gizmos.color = Color.blue;

            // NEW PLAN: DRAW A LINE FOR EACH SPRING

            //foreach (OptiblobPoint point in blobPoints)
            //{
            //    foreach(
            //}

            // DRAW ALL NEIGHBOR SPRINGS
            var enumerator = neighbors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                List<OptiblobPoint> points = enumerator.Current.Value;
                foreach (OptiblobPoint point in points)
                {
                    Gizmos.DrawLine(enumerator.Current.Key.transform.position, point.transform.position);
                }
            }

            Gizmos.color = Color.red;

            // DRAW ROOT SPRINGS
            foreach (OptiblobPoint point in blobPoints)
            {
                Gizmos.DrawLine(point.transform.position, point.rootSpring.connectedBody.transform.position);
            }

            // JUST DRAW LINES BETWEEN EACH NEIGHBOR (DOESNT SHOW SPRINGS AT ALL)
            //for (int i = 0; i < blobPoints.Count; i++)
            //{
            //    OptiblobPoint point = blobPoints[i];
            //    List<OptiblobPoint> pointNeighbors = neighbors[point];

                //for (int j = 0; j < pointNeighbors.Count; j++) {
                //    Gizmos.DrawLine(point.transform.position, pointNeighbors[j].transform.position);
                //}
            //}
        }
    }
}
