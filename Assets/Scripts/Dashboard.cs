using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using IATK;
using UnityEngine;
using UnityEngine.Events;

public enum DashboardDimension
{
    X,
    Y,
    Z,
    FACETBY,
    COLORBY,
    SIZEBY,
    COLORPALETTE
}

public enum DashboardPage
{
    MAIN = 0,
    STANDARD = 1,
    SPLOM = 2,
    DIMENSIONS = 3,
    SIZE = 4,
    COLOR = 5,
    FACET = 6
}

public class Dashboard : Photon.MonoBehaviour
{
    private DataSource dataSource;
    private PhotonPlayer originalOwner;

    public Chart standardChart;
    public Chart splomChart;

    [SerializeField]
    private Transform standardTransform;
    [SerializeField]
    private Transform splomTransform;

    [SerializeField]
    private List<GameObject> standardButtons;
    [SerializeField]
    private List<GameObject> splomButtons;
    [SerializeField]
    private List<GameObject> dimensionsButtons;
    [SerializeField]
    private List<GameObject> sizeButtons;
    [SerializeField]
    private List<GameObject> colorButtons;
    [SerializeField]
    private List<GameObject> facetButtons;
    
    private List<GameObject> allButtons;

    private DashboardPage activePage;
    private DashboardPage activeChart;


    [Serializable]
    public class VisualisationTransferredEvent : UnityEvent<Chart> { }
    public VisualisationTransferredEvent ChartTransferred;

    private void Awake()
    {
        allButtons = standardButtons.Union(splomButtons)
            .Union(dimensionsButtons)
            .Union(sizeButtons)
            .Union(colorButtons)
            .Union(facetButtons)
            .ToList();
    }

    private void Start()
    {
        originalOwner = photonView.owner;

        if (dataSource == null)
            dataSource = ChartManager.Instance.DataSource;

        if (photonView.isMine)
        {
            // Configure standard scatterplot/standardChart
            standardChart.DataSource = ChartManager.Instance.DataSource;
            ChartManager.Instance.RegisterVisualisation(standardChart);
            standardChart.VisualisationType = AbstractVisualisation.VisualisationTypes.FACET;
            standardChart.GeometryType = AbstractVisualisation.GeometryType.Points;
            standardChart.Color = Color.white;
            standardChart.XDimension = dataSource[0].Identifier;
            standardChart.YDimension = dataSource[0].Identifier;
            standardChart.FacetSize = 1;
            standardChart.Width = standardTransform.localScale.x;
            standardChart.Height = standardTransform.localScale.y;
            standardChart.Depth = standardTransform.localScale.z;
            standardChart.transform.position = standardTransform.position;
            standardChart.transform.rotation = standardTransform.rotation;

            //// Configure scatterplot matrix
            //splomChart.DataSource = ChartManager.Instance.DataSource;
            //ChartManager.Instance.RegisterVisualisation(splomChart);
            //splomChart.VisualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX;
            //splomChart.GeometryType = AbstractVisualisation.GeometryType.Points;
            //splomChart.Size = 0.3f;
            //splomChart.transform.position = splomTransform.position;
            //splomChart.transform.rotation = splomTransform.rotation;
            //splomChart.Width = splomTransform.localScale.x;
            //splomChart.Height = splomTransform.localScale.y;
            //splomChart.Depth = splomTransform.localScale.z;

            ChangePage(DashboardPage.STANDARD);
        }
        else
        {
            ChartManager.Instance.RegisterVisualisation(standardChart);
            //ChartManager.Instance.RegisterVisualisation(splomChart);
        }
    }

    private bool IsOriginalOwner()
    {
        if (originalOwner == null)
            return (photonView.owner.ID == PhotonNetwork.player.ID);

        return (originalOwner.ID == PhotonNetwork.player.ID);
    }

    /// <summary>
    /// An overload to allow for enums to be passed via the inspector
    /// </summary>
    /// <param name="page"></param>
    public void ChangePage(int page)
    {
        ChangePage((DashboardPage)page);
    }

    public void ChangePage(DashboardPage page)
    {
        switch (page)
        {
            case DashboardPage.MAIN:
                // Change the page to either the standard or splom one, depending on the active chart
                ChangePage((activeChart == DashboardPage.STANDARD) ? DashboardPage.STANDARD : DashboardPage.SPLOM);
                break;

            case DashboardPage.STANDARD:
            case DashboardPage.SPLOM:
                photonView.RPC("ToggleCharts", PhotonTargets.All, page);
                photonView.RPC("ToggleButtons", PhotonTargets.All, page);
                break;

            case DashboardPage.DIMENSIONS:
            case DashboardPage.SIZE:
            case DashboardPage.COLOR:
            case DashboardPage.FACET:
                photonView.RPC("ToggleButtons", PhotonTargets.All, page);
                break;
        }
    }
    
