using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Billboard : MonoBehaviour {

    [SerializeField] [Tooltip("Lock the rotation around the x axis.")]
    private bool xAxisLocked = false;
    [SerializeField] [Tooltip("Lock the rotation around the y axis.")]
    private bool yAxisLocked = false;
    [SerializeField] [Tooltip("Lock the rotation around the z axis.")]
    private bool zAxisLocked = false;
    [SerializeField] [Tooltip("Look at in the opposite direction.")]
    private bool flipDirection = false;
    
    private void Update () {
        if (VRTK_DeviceFinder.HeadsetCamera() != null)
        {
            if (!flipDirection)
                transform.LookAt(VRTK_DeviceFinder.HeadsetCamera());
            else
                transform.LookAt(2 * transform.position - VRTK_DeviceFinder.HeadsetCamera().position);

            float x = xAxisLocked ? transform.rotation.x : transform.rotation.eulerAngles.x;
            float y = yAxisLocked ? transform.rotation.y : transform.rotation.eulerAngles.y;
            float z = zAxisLocked ? transform.rotation.z : transform.rotation.eulerAngles.z;

            transform.rotation = Quaternion.Euler(x, y, z);
        }
	}
}
