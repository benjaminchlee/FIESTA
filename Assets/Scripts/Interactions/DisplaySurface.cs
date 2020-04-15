using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySurface : MonoBehaviour {
    
    public void CalculatePositionOnScreen(Chart chart, out Vector3 pos, out Quaternion rot)
    {
        // Temporarily remove parent for calculations
        Transform oldParent = chart.transform.parent;
        chart.transform.parent = null;
                     
        // Store the previous position and rotation
        Vector3 oldPos = chart.transform.position;
        Quaternion oldRot = chart.transform.rotation;

        // Rotate the chart such that it aligns against the wall nicely
        if (chart.Is3D)
        {
            float yChartRotation = chart.transform.eulerAngles.y;
            yChartRotation = 90 * (int)Mathf.Round(yChartRotation / 90.0f);
            rot = Quaternion.Euler(new Vector3(0, yChartRotation, 0));
            chart.transform.rotation = rot;
        }
        else
        {
            chart.transform.rotation = transform.rotation;
            rot = transform.rotation;
        }

        // For each of the 8 vertices, calculate how much to move the position of the chart such that it fits "inside" of the wall
        BoxCollider b = chart.GetComponent<BoxCollider>();

        Vector3 v1 = chart.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideScreen(chart.transform.position, v1);
        Vector3 v2 = chart.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, -b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideScreen(chart.transform.position, v2);
        Vector3 v3 = chart.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideScreen(chart.transform.position, v3);
        Vector3 v4 = chart.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideScreen(chart.transform.position, v4);
        Vector3 v5 = chart.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideScreen(chart.transform.position, v5);
        Vector3 v6 = chart.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, -b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideScreen(chart.transform.position, v6);
        Vector3 v7 = chart.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideScreen(chart.transform.position, v7);
        Vector3 v8 = chart.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideScreen(chart.transform.position, v8);

        pos = chart.transform.position;
        
        if (!chart.Is3D)
        {
            pos = transform.InverseTransformPoint(pos);
            pos.z = -b.size.z * 0.5f;
            pos = transform.TransformPoint(pos);
        }

        // Restore the original position, rotation and parent
        chart.transform.position = oldPos;
        chart.transform.rotation = oldRot;
        chart.transform.SetParent(oldParent);
        
    }

    private Vector3 MovePositionInsideScreen(Vector3 position, Vector3 vertex)
    {
        Vector3 localPos = transform.InverseTransformPoint(position);
        Vector3 localVertex = transform.InverseTransformPoint(vertex);
        
        // Case 1: vertex is too far to the left
        if (localVertex.x <= -0.5f)
        {
            float delta = Mathf.Abs(-0.5f - localVertex.x);
            localPos.x += delta;
        }
        // Case 2: vertex is too far to the right
        else if (0.5f <= localVertex.x)
        {
            float delta = localVertex.x - 0.5f;
            localPos.x -= delta;
        }
        // Case 3: vertex is too far to the top
        if (0.5f <= localVertex.y)
        {
            float delta = localVertex.y - 0.5f;
            localPos.y -= delta;
        }
        // Case 4: vertex is too far to the bottom
        else if (localVertex.y <= -0.5f)
        {
            float delta = Mathf.Abs(-0.5f - localVertex.y);
            localPos.y += delta;
        }
        // Case 5: vertex is behind the screen
        if (0f <= localVertex.z)
        {
            float delta = localVertex.z;
            localPos.z -= delta;
        }

        return transform.TransformPoint(localPos);
    }
}