    public void ChangeSpecialPage(DashboardPage page, bool activate)
    {
        photonView.RPC("ToggleSpecialButtons", PhotonTargets.All, (int)page, activate);
    }

    [PunRPC]
    private void ToggleCharts(DashboardPage page)
    {
        switch (page)
        {
            case DashboardPage.STANDARD:
                standardChart.transform.position = standardTransform.position;
                standardChart.transform.rotation = standardTransform.rotation;
               // splomChart.transform.position = Vector3.one * 9999;
                break;

            case DashboardPage.SPLOM:
                standardChart.transform.position = Vector3.one * 9999;
                //splomChart.transform.position = splomTransform.position;
                //splomChart.transform.rotation = splomTransform.rotation;
                break;

            default:
                return;
        }

        activeChart = page;
    }

    [PunRPC]
    private void ToggleButtons(DashboardPage page)
    {
        switch (page)
        {
            case DashboardPage.STANDARD:
                EnableAndDisableButtons(standardButtons);
                break;

            case DashboardPage.SPLOM:
                EnableAndDisableButtons(splomButtons);
                break;

            case DashboardPage.DIMENSIONS:
                EnableAndDisableButtons(dimensionsButtons);
                break;

            case DashboardPage.SIZE:
                EnableAndDisableButtons(sizeButtons);
                break;

            case DashboardPage.COLOR:
                EnableAndDisableButtons(colorButtons);
                break;

            case DashboardPage.FACET:
                EnableAndDisableButtons(facetButtons);
                break;
        }

        activePage = page;
    }

    private void EnableAndDisableButtons(List<GameObject> buttonsToEnable)
    {
        foreach (GameObject button in allButtons)
        {
            if (buttonsToEnable.Contains(button))
            {
                button.SetActive(true);
            }
            else
            {
                button.SetActive(false);
            }
        }
    }
    
