using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen : MonoBehaviour {

    private List<Chart> attachedCharts;

    private void Start()
    {
        attachedCharts = new List<Chart>();
    }

    private void Update()
    {
        foreach (Chart chart in attachedCharts)
        {
            ConstrainChartToScreen(chart);
        }
    }

    private void ConstrainChartToScreen(Chart chart)
    {
        Transform previousParent = chart.transform.parent;
        chart.transform.parent = null;
                
        chart.transform.rotation = gameObject.transform.rotation;  // Force rotation

        // Position
        Vector3 pos = gameObject.transform.InverseTransformPoint(chart.transform.position);

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

        chart.transform.position = gameObject.transform.TransformPoint(pos);

        chart.transform.SetParent(previousParent);
    }

    public void AttachChart(Chart chart)
    {
        if (!attachedCharts.Contains(chart))
        {
            attachedCharts.Add(chart);

            Transform previousParent = chart.transform.parent;
            chart.transform.parent = null;

            Vector3 newPos = gameObject.GetComponent<Collider>().ClosestPoint(chart.GetComponent<Collider>().bounds.center);

            chart.transform.position = newPos;

            chart.transform.SetParent(previousParent);

            ConstrainChartToScreen(chart);
        }
    }

    public void DetachChart(Chart chart)
    {
        if (!attachedCharts.Contains(chart))
        {
            attachedCharts.Remove(chart);
        }
    }
}
