using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSnapToSurface : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 CalculateNewPosition(Chart chart)
    {
        Vector3 colliderPointPos = gameObject.GetComponent<Collider>().ClosestPoint(chart.GetComponent<Collider>().bounds.center);
        //Debug.Log("chart.GetComponent<Collider>().bounds.center: " + chart.GetComponent<Collider>().bounds.center);
        //Debug.Log("gameObject.GetComponent<Collider>().ClosestPoint: " + gameObject.GetComponent<Collider>().ClosestPoint(chart.GetComponent<Collider>().bounds.center));

        Vector3 localPos = chart.transform.position;
        //Debug.Log("localPos: " + localPos);

        Vector3 pos = new Vector3(colliderPointPos.x * 0.8f, colliderPointPos.y * 1.55f, localPos.z);
        //Debug.Log("pos: " + pos);

        return pos;
    }

    public Quaternion CalculateRotation(Chart chart)
    {
        Quaternion oldRot = chart.transform.rotation;
        Quaternion rot = new Quaternion(0f, oldRot.y, 0f, oldRot.w);
        //Debug.Log("rot " + rot);
        return rot;

    }
}
