using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayScreen : MonoBehaviour {

    //private List<Chart> attachedCharts;

    //private void Start()
    //{
    //    attachedCharts = new List<Chart>();
    //}

    //private void Update()
    //{
    //    foreach (Chart chart in attachedCharts)
    //    {
    //        if (!chart.isAnimating)
    //            ConstrainChartToScreen(chart);
    //    }
    //}

    public Vector3 CalculatePositionOnScreen(Chart chart)
    {
        // Temporarily remove parent for calculations
        Transform oldParent = chart.transform.parent;
        chart.transform.parent = null;

        // Calculate initial position of the chart
        Vector3 pos = gameObject.GetComponent<Collider>().ClosestPoint(chart.GetComponent<Collider>().bounds.center);

        // Temporarily move the chart to that position, storing the previous position and rotation
        Vector3 oldPos = chart.transform.position;
        Quaternion oldRot = chart.transform.rotation;
        chart.transform.position = pos;
        chart.transform.rotation = CalculateRotationOnScreen(chart);
        
        // Convert the working position to local space
        pos = gameObject.transform.InverseTransformPoint(pos);

        // Calculate corners of the chart's collider from local to world space
        BoxCollider b = chart.GetComponent<BoxCollider>();
        Vector3 topLeft = chart.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, b.size.z) * 0.5f);
        Vector3 bottomRight = chart.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f);

        // Convert back into local space whereby a chart is inside the screen when all points are in the range [-0.5, 0.5]
        topLeft = gameObject.transform.InverseTransformPoint(topLeft);
        bottomRight = gameObject.transform.InverseTransformPoint(bottomRight);

        float left = topLeft.x;
        float right = bottomRight.x;
        float top = topLeft.y;
        float bottom = bottomRight.y;

        // Case 1: chart is too far to the left
        if (left <= -0.5f)
        {
            float delta = Mathf.Abs(-0.5f - left);
            pos.x += delta;
        }
        // Case 2: chart is too far to the right
        else if (0.5f <= right)
        {
            float delta = right - 0.5f;
            pos.x -= delta;
        }
        // Case 3: chart is too far to the top
        if (0.5f <= top)
        {
            float delta = top - 0.5f;
            pos.y -= delta;
        }
        // Case 4: chart is too far to the bottom
        else if (bottom <= -0.5f)
        {
            float delta = Mathf.Abs(-0.5f - bottom);
            pos.y += delta;
        }

        // Force z position
        pos.z = -0.025f;

        // Convert the working pos back to world space
        pos = gameObject.transform.TransformPoint(pos);

        // Restore the original position, rotation and parent
        chart.transform.position = oldPos;
        chart.transform.rotation = oldRot;
        chart.transform.SetParent(oldParent);

        return pos;
    }

    public Quaternion CalculateRotationOnScreen(Chart chart)
    {
        return transform.rotation;
    }
}
