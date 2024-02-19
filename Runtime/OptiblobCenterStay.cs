using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptiblobCenterStay : MonoBehaviour
{
    public Optiblob optiblob;
    public enum UpdateOption {Update, FixedUpdate, LateUpdate}
    public UpdateOption updateOption;

    void Update()
    {
        if (updateOption == UpdateOption.Update)
            transform.position = optiblob.BlobCenter;
    }

    void FixedUpdate()
    {
        if (updateOption == UpdateOption.FixedUpdate)
            transform.position = optiblob.BlobCenter;
    }

    void LateUpdate()
    {
        if (updateOption == UpdateOption.LateUpdate)
            transform.position = optiblob.BlobCenter;
    }
}
