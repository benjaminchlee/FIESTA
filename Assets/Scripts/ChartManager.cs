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
    
    public List<Chart> Charts { get; private set; }
    public List<Visualisation> Visualisations { get; private set; }

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

        Charts = new List<Chart>();
        Visualisations = new List<Visualisation>();
    }

    private void Start()
    {
        PhotonNetwork.Instantiate("Dashboard", Vector3.zero, Quaternion.identity, 0);
    }

    public Chart CreateVisualisation(string name)
    {
        GameObject vis;
        Chart chart;

        if (PhotonNetwork.connected)
        {
            vis = PhotonNetwork.Instantiate("Chart", Vector3.zero, Quaternion.identity, 0);
            vis.name = name;
            chart = vis.GetComponent<Chart>();
        }
        else
        {
            vis = new GameObject(name);
            chart = vis.AddComponent<Chart>();
        }

        chart.DataSource = DataSource;
        RegisterVisualisation(chart);

        return chart;
    }

    public Chart DuplicateVisualisation(Chart dupe)
    {
        GameObject vis;
        Chart chart;

        if (PhotonNetwork.connected)
        {
            vis = PhotonNetwork.Instantiate("Chart", Vector3.zero, Quaternion.identity, 0);
            chart = vis.GetComponent<Chart>();
        }
        else
        {
            vis = new GameObject();
            chart = vis.AddComponent<Chart>();
        }

        chart.DataSource = DataSource;
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

        RegisterVisualisation(chart);

        return chart;
    }

    public void RegisterVisualisation(Chart chart)
    {
        if (chart.DataSource == null)
            chart.DataSource = DataSource;

        if (!Charts.Contains(chart))
            Charts.Add(chart);
        if (!Visualisations.Contains(chart.Visualisation))
            Visualisations.Add(chart.Visualisation);
    }

    public void DeregisterVisualisation(Chart chart)
    {
        if (Charts.Contains(chart))
            Charts.Remove(chart);
        if (Visualisations.Contains(chart.Visualisation))
            Visualisations.Remove(chart.Visualisation);
    }

    public void RemoveVisualisation(Chart chart)
    {
        DeregisterVisualisation(chart);

        PhotonNetwork.Destroy(chart.gameObject);
    }
}
