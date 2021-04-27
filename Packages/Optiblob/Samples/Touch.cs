using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour
{
    public AutoCube autoCube;
    public float z = 13;

	void FixedUpdate ()
    {
        foreach (UnityEngine.Touch touch in Input.touches)
        {
            var ray = Camera.main.ScreenPointToRay(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                autoCube.updateAnim = false;
            }
            if (touch.phase == TouchPhase.Moved)
            {
                if (z != 0)
                {
                    Vector3 pos = ray.GetPoint(z);
                    autoCube.rb.MovePosition(pos);
                }
            }
            if (touch.phase == TouchPhase.Ended)
            {
                autoCube.updateAnim = true;
            }
        }
	}
}
