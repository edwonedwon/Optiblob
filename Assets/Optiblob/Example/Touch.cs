using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour
{
    public Rigidbody cubeRB;
    public float z = 13;

	void FixedUpdate ()
    {
        foreach (UnityEngine.Touch touch in Input.touches)
        {
            var ray = Camera.main.ScreenPointToRay(touch.position);

            //Debug.Log("touch");
            if (touch.phase == TouchPhase.Began)
            {
                //Debug.Log("touch began");
                //// Construct a ray from the current touch coordinates
                //RaycastHit hit;
                //if (Physics.Raycast(ray, out hit))
                //{
                //    Debug.Log("hit: " + hit.point);
                //}
            }
            if (touch.phase == TouchPhase.Moved)
            {
                if (z != 0)
                {
                    Vector3 pos = ray.GetPoint(z);
                    cubeRB.MovePosition(pos);
                }
            }
        }
	}
}
