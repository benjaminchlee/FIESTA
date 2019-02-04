using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IATK;
using UnityEngine;
using UnityEngine.Rendering;

public enum WorkScreenDimension
{
    X,
    Y,
    Z,
    FACETBY,
    COLORBY
}

public class WorkScreen : Photon.MonoBehaviour
{
    private DataSource dataSource;

    private Chart standardChart;
    private Chart splomChart;

    [SerializeField]
    private Transform standardTransform;
    [SerializeField]
    private Transform splomTransform;

    [SerializeField]
    private List<GameObject> standardButtons;
    [SerializeField]
    private List<GameObject> facetButtons;
    [SerializeField]
    private List<GameObject> splomButtons;
    
    private void Start()
    {
        if (dataSource == null)
            dataSource = ChartManager.Instance.DataSource;

        if (standardButtons == null)
            standardButtons = new List<GameObject>();

        if (splomButtons == null)
            splomButtons = new List<GameObject>();
        
        if (facetButtons == null)
            facetButtons = new List<GameObject>();

        if (PhotonNetwork.isMasterClient)
        {
            // Configure standard scatterplot/standardChart
            standardChart = ChartManager.Instance.CreateVisualisation("WorkscreenStandardChart");
            standardChart.VisualisationType = AbstractVisualisation.VisualisationTypes.FACET;
            standardChart.GeometryType = AbstractVisualisation.GeometryType.Points;
            standardChart.Color = Color.white;
            standardChart.XDimension = dataSource[0].Identifier;
            standardChart.YDimension = dataSource[0].Identifier;
            //standardChart.FacetDimension = "mpg";
            //standardChart.FacetSize = 5;
            standardChart.Width = standardTransform.localScale.x;
            standardChart.Height = standardTransform.localScale.y;
            standardChart.Depth = standardTransform.localScale.z;
            standardChart.transform.position = standardTransform.position;
            standardChart.transform.rotation = standardTransform.rotation;

            // Configure scatterplot matrix
            splomChart = ChartManager.Instance.CreateVisualisation("WorkscreenSPLOMChart");
            splomChart.VisualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX;
            splomChart.GeometryType = AbstractVisualisation.GeometryType.Points;
            splomChart.Size = 0.3f;
            splomChart.transform.position = splomTransform.position;
            splomChart.transform.rotation = splomTransform.rotation;
            splomChart.Width = splomTransform.localScale.x;
            splomChart.Height = splomTransform.localScale.y;
            splomChart.Depth = splomTransform.localScale.z;

            ShowStandard();

            // Send Photon View IDs to all clients using buffer
            photonView.RPC("SetPrototypeIds", PhotonTargets.OthersBuffered, standardChart.photonView.viewID, splomChart.photonView.viewID);
        }
    }

    [PunRPC]
    private void SetPrototypeIds(int standardChartId, int splomChartId)
    {
        standardChart = PhotonView.Find(standardChartId).gameObject.GetComponent<Chart>();
        splomChart = PhotonView.Find(splomChartId).gameObject.GetComponent<Chart>();

        ShowStandard();
    }

    public void ShowStandard()
    {
        ToggleState(true, false, false);

        standardChart.FacetSize = 1;
    }

    public void ShowSPLOM()
    {
        ToggleState(false, true, false);
    }

    public void ShowFacet()
    {
        ToggleState(true, false, true);
    }

    private void ToggleState(bool sp, bool spm, bool f)
    {
        standardChart.photonView.RequestOwnership();
        splomChart.photonView.RequestOwnership();

        standardChart.transform.position = (sp || f) ? standardTransform.position : new Vector3(9999, 9999, 9999);
        splomChart.transform.position = spm ? splomTransform.position : new Vector3(99999, 9999, 9999);
        standardChart.transform.rotation = standardTransform.rotation;
        splomChart.transform.rotation = splomTransform.rotation;
        
        foreach (GameObject button in standardButtons.Concat(splomButtons).Concat(facetButtons))
        {
            bool isActive = (sp && standardButtons.Contains(button)) ||
                            (spm && splomButtons.Contains(button)) || 
                            (f && facetButtons.Contains(button));
            button.SetActive(isActive);
        }
    }

    public void DimensionChanged(WorkScreenDimension dimension, string dimensionName)
    {
        switch (dimension)
        {
            case WorkScreenDimension.X:
                standardChart.XDimension = dimensionName;
                break;

            case WorkScreenDimension.Y:
                standardChart.YDimension = dimensionName;
                break;

            case WorkScreenDimension.Z:
                standardChart.ZDimension = dimensionName;
                break;

            case WorkScreenDimension.FACETBY:
                if (dimensionName == "None")
                {
                    ShowStandard();
                    standardChart.FacetSize = 1;
                }
                else
                {
                    ShowFacet();
                    standardChart.FacetDimension = dimensionName;
                }
                break;
        }
    }

    public void SizeSliderValueChanged(float value)
    {
        if (standardChart != null && splomChart != null)
        {
            standardChart.Size = value;
            splomChart.Size = value;
        }
    }

    public void RedSliderValueChanged(float value)
    {
        if (standardChart != null && splomChart != null)
        {
            Color color = standardChart.Color;
            color.r = value;

            standardChart.Color = color;
            splomChart.Color = color;
        }
    }

    public void GreenSliderValueChanged(float value)
    {
        if (standardChart != null && splomChart != null)
        {
            Color color = standardChart.Color;
            color.g = value;

            standardChart.Color = color;
            splomChart.Color = color;
        }
    }

    public void BlueSliderValueChanged(float value)
    {
        if (standardChart != null && splomChart != null)
        {
            Color color = standardChart.Color;
            color.b = value;

            standardChart.Color = color;
            splomChart.Color = color;
        }
    }

    public void ScatterplotMatrixSizeSlider(float value)
    {
        if (standardChart != null && splomChart != null)
        {
            splomChart.ScatterplotMatrixSize = (int)value;
        }
    }

    public void FacetSizeSlider(float value)
    {
        if (standardChart != null && splomChart != null)
        {
        }
    }
}
