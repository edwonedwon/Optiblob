using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Optiblob
{
    public class OptiblobCenterStay : MonoBehaviour
    {
        public Optiblob optiblob;
        public bool useFixedUpdate = false;

        void Update()
        {
            if (!useFixedUpdate)
                transform.position = optiblob.BlobCenter;
        }


        void FixedUpdate()
        {
            if (useFixedUpdate)
                transform.position = optiblob.BlobCenter;
        }
    }
}