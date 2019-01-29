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

public class WorkScreen : MonoBehaviour
{
    private DataSource dataSource;

    private Chart scatterplot;
    private Chart scatterplotMatrix;
    private Chart facet;

    [SerializeField]
    private Transform scatterplotTransform;
    [SerializeField]
    private Transform scatterplotMatrixTransform;
    [SerializeField]
    private Transform facetTransform;

    [SerializeField]
    private List<GameObject> scatterplotButtons;
    [SerializeField]
    private List<GameObject> scatterplotMatrixButtons;
    [SerializeField]
    private List<GameObject> facetButtons;
    
    private void Start()
    {
        if (dataSource == null)
            dataSource = ChartManager.Instance.DataSource;

        if (scatterplotButtons == null)
            scatterplotButtons = new List<GameObject>();

        if (scatterplotMatrixButtons == null)
            scatterplotMatrixButtons = new List<GameObject>();
        
        if (facetButtons == null)
            facetButtons = new List<GameObject>();

        // Configure scatterplot
        scatterplot = ChartManager.Instance.CreateVisualisation("WorkscreenScatterplot");
        scatterplot.VisualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT;
        scatterplot.GeometryType = AbstractVisualisation.GeometryType.Points;
        scatterplot.XDimension = dataSource[0].Identifier;
        scatterplot.YDimension = dataSource[0].Identifier;
        scatterplot.Width = scatterplotTransform.localScale.x;
        scatterplot.Height = scatterplotTransform.localScale.y;
        scatterplot.Depth = scatterplotTransform.localScale.z;
        scatterplot.transform.position = scatterplotTransform.position;
        scatterplot.transform.rotation = scatterplotTransform.rotation;
        scatterplot.SetAsPrototype();

        // Configure scatterplot matrix
        scatterplotMatrix = ChartManager.Instance.CreateVisualisation("WorkscreenScatterplotMatrix");
        scatterplotMatrix.VisualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX;
        scatterplotMatrix.GeometryType = AbstractVisualisation.GeometryType.Points;
        scatterplotMatrix.transform.position = scatterplotMatrixTransform.position;
        scatterplotMatrix.transform.rotation = scatterplotMatrixTransform.rotation;
        scatterplotMatrix.Width = scatterplotMatrixTransform.localScale.x;
        scatterplotMatrix.Height = scatterplotMatrixTransform.localScale.y;
        scatterplotMatrix.Depth = scatterplotMatrixTransform.localScale.z;

        // Configure facets
        facet = ChartManager.Instance.CreateVisualisation("WorkscreenFacet");
        facet.VisualisationType = AbstractVisualisation.VisualisationTypes.FACET;
        facet.GeometryType = AbstractVisualisation.GeometryType.Points;
        facet.XDimension = dataSource[0].Identifier;
        facet.YDimension = dataSource[0].Identifier;
        //facet.FacetDimension = "mpg";
        //facet.FacetSize = 5;
        facet.Width = facetTransform.localScale.x;
        facet.Height = facetTransform.localScale.y;
        facet.Depth = facetTransform.localScale.z;
        facet.transform.position = facetTransform.position;
        facet.transform.rotation = facetTransform.rotation;

        ShowScatterplot();
    }

    public void ShowScatterplot()
    {
        ToggleState(true, false, false);
    }

    public void ShowScatterplotMatrix()
    {
        ToggleState(false, true, false);
    }

    public void ShowFacet()
    {
        ToggleState(false, false, true);
    }

    private void ToggleState(bool sp, bool spm, bool f)
    {
        scatterplot.transform.position = sp ? scatterplotTransform.position : new Vector3(9999, 9999, 9999);
        scatterplotMatrix.transform.position = spm ? scatterplotMatrixTransform.position : new Vector3(99999, 9999, 9999);
        facet.transform.position = f ? facetTransform.position : new Vector3(9999, 9999, 9999);

        //scatterplot.gameObject.SetActive(sp);
        //scatterplotMatrix.gameObject.SetActive(spm);
        //facet.gameObject.SetActive(f);

        foreach (GameObject button in scatterplotButtons.Concat(scatterplotMatrixButtons).Concat(facetButtons))
        {
            bool isActive = (sp && scatterplotButtons.Contains(button)) ||
                            (spm && scatterplotMatrixButtons.Contains(button)) || 
                            (f && facetButtons.Contains(button));
            button.SetActive(isActive);
        }
    }

    public void DimensionChanged(WorkScreenDimension dimension, string dimensionName)
    {
        switch (dimension)
        {
            case WorkScreenDimension.X:
                scatterplot.XDimension = dimensionName;
                facet.XDimension = dimensionName;
                break;

            case WorkScreenDimension.Y:
                scatterplot.YDimension = dimensionName;
                facet.YDimension = dimensionName;
                break;

            case WorkScreenDimension.Z:
                scatterplot.ZDimension = dimensionName;
                facet.ZDimension = dimensionName;
                break;

            case WorkScreenDimension.FACETBY:
                facet.FacetDimension = dimensionName;
                break;
        }
    }

    public void SizeSliderValueChanged(float value)
    {
        scatterplot.Size = value;
        scatterplotMatrix.Size = value;
        facet.Size = value;
    }

    public void RedSliderValueChanged(float value)
    {
        Color color = scatterplot.Color;
        color.r = value;

        scatterplot.Color = color;
        scatterplotMatrix.Color = color;
        facet.Color = color;
    }

    public void GreenSliderValueChanged(float value)
    {
        Color color = scatterplot.Color;
        color.g = value;

        scatterplot.Color = color;
        scatterplotMatrix.Color = color;
        facet.Color = color;
    }

    public void BlueSliderValueChanged(float value)
    {
        Color color = scatterplot.Color;
        color.b = value;

        scatterplot.Color = color;
        scatterplotMatrix.Color = color;
        facet.Color = color;
    }

    public void ScatterplotMatrixSizeSlider(float value)
    {
        scatterplotMatrix.ScatterplotMatrixSize = (int)value;
    }

    public void FacetSizeSlider(float value)
    {
        facet.FacetSize = (int)value;
    }
}
