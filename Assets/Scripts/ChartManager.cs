using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using IATK;

public class ChartManager : MonoBehaviour {

    public static ChartManager Instance { get; private set; }

    public CSVDataSource dataSource;
    private List<Chart> visualisations;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        visualisations = new List<Chart>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            AddVisualisation();
        }
    }

    public Chart AddVisualisation()
    {
        GameObject vis = new GameObject();
        Chart chart = vis.AddComponent<Chart>();
        visualisations.Add(chart);

        chart.Initialise(dataSource);
        chart.VisualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT;
        chart.GeometryType = AbstractVisualisation.GeometryType.Points;
        chart.XDimension = "mpg";
        chart.YDimension = "cylinders";

        return chart;
    }

    public Chart DuplicateVisualisation(Chart dupe)
    {
        GameObject vis = new GameObject();
        vis.transform.position = dupe.transform.position;
        vis.transform.rotation = dupe.transform.rotation;
        vis.transform.localScale = dupe.transform.localScale;

        Chart chart = vis.AddComponent<Chart>();
        visualisations.Add(chart);

        chart.Initialise(dataSource);
        chart.VisualisationType = dupe.VisualisationType;
        chart.XDimension = dupe.XDimension;
        chart.YDimension = dupe.YDimension;
        chart.ZDimension = dupe.ZDimension;
        chart.Color = dupe.Color;
        chart.Size = dupe.Size;
        chart.Width = dupe.Width;
        chart.Height = dupe.Height;
        chart.Depth = dupe.Depth;
        
        return chart;
    }
}
