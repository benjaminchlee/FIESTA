using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using IATK;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public enum PanelDimension
{
    X,
    Y,
    Z,
    FACETBY,
    COLORBY,
    SIZEBY,
    COLORPALETTE,
    LINKING
}

public enum PanelPage
{
    MAIN = 0,
    STANDARD = 1,
    SPLOM = 2,
    DIMENSIONS = 3,
    SIZE = 4,
    COLOR = 5,
    MISC = 6
}

public class Panel : MonoBehaviourPunCallbacks
{
    private DataSource dataSource;
    public Photon.Realtime.Player OriginalOwner { get; private set; }

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

    private PanelPage activePage;
    private PanelPage activeChart;


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
        OriginalOwner = photonView.Owner;

        if (dataSource == null)
            dataSource = ChartManager.Instance.DataSource;

        if (photonView.IsMine)
        {
            // Configure standard scatterplot/standardChart
            standardChart.DataSource = ChartManager.Instance.DataSource;
            ChartManager.Instance.RegisterVisualisation(standardChart);
            standardChart.VisualisationType = AbstractVisualisation.VisualisationTypes.FACET;
            standardChart.GeometryType = AbstractVisualisation.GeometryType.Points;
            standardChart.Color = Color.white;
            standardChart.XDimension = dataSource[0].Identifier;
            standardChart.YDimension = dataSource[0].Identifier;
            //standardChart.ZDimension = dataSource[0].Identifier;
            standardChart.FacetSize = 1;
            standardChart.transform.position = standardTransform.position;
            standardChart.transform.rotation = standardTransform.rotation;

            // Resize standard Chart
            standardChart.Width = 0.5f;
            standardChart.Height = 0.5f;
            standardChart.Depth = 0.5f;

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

            ChangePage(PanelPage.STANDARD);
        }
        else
        {
            ChartManager.Instance.RegisterVisualisation(standardChart);
            //ChartManager.Instance.RegisterVisualisation(splomChart);
        }
    }

    private bool IsOriginalOwner()
    {
        if (OriginalOwner == null)
            return (photonView.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

        return (OriginalOwner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
    }

    /// <summary>
    /// An overload to allow for enums to be passed via the inspector
    /// </summary>
    /// <param name="page"></param>
    public void ChangePage(int page)
    {
        ChangePage((PanelPage)page);
    }

    public void ChangePage(PanelPage page)
    {
        switch (page)
        {
            case PanelPage.MAIN:
                // Change the page to either the standard or splom one, depending on the active chart
                ChangePage((activeChart == PanelPage.STANDARD) ? PanelPage.STANDARD : PanelPage.SPLOM);
                break;

            case PanelPage.STANDARD:
            case PanelPage.SPLOM:
                photonView.RPC("ToggleCharts", RpcTarget.All, page);
                photonView.RPC("ToggleButtons", RpcTarget.All, page);
                break;

            case PanelPage.DIMENSIONS:
            case PanelPage.SIZE:
            case PanelPage.COLOR:
            case PanelPage.MISC:
                photonView.RPC("ToggleButtons", RpcTarget.All, page);
                break;
        }
    }
    
    public void ChangeSpecialPage(PanelPage page, bool activate)
    {
        photonView.RPC("ToggleSpecialButtons", RpcTarget.All, (int)page, activate);
    }

    [PunRPC]
    private void ToggleCharts(PanelPage page)
    {
        switch (page)
        {
            case PanelPage.STANDARD:
                standardChart.transform.position = standardTransform.position;
                standardChart.transform.rotation = standardTransform.rotation;
               // splomChart.transform.position = Vector3.one * 9999;
                break;

            case PanelPage.SPLOM:
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
    private void ToggleButtons(PanelPage page)
    {
        switch (page)
        {
            case PanelPage.STANDARD:
                EnableAndDisableButtons(standardButtons);
                break;

            case PanelPage.SPLOM:
                EnableAndDisableButtons(splomButtons);
                break;

            case PanelPage.DIMENSIONS:
                EnableAndDisableButtons(dimensionsButtons);
                break;

            case PanelPage.SIZE:
                EnableAndDisableButtons(sizeButtons);
                break;

            case PanelPage.COLOR:
                EnableAndDisableButtons(colorButtons);
                break;

            case PanelPage.MISC:
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
    public void DimensionChanged(PanelDimension dimension, string dimensionName)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            switch (dimension)
            {
                case PanelDimension.X:
                    standardChart.XDimension = dimensionName;
                    break;

                case PanelDimension.Y:
                    standardChart.YDimension = dimensionName;
                    break;

                case PanelDimension.Z:
                    standardChart.ZDimension = dimensionName;
                    break;

                case PanelDimension.FACETBY:
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

                case PanelDimension.COLORBY:
                    standardChart.ColorDimension = dimensionName;
                    //splomChart.ColorDimension = dimensionName;
                    break;

                case PanelDimension.SIZEBY:
                    standardChart.SizeDimension = dimensionName;
                    //splomChart.SizeDimension = dimensionName;
                    break;

                case PanelDimension.COLORPALETTE:
                    standardChart.ColorPaletteDimension = dimensionName;
                    //splomChart.ColorPaletteDimension = dimensionName;
                    break;

                case PanelDimension.LINKING:
                    standardChart.LinkingDimension = dimensionName;
                    break;
            }
        }
        else
        {
            photonView.RPC("DimensionChanged", OriginalOwner, dimension, dimensionName);
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
            photonView.RPC("SizeSliderValueChanged", OriginalOwner, value);
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
            photonView.RPC("ColorPickerValueChanged", OriginalOwner, value);
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
            photonView.RPC("ScatterplotMatrixSizeValueChanged", OriginalOwner, value);
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
            photonView.RPC("FacetSizeValueChanged", OriginalOwner, value);
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

            photonView.RPC("ColorGradientChanged", OriginalOwner, gradientList.ToArray());
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
            photonView.RPC("ColorPaletteChanged", OriginalOwner, palette);
        }
    }

    /// <summary>
    /// Resets the ownership of all the subcharts back to the original owner of the panel (as that client is the one which
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
                if (!pv.IsMine)
                    pv.TransferOwnership(PhotonNetwork.LocalPlayer);
            }

            //splomChart.photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            //photonViews = splomChart.GetComponentsInChildren<PhotonView>();
            //foreach (PhotonView pv in photonViews)
            //{
            //    if (!pv.IsMine)
            //        pv.TransferOwnership(PhotonNetwork.LocalPlayer);
            //}
        }
    }
    
    public void LoadChart(Chart chart, bool destroyOnComplete = false)
    {
        photonView.RPC("LoadChart", RpcTarget.All, chart.photonView.ViewID, destroyOnComplete);
    }

    [PunRPC]
    private void LoadChart(int chartViewId, bool destroyOnComplete = false)
    {
        Chart chart = PhotonView.Find(chartViewId).GetComponent<Chart>();
        
        standardChart.LinkingDimension = chart.LinkingDimension;
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

        if (chart.AttributeFilters.Length == 0)
        {
            standardChart.FacetSize = 1;
            standardChart.FacetDimension = "Undefined";
        }

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

        if (destroyOnComplete && chart.photonView.IsMine)
            chart.transform.DOScale(0, 0.3f).SetEase(Ease.InBack)
                .OnComplete(() => ChartManager.Instance.RemoveVisualisation(chart));
    }
}
