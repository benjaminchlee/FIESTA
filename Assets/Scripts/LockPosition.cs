using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Locks an object in position in either world or local space
/// </summary>
public class LockPosition : MonoBehaviour {

    public bool isWorldSpace = true;
    private Vector3 position = Vector3.zero;
    private Quaternion rotation = Quaternion.identity;

    public void SetPosition(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }

    private void LateUpdate()
    {
        if (isWorldSpace)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
        else
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
        }
    }
}
