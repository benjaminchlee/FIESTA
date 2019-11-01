using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSnapToSurface : MonoBehaviour
{
    public void CalculatePositionOnTable(Chart chart, out Vector3 pos, out Quaternion rot)
    {
        Vector3 oldPos = chart.transform.position;
        Quaternion oldRot = chart.transform.rotation;

        Bounds bounds = chart.GetComponent<Collider>().bounds;

        bool isHorizontal = false;

        // Set rotation
        if (chart.Is3D)
        {
            Vector3 euler = chart.transform.eulerAngles;

            // Snap all degrees of rotation to closest 90 degree increments
            euler.x = 90 * (int)Mathf.Round(euler.x / 90.0f);
            euler.y = 90 * (int)Mathf.Round(euler.y / 90.0f);
            euler.z = 90 * (int)Mathf.Round(euler.z / 90.0f);

            rot = Quaternion.Euler(euler);
        }
        else
        {
            float angle = Vector3.Angle(chart.transform.up, Vector3.up);
            Vector3 euler = chart.transform.eulerAngles;

            // Place vertically
            if (angle < 45 || 135 < angle)
            {
                isHorizontal = false;
                euler.x = 0;
                euler.z = 0;
            }
            //Place horizontally
            else
            {
                isHorizontal = true;
                euler.x = 90;
            }

            rot = Quaternion.Euler(euler);
        }

        chart.transform.rotation = rot;

        // Confine chart into bounds
        // For each of the 8 vertices, calculate how much to move the position of the chart such that it fits "inside" of the wall
        BoxCollider b = chart.GetComponent<BoxCollider>();

        Vector3 v1 = chart.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideTable(chart.transform.position, v1);
        Vector3 v2 = chart.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, -b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideTable(chart.transform.position, v2);
        Vector3 v3 = chart.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideTable(chart.transform.position, v3);
        Vector3 v4 = chart.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideTable(chart.transform.position, v4);
        Vector3 v5 = chart.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideTable(chart.transform.position, v5);
        Vector3 v6 = chart.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, -b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideTable(chart.transform.position, v6);
        Vector3 v7 = chart.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideTable(chart.transform.position, v7);
        Vector3 v8 = chart.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f);
        chart.transform.position = MovePositionInsideTable(chart.transform.position, v8);

        pos = chart.transform.position;

        if (!chart.Is3D && isHorizontal)
            pos.y -= 0.05f;

        chart.transform.position = oldPos;
        chart.transform.rotation = oldRot;
    }

    private Vector3 MovePositionInsideTable(Vector3 position, Vector3 vertex)
    {
        Vector3 localPos = transform.InverseTransformPoint(position);
        Vector3 localVertex = transform.InverseTransformPoint(vertex);
        Debug.Log(localVertex.ToString("F3"));
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
        // Case 3: vertex is too far forward
        if (0.5f <= localVertex.z)
        {
            float delta = localVertex.z - 0.5f;
            localPos.z -= delta;
        }
        // Case 4: vertex is too far to backward
        else if (localVertex.z <= -0.5f)
        {
            float delta = Mathf.Abs(-0.5f - localVertex.z);
            localPos.z += delta;
        }
        // Case 5: vertex is below the table
        if (localVertex.y <= 0.1f)
        {
            float delta = Mathf.Abs(0.1f - localVertex.y);
            localPos.y += delta;
        }

        return transform.TransformPoint(localPos);
    }
}
