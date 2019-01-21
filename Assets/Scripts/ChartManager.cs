using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using IATK;

public class ChartManager : MonoBehaviour {

    public static ChartManager Instance { get; private set; }

    [SerializeField]
    private CSVDataSource dataSource;
    public CSVDataSource DataSource
    {
        get { return dataSource; }
        set { dataSource = value; }
    }
    
    public List<Chart> Visualisations { get; private set; }

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

        Visualisations = new List<Chart>();
    }
    
    private void Start() { }

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
        Visualisations.Add(chart);

        chart.Initialise(DataSource);
        chart.VisualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT;
        chart.GeometryType = AbstractVisualisation.GeometryType.Points;
        chart.XDimension = "mpg";
        chart.YDimension = "cylinders";

        return chart;
    }

    public Chart CreateVisualisation(string name)
    {
        GameObject vis = new GameObject();
        vis.name = name;
        Chart chart = vis.AddComponent<Chart>();
        Visualisations.Add(chart);

        chart.Initialise(DataSource);

        return chart;
    }

    public Chart DuplicateVisualisation(Chart dupe)
    {
        GameObject vis = new GameObject();
        Chart chart = vis.AddComponent<Chart>();
        Visualisations.Add(chart);

        chart.Initialise(DataSource);
        chart.VisualisationType = dupe.VisualisationType;
        chart.GeometryType = dupe.GeometryType;
        chart.XDimension = dupe.XDimension;
        chart.YDimension = dupe.YDimension;
        chart.ZDimension = dupe.ZDimension;
        chart.Color = dupe.Color;
        chart.Size = dupe.Size;
        chart.Width = dupe.Width;
        chart.Height = dupe.Height;
        chart.Depth = dupe.Depth;
        chart.AttributeFilters = dupe.AttributeFilters;

        vis.transform.position = dupe.transform.position;
        vis.transform.rotation = dupe.transform.rotation;
        vis.transform.localScale = dupe.transform.localScale;

        return chart;
    }

    public void RemoveVisualisation(Chart chart)
    {
        if (Visualisations.Contains(chart))
            Visualisations.Remove(chart);

        Destroy(chart.gameObject);
    }
}
