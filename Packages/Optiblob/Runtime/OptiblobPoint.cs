using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Optiblob 
{
    [RequireComponent(typeof(Rigidbody))]
    public class OptiblobPoint : MonoBehaviour
    {
        Optiblob optiblob;
        [HideInInspector]
        public Rigidbody rb;
        [HideInInspector]
        public SpringJoint rootSpring;
        public List<SpringJoint> neighborSprings;

        public void Init(Optiblob blob, float drag)
        {
            optiblob = blob;
            rb = GetComponent<Rigidbody>();
            rb.drag = drag;
            rootSpring = transform.GetOrAddComponent<SpringJoint>();
            rootSpring.connectedBody = blob.rootRigidbody;

            // freeze rigidbody rotation constraints
            RigidbodyConstraints constraints = RigidbodyConstraints.FreezeRotation;
            rb.constraints = constraints;
        }

        public void InitNeighborSprings()
        {
            for (int i = 0; i < Neighbors().Count; i++)
            {
                SpringJoint newSpring = gameObject.AddComponent<SpringJoint>();
                newSpring.connectedBody = Neighbors()[i].rb;
                neighborSprings.Add(newSpring);
            }
            UpdateNeighborSpringSettings();
        }

        public void UpdateNeighborSpringSettings()
        {
            foreach (SpringJoint spring in neighborSprings)
            {
                spring.spring = optiblob.neighborSpring;
                spring.tolerance = optiblob.allSpringsTolerance;
            }
        }

        public void AddForce(Vector3 force)
        {
            rb.AddForce(force);
        }

        public List<OptiblobPoint> Neighbors()
        {
            return optiblob.NeighborsOfPoint(this);
        }
    }
}