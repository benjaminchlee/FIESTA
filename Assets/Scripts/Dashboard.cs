using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DG.Tweening.Plugins;
using IATK;
using UnityEngine;
using UnityEngine.Rendering;

public enum DashboardDimension
{
    X,
    Y,
    Z,
    FACETBY,
    COLORBY
}

public class Dashboard : Photon.MonoBehaviour
{
    private DataSource dataSource;
    
    public Chart standardChart;
    public Chart splomChart;
    
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

    private PhotonPlayer originalOwner;

    private void Start()
    {
        originalOwner = photonView.owner;

        if (dataSource == null)
            dataSource = ChartManager.Instance.DataSource;

        if (standardChart == null)
            standardChart = transform.Find("StandardChart").GetComponent<Chart>();

        if (splomChart == null)
            splomChart = transform.Find("SPLOMChart").GetComponent<Chart>();

        if (standardButtons == null)
            standardButtons = new List<GameObject>();

        if (splomButtons == null)
            splomButtons = new List<GameObject>();
        
        if (facetButtons == null)
            facetButtons = new List<GameObject>();

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
            //standardChart.FacetDimension = "mpg";
            //standardChart.FacetSize = 5;
            standardChart.Width = standardTransform.localScale.x;
            standardChart.Height = standardTransform.localScale.y;
            standardChart.Depth = standardTransform.localScale.z;
            standardChart.transform.position = standardTransform.position;
            standardChart.transform.rotation = standardTransform.rotation;

            // Configure scatterplot matrix
            splomChart.DataSource = ChartManager.Instance.DataSource;
            ChartManager.Instance.RegisterVisualisation(splomChart);
            splomChart.VisualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX;
            splomChart.GeometryType = AbstractVisualisation.GeometryType.Points;
            splomChart.Size = 0.3f;
            splomChart.transform.position = splomTransform.position;
            splomChart.transform.rotation = splomTransform.rotation;
            splomChart.Width = splomTransform.localScale.x;
            splomChart.Height = splomTransform.localScale.y;
            splomChart.Depth = splomTransform.localScale.z;

            ShowStandard();
        }
    }

    private bool IsOriginalOwner()
    {
        return (originalOwner.ID == PhotonNetwork.player.ID);
    }

    public void ShowStandard()
    {
        ToggleState(true, false, false);

        FacetSizeValueChanged(1);
    }

    public void ShowSPLOM()
    {
        ToggleState(false, true, false);
    }

    public void ShowFacet()
    {
        ToggleState(true, false, true);
    }

    [PunRPC]
    private void ToggleState(bool sp, bool spm, bool f)
    {
        if (standardChart.photonView.isMine)
        {
            standardChart.transform.position = (sp || f) ? standardTransform.position : new Vector3(9999, 9999, 9999);
            splomChart.transform.position = spm ? splomTransform.position : new Vector3(99999, 9999, 9999);
            standardChart.transform.rotation = standardTransform.rotation;
            splomChart.transform.rotation = splomTransform.rotation;
        }
        else
        {
            photonView.RPC("ToggleState", originalOwner, sp, spm, f);
        }

        // This will get called twice but too lazy to fix
        photonView.RPC("ToggleButtons", PhotonTargets.All, sp, spm, f);
    }

    [PunRPC]
    private void ToggleButtons(bool sp, bool spm, bool f)
    {
        foreach (GameObject button in standardButtons.Concat(splomButtons).Concat(facetButtons))
        {
            bool isActive = (sp && standardButtons.Contains(button)) ||
                            (spm && splomButtons.Contains(button)) ||
                            (f && facetButtons.Contains(button));
            button.SetActive(isActive);
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

                case DashboardDimension.COLORBY:
                    if (dimensionName == "None")
                    {
                        standardChart.ColorDimension = "Undefined";
                        splomChart.ColorDimension = "Undefined";
                    }
                    else
                    {
                        standardChart.ColorDimension = dimensionName;
                        splomChart.ColorDimension = dimensionName;
                    }
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
            splomChart.Size = value;
        }
        // Otherwise, inform the original owner
        else
        {
            photonView.RPC("SizeSliderValueChanged", originalOwner, value);
        }
    }

    [PunRPC]
    public void RedSliderValueChanged(float value)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            Color color = standardChart.Color;
            color.r = value;

            standardChart.Color = color;
            splomChart.Color = color;
        }
        else
        {
            photonView.RPC("RedSliderValueChanged", originalOwner, value);
        }
    }

    [PunRPC]
    public void GreenSliderValueChanged(float value)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            Color color = standardChart.Color;
            color.g = value;

            standardChart.Color = color;
            splomChart.Color = color;
        }
        else
        {
            photonView.RPC("GreenSliderValueChanged", originalOwner, value);
        }
    }

    [PunRPC]
    public void BlueSliderValueChanged(float value)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            Color color = standardChart.Color;
            color.b = value;

            standardChart.Color = color;
            splomChart.Color = color;
        }
        else
        {
            photonView.RPC("BlueSliderValueChanged", originalOwner, value);
        }
    }

    [PunRPC]
    public void HueSliderValueChanged(float value)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            // Convert RGB to HSV, set hue, then convert back to RGB
            Color color = standardChart.Color;
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            color = Color.HSVToRGB(value, s, v);

            standardChart.Color = color;
            splomChart.Color = color;
        }
        else
        {
            photonView.RPC("HueSliderValueChanged", originalOwner, value);
        }
    }

    public void ColorPickerValueChanged(Color value)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            standardChart.Color = value;
            splomChart.Color = value;
        }
        else
        {
            photonView.RPC("ColorPickerValueChanged", originalOwner, value.r, value.g, value.b);
        }
    }

    [PunRPC]
    public void ColorPickerValueChanged(float r, float g, float b)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            Color color = new Color(r, g, b, 1);

            standardChart.Color = color;
            splomChart.Color = color;
        }
    }

    [PunRPC]
    public void ScatterplotMatrixSizeValueChanged(int value)
    {
        if (IsOriginalOwner())
        {
            ResetChartOwnership();

            splomChart.ScatterplotMatrixSize = value;
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
            splomChart.Gradient = gradient;
        }
        else
        {
            Dictionary<float, float[]> gradientDictionary = new Dictionary<float, float[]>();
            foreach (GradientColorKey colorKey in gradient.colorKeys)
            {
                gradientDictionary[colorKey.time] = new[] { colorKey.color.r, colorKey.color.g, colorKey.color.b };
            }

            photonView.RPC("ColorGradientChanged", originalOwner, gradientDictionary);
        }
    }

    /// <summary>
    /// Sets the gradient of the charts. This overload uses a converted gradient in the form of a dictionary for serialization
    /// </summary>
    /// <param name="gradientDictionary"></param>
    [PunRPC]
    private void ColorGradientChanged(Dictionary<float, float[]> gradientDictionary)
    {
        if (IsOriginalOwner())
        {
            // Create color keys from received dictionary
            List<GradientColorKey> colorKeys = new List<GradientColorKey>();

            foreach (float time in gradientDictionary.Keys)
            {
                Color color = new Color(gradientDictionary[time][0], gradientDictionary[time][1], gradientDictionary[time][2]);

                GradientColorKey colorKey = new GradientColorKey(color, time);
                colorKeys.Add(colorKey);
            }

            Gradient gradient = new Gradient();
            gradient.colorKeys = colorKeys.ToArray();

            ResetChartOwnership();

            standardChart.Gradient = gradient;
            splomChart.Gradient = gradient;
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
            photonViews = splomChart.GetComponentsInChildren<PhotonView>();
            foreach (PhotonView pv in photonViews)
            {
                if (!pv.isMine)
                    pv.TransferOwnership(PhotonNetwork.player);
            }
        }
    }
}
