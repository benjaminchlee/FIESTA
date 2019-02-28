using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using IATK;
using VRTK;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

public class ChartManager : MonoBehaviour {

    public static ChartManager Instance { get; private set; }

    [SerializeField]
    private CSVDataSource dataSource;
    public CSVDataSource DataSource
    {
        get { return dataSource; }
        set { dataSource = value; }
    }
    
    //public List<Chart> Charts { get; private set; }
    public List<Chart> Charts;
    public List<Visualisation> Visualisations { get; private set; }

    [Serializable]
    public class ChartAddedEvent : UnityEvent<Chart> { }
    public ChartAddedEvent ChartAdded;


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
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "MainScene")
        {
            Invoke("CreateDashboard", 0.1f);
        }
    }

    private void CreateDashboard()
    {
        Transform headset = VRTK_DeviceFinder.HeadsetCamera();

        if (PhotonNetwork.connected)
        {
            int id = PhotonNetwork.player.ID;

            Vector3 pos;
            Quaternion rot;

            //switch (id)
            //{
            //    case 1:
            //        pos = new Vector3(1f, 1.5f, 0f);
            //        rot = Quaternion.Euler(0f, 90f, 0f);
            //        break;

            //    case 2:
            //        pos = new Vector3(0f, 1.5f, -1f);
            //        rot = Quaternion.Euler(0f, 180f, 0f);
            //        break;

            //    case 3:
            //        pos = new Vector3(-1f, 1.5f, 0f);
            //        rot = Quaternion.Euler(0f, -90f, 0f);
            //        break;

            //    default:
            //        pos = headset.TransformPoint(Vector3.forward * 0.5f);
            //        rot = headset.rotation;
            //        break;
            //}

            pos = headset.TransformPoint(Vector3.forward * 0.5f);
            Vector3 euler = headset.eulerAngles;
            euler.x = 0;
            euler.z = 0;
            rot = Quaternion.Euler(euler);

            PhotonNetwork.Instantiate("Dashboard", pos, rot, 0);
        }
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
        chart.XNormaliser = dupe.XNormaliser;
        chart.YNormaliser = dupe.YNormaliser;
        chart.ZNormaliser = dupe.ZNormaliser;
        chart.ColorDimension = dupe.ColorDimension;
        chart.ColorPaletteDimension = dupe.ColorPaletteDimension;
        chart.Color = dupe.Color;
        chart.Gradient = dupe.Gradient;
        chart.ColorPalette = dupe.ColorPalette;
        chart.SizeDimension = dupe.SizeDimension;
        chart.Size = dupe.Size;
        chart.Scale = dupe.Scale;
        if (dupe.AttributeFilters[0].minScale > 0.001f || dupe.AttributeFilters[0].maxScale < 0.999f)
            chart.AttributeFilters = dupe.AttributeFilters;
        chart.ResizeHandleVisibility = true;

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

        ChartAdded.Invoke(chart);
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