    [PunRPC]
    public void DimensionChanged(DashboardDimension dimension, string dimensionName)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            switch (dimension)
            {
                case DashboardDimension.X:
                    standardChart.XDimension = dimensionName;
                    break;

                case DashboardDimension.Y:
                    standardChart.YDimension = dimensionName;
                    break;

                case DashboardDimension.Z:
                    standardChart.ZDimension = dimensionName;
                    break;

                case DashboardDimension.FACETBY:
                    if (dimensionName == "Undefined")
                    {
                        standardChart.FacetSize = 1;
                        standardChart.FacetDimension = dimensionName;
                    }
                    else
                    {
                        standardChart.FacetDimension = dimensionName;
                    }
                    break;

                case DashboardDimension.COLORBY:
                    standardChart.ColorDimension = dimensionName;
                    //splomChart.ColorDimension = dimensionName;
                    break;

                case DashboardDimension.SIZEBY:
                    standardChart.SizeDimension = dimensionName;
                    //splomChart.SizeDimension = dimensionName;
                    break;

                case DashboardDimension.COLORPALETTE:
                    standardChart.ColorPaletteDimension = dimensionName;
                    //splomChart.ColorPaletteDimension = dimensionName;
                    break;
            }
        }
        else
        {
            photonView.RPC("DimensionChanged", originalOwner, dimension, dimensionName);
        }
    }
    
    [PunRPC]
    public void SizeSliderValueChanged(float value)
    {
        // If the current client owns the charts, change their values
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            standardChart.Size = value;
            //splomChart.Size = value;
        }
        // Otherwise, inform the original owner
        else
        {
            photonView.RPC("SizeSliderValueChanged", originalOwner, value);
        }
    }
    
    [PunRPC]
    public void ColorPickerValueChanged(Color value)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            standardChart.Color = value;
            //splomChart.Color = value;
        }
        else
        {
            photonView.RPC("ColorPickerValueChanged", originalOwner, value);
        }
    }

    [PunRPC]
    public void ScatterplotMatrixSizeValueChanged(int value)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            //splomChart.ScatterplotMatrixSize = value;
        }
        else
        {
            photonView.RPC("ScatterplotMatrixSizeValueChanged", originalOwner, value);
        }
    }

    [PunRPC]
    public void FacetSizeValueChanged(int value)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            standardChart.FacetSize = value;
        }
        else
        {
            photonView.RPC("FacetSizeValueChanged", originalOwner, value);
        }
    }

    public void ColorGradientChanged(Gradient gradient)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            standardChart.Gradient = gradient;
            //splomChart.Gradient = gradient;
        }
        else
        {
            List<float> gradientList = new List<float>();

            foreach (GradientColorKey colorKey in gradient.colorKeys)
            {
                gradientList.Add(colorKey.time);
                gradientList.Add(colorKey.color.r);
                gradientList.Add(colorKey.color.g);
                gradientList.Add(colorKey.color.b);
            }

            photonView.RPC("ColorGradientChanged", originalOwner, gradientList.ToArray());
        }
    }

    /// <summary>
    /// Sets the gradient of the charts. This overload uses a converted gradient in the form of a dictionary for serialization
    /// </summary>
    /// <param name="gradientDictionary"></param>
    [PunRPC]
    private void ColorGradientChanged(float[] gradientArray)
    {
        if (IsOriginalOwner())
        {
            // Create colorkeys from received array
            List<GradientColorKey> colorKeys = new List<GradientColorKey>();

            for (int i = 0; i < gradientArray.Length; i += 4)
            {
                Color color = new Color(gradientArray[i + 1], gradientArray[i + 2], gradientArray[i + 3]);

                GradientColorKey colorKey = new GradientColorKey(color, gradientArray[i]);
                colorKeys.Add(colorKey);
            }

            Gradient gradient = new Gradient();
            gradient.colorKeys = colorKeys.ToArray();

            ResetChartOwnership();

            standardChart.Gradient = gradient;
            //splomChart.Gradient = gradient;
        }
    }

    [PunRPC]
    public void ColorPaletteChanged(Color[] palette)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            standardChart.ColorPalette = palette;
            //splomChart.ColorPalette = palette;
        }
        else
        {
            photonView.RPC("ColorPaletteChanged", originalOwner, palette);
        }
    }

    /// <summary>
    /// Resets the ownership of all the subcharts back to the original owner of the dashboard (as that client is the one which
    /// has the proper links and references to the subcharts
    /// </summary>
    private void ResetChartOwnership()
    {
        if (IsOriginalOwner())
        {
            // Transfer ownership of all of its children
            PhotonView[] photonViews = standardChart.GetComponentsInChildren<PhotonView>();
            foreach (PhotonView pv in photonViews)
            {
                if (!pv.isMine)
                    pv.TransferOwnership(PhotonNetwork.player);
            }

            //splomChart.photonView.TransferOwnership(PhotonNetwork.player);
            //photonViews = splomChart.GetComponentsInChildren<PhotonView>();
            //foreach (PhotonView pv in photonViews)
            //{
            //    if (!pv.isMine)
            //        pv.TransferOwnership(PhotonNetwork.player);
            //}
        }
    }
    
    public void LoadChart(Chart chart, bool destroyOnComplete = false)
    {
        photonView.RPC("LoadChart", PhotonTargets.All, chart.photonView.viewID, destroyOnComplete);

    }

    [PunRPC]
    private void LoadChart(int chartViewId, bool destroyOnComplete = false)
    {
        Chart chart = PhotonView.Find(chartViewId).GetComponent<Chart>();

        standardChart.GeometryType = chart.GeometryType;
        standardChart.XDimension = chart.XDimension;
        standardChart.YDimension = chart.YDimension;
        standardChart.ZDimension = chart.ZDimension;
        standardChart.XNormaliser = chart.XNormaliser;
        standardChart.YNormaliser = chart.YNormaliser;
        standardChart.ZNormaliser = chart.ZNormaliser;
        standardChart.ColorDimension = chart.ColorDimension;
        standardChart.ColorPaletteDimension = chart.ColorPaletteDimension;
        standardChart.Color = chart.Color;
        standardChart.Gradient = chart.Gradient;
        standardChart.ColorPalette = chart.ColorPalette;
        standardChart.SizeDimension = chart.SizeDimension;
        standardChart.Size = chart.Size;
        //standardChart.AttributeFilters = chart.AttributeFilters;

        //splomChart.GeometryType = chart.GeometryType;
        //splomChart.ColorDimension = chart.ColorDimension;
        //splomChart.ColorPaletteDimension = chart.ColorPaletteDimension;
        //splomChart.Color = chart.Color;
        //splomChart.Gradient = chart.Gradient;
        //splomChart.ColorPalette = chart.ColorPalette;
        //splomChart.SizeDimension = chart.SizeDimension;
        //splomChart.Size = chart.Size;
        ////splomChart.AttributeFilters = chart.AttributeFilters;

        ChartTransferred.Invoke(chart);

        if (destroyOnComplete)
            chart.transform.DOScale(0, 0.4f).SetEase(Ease.InBack)
                .OnComplete(() => ChartManager.Instance.RemoveVisualisation(chart));
    }
}
