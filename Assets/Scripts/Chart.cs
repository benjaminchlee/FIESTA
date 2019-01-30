using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using VRTK;
using VRTK.GrabAttachMechanics;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Xml.Serialization;
using DG.Tweening;
using NetBase;
using TMPro;
using UnityEditorInternal.VersionControl;
using Component = UnityEngine.Component;

/// <summary>
/// Acts as a wrapper for IATK's visualisation script
/// </summary>
public class Chart : Photon.MonoBehaviour
{
    private Visualisation visualisation;
    private GameObject visualisationGameObject;
    private VRTK_InteractableObject interactableObject;
    private VRTK_BaseGrabAttach grabAttach;
    private BoxCollider boxCollider;
    private BoxCollider raycastCollider;
    private Rigidbody rigidbody;

    private Chart[,] splomCharts;  // Stored as 2D array
    private List<Chart> subCharts;  // Stored as 1D array
    private SPLOMButton[] splomButtons;
    private List<GameObject> facetLabels;

    private DisplayScreen displayScreen;

    private bool isPrototype = false;
    private bool isThrowing = false;
    private bool isTouchingDisplayScreen = false;

    private Vector3 originalPos;
    private Quaternion originalRot;

    private float deletionTimer = 0;

    #region VisualisationProperties

    public Visualisation Visualisation
    {
        get { return visualisation; }
    }

    public DataSource DataSource
    {
        get { return visualisation.dataSource; }
        set { visualisation.dataSource = value; }
    }

    /// <summary>
    /// ChartType is the type of the chart (the wrapper to the visualisation)
    /// </summary>
    private AbstractVisualisation.VisualisationTypes chartType;
    public AbstractVisualisation.VisualisationTypes VisualisationType
    {
        get { return chartType; }
        set
        {
            chartType = value;

            switch (value)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT:
                    visualisation.visualisationType = value;
                    visualisation.CreateVisualisation(value);
                    SetAsScatterplot();
                    break;

                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX:
                    SetAsScatterplotMatrix();
                    break;

                case AbstractVisualisation.VisualisationTypes.FACET:
                    SetAsFacet();
                    break;
            }
        }
    }

    public AbstractVisualisation.GeometryType GeometryType
    {
        get { return visualisation.geometry; }
        set
        {
            visualisation.geometry = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.GeometryType);

            switch (chartType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX:
                case AbstractVisualisation.VisualisationTypes.FACET:
                    foreach (Chart chart in subCharts)
                        chart.GeometryType = value;
                    break;
            }
        }
    }

    public string XDimension
    {
        get { return visualisation.xDimension.Attribute; }
        set
        {
            visualisation.xDimension = value;

            switch (chartType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT:
                    visualisation.updateViewProperties(AbstractVisualisation.PropertyType.X);
                    CenterVisualisation();
                    SetColliderBounds();
                    break;

                case AbstractVisualisation.VisualisationTypes.FACET:
                    foreach (Chart chart in subCharts)
                    {
                        chart.XDimension = value;
                    }
                        
                    break;
            }
        }
    }

    public string YDimension
    {
        get { return visualisation.yDimension.Attribute; }
        set
        {
            visualisation.yDimension = value;

            switch (chartType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT:
                    visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Y);
                    CenterVisualisation();
                    SetColliderBounds();
                    break;

                case AbstractVisualisation.VisualisationTypes.FACET:
                    foreach (Chart chart in subCharts)
                    {
                        chart.YDimension = value;
                    }
                    break;
            }
        }
    }

    public string ZDimension
    {
        get { return visualisation.zDimension.Attribute; }
        set
        {
            visualisation.zDimension = value;

            switch (chartType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT:
                    visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Z);
                    CenterVisualisation();
                    SetColliderBounds();
                    break;

                case AbstractVisualisation.VisualisationTypes.FACET:
                    foreach (Chart chart in subCharts)
                    {
                        chart.ZDimension = value;
                    }
                    break;
            }
        }
    }

    public Color Color
    {
        get { return visualisation.colour; }
        set
        {
            visualisation.colour = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Colour);

            switch (chartType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX:
                case AbstractVisualisation.VisualisationTypes.FACET:
                    foreach (Chart chart in subCharts)
                        chart.Color = value;
                    break;
            }
        }
    }

    public float Size
    {
        get { return visualisation.size; }
        set
        {
            visualisation.size = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.SizeValues);

            switch (chartType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX:
                case AbstractVisualisation.VisualisationTypes.FACET:
                    foreach (Chart chart in subCharts)
                        chart.Size = value;
                    break;
            }
        }
    }

    public float Width
    {
        get { return visualisation.width; }
        set
        {
            visualisation.width = value;
            CenterVisualisation();
            SetColliderBounds();

            switch (chartType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT:
                    // Update the axis object with the length
                    GameObject axis = visualisation.theVisualizationObject.X_AXIS;
                    if (axis != null)
                    {
                        axis.GetComponent<Axis>().Length = value;
                        axis.GetComponent<Axis>().UpdateLength();
                    }

                    visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
                    ForceViewScale();
                    break;

                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX:
                    ResizeAndPositionScatterplotMatrix();
                    break;

                case AbstractVisualisation.VisualisationTypes.FACET:
                    AdjustAndUpdateFacet();
                    break;
            }
        }
    }

    public float Height
    {
        get { return visualisation.height; }
        set
        {
            visualisation.height = value;
            CenterVisualisation();
            SetColliderBounds();

            switch (chartType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT:
                    // Update the axis object with the length
                    GameObject axis = visualisation.theVisualizationObject.Y_AXIS;
                    if (axis != null)
                    {
                        axis.GetComponent<Axis>().Length = value;
                        axis.GetComponent<Axis>().UpdateLength();
                    }

                    visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
                    ForceViewScale();
                    break;

                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX:
                    ResizeAndPositionScatterplotMatrix();
                    break;

                case AbstractVisualisation.VisualisationTypes.FACET:
                    AdjustAndUpdateFacet();
                    break;
            }
        }
    }

    public float Depth
    {
        get { return visualisation.depth; }
        set
        {
            visualisation.depth = value;
            CenterVisualisation();
            SetColliderBounds();

            switch (chartType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT:
                    // Update the axis object with the length
                    GameObject axis = visualisation.theVisualizationObject.Z_AXIS;
                    if (axis != null)
                    {
                        axis.GetComponent<Axis>().Length = value;
                        axis.GetComponent<Axis>().UpdateLength();
                    }

                    visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
                    ForceViewScale();
                    break;

                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX:
                    ResizeAndPositionScatterplotMatrix();
                    break;

                case AbstractVisualisation.VisualisationTypes.FACET:
                    AdjustAndUpdateFacet();
                    break;
            }
        }
    }

    public AttributeFilter[] AttributeFilters
    {
        get { return visualisation.attributeFilters; }
        set
        {
            visualisation.attributeFilters = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.AttributeFiltering);
        }
    }

    private int scatterplotMatrixSize = 3;
    public int ScatterplotMatrixSize
    {
        get { return scatterplotMatrixSize; }
        set
        {
            if (scatterplotMatrixSize < 2)
                value = 2;
            else if (scatterplotMatrixSize > DataSource.DimensionCount)
                value = DataSource.DimensionCount;

            if (VisualisationType == AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX)
            {
                scatterplotMatrixSize = value;
                AdjustScatterplotMatrixSize();
            }
        }
    }

    private string facetDimension;
    public string FacetDimension
    {
        get { return facetDimension; }
        set
        {
            facetDimension = value;

            if (VisualisationType == AbstractVisualisation.VisualisationTypes.FACET)
            {
                AdjustAndUpdateFacet();
            }
        }
    }

    private int facetSize = 1;
    public int FacetSize
    {
        get { return facetSize; }
        set
        {
            if (0 < facetSize)
            {
                facetSize = value;

                if (VisualisationType == AbstractVisualisation.VisualisationTypes.FACET)
                {
                    AdjustAndUpdateFacet();
                }
            }
        }
    }

#endregion

    public void Initialise(CSVDataSource dataSource)
    {
        gameObject.tag = "Chart";

        visualisationGameObject = new GameObject("Visualisation");
        visualisationGameObject.transform.SetParent(transform);

        visualisation = visualisationGameObject.AddComponent<Visualisation>();

        DataSource = dataSource;
        displayScreen = GameObject.FindGameObjectWithTag("DisplayScreen").GetComponent<DisplayScreen>();
        
        // Set blank values
        visualisation.colourDimension = "Undefined";
        visualisation.sizeDimension = "Undefined";
        visualisation.linkingDimension = "Undefined";
        visualisation.colorPaletteDimension = "Undefined";

        // Add VRTK interactable scripts
        interactableObject = gameObject.AddComponent<VRTK_InteractableObject>();
        interactableObject.isGrabbable = true;
        grabAttach = gameObject.AddComponent<VRTK_ChildOfControllerGrabAttach>();
        interactableObject.grabAttachMechanicScript = grabAttach;
        interactableObject.grabAttachMechanicScript.precisionGrab = true;

        // Add Photon scripts for networking
        PhotonView photonView = gameObject.AddComponent<PhotonView>();
        photonView.viewID = PhotonNetwork.AllocateViewID();
        photonView.synchronization = ViewSynchronization.UnreliableOnChange;

        NetworkObject networkObject = gameObject.AddComponent<NetworkObject>();
        networkObject.parent = true;
        networkObject.onChangeOnly = false;

        photonView.ObservedComponents = new List<Component>() {networkObject, this};

        // Subscribe to events
        interactableObject.InteractableObjectGrabbed += ChartGrabbed;
        interactableObject.InteractableObjectUngrabbed += ChartUngrabbed;

        // Add collider and ignore raycasts (child object will handle raycasts)
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        gameObject.layer = 2;

        // Add collider to be used for raycasting
        GameObject go = new GameObject("RaycastCollider");
        go.tag = "ChartRaycastCollider";
        go.transform.SetParent(transform);
        raycastCollider = go.AddComponent<BoxCollider>();
        raycastCollider.isTrigger = true;
        interactableObject.ignoredColliders = new Collider[] { raycastCollider };

        // Configure rigidbody
        rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
    }

    private void SetAsScatterplot()
    {
        // Enable the visualisation
        visualisationGameObject.SetActive(true);

        // Enable this collider
        boxCollider.enabled = true;

        interactableObject.isGrabbable = true;

        //// Destroy scatterplot matrix gameobjects
        //for (int i = 0; i < splomCharts.GetLength(0); i++)
        //    for (int j = 0; j < splomCharts.GetLength(1); j++)
        //        Destroy(splomCharts[i, j]);
    }

    private void SetAsScatterplotMatrix()
    {
        // Disable the visualisation
        visualisationGameObject.SetActive(false);

        // Disable this collider
        boxCollider.enabled = false;

        interactableObject.isGrabbable = false;

        // Create scatterplot matrix gameobjects
        int nbDimensions = DataSource.DimensionCount;
        splomCharts = new Chart[nbDimensions, nbDimensions];
        subCharts = new List<Chart>();
        splomButtons = new SPLOMButton[nbDimensions];

        AdjustScatterplotMatrixSize();

        ResizeAndPositionScatterplotMatrix();
    }

    private void SetAsFacet()
    {
        // Disable the visualisation
        visualisationGameObject.SetActive(false);

        // Disable this collider
        boxCollider.enabled = false;

        interactableObject.isGrabbable = false;

        // Create facet gameobjects and labels
        subCharts = new List<Chart>();
        facetLabels = new List<GameObject>();

        // Set default faceted dimension
        facetDimension = DataSource[0].Identifier;

        AdjustAndUpdateFacet();
    }

    /// <summary>
    /// Adjusts the number of dimensions by creating/removing subcharts
    /// </summary>
    private void AdjustScatterplotMatrixSize()
    {
        // Create scatterplot matrix gameobjects
        int nbDimensions = DataSource.DimensionCount;

        for (int i = 0; i < nbDimensions; i++)
        {
            for (int j = 0; j < nbDimensions; j++)
            {
                // Only add/modify a subchart if [i,j] are smaller than the SPLOM's size
                if (i < scatterplotMatrixSize && j < scatterplotMatrixSize)
                {
                    Chart subChart = splomCharts[i, j];
                    // Only create an object if there wasn't already one
                    if (subChart == null)
                    {
                        // If not along the diagonal, create a chart
                        if (i != j)
                        {
                            subChart = ChartManager.Instance.CreateVisualisation(DataSource[i].Identifier + ", " + DataSource[j].Identifier);

                            subChart.VisualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT;
                            subChart.GeometryType = GeometryType;
                            // Get the x and y dimension from the splom button if it exists, otherwise use default
                            subChart.XDimension = (splomButtons[i] != null) ? splomButtons[i].Text : DataSource[i].Identifier;
                            subChart.YDimension = (splomButtons[j] != null) ? splomButtons[j].Text : DataSource[j].Identifier;
                            subChart.Color = Color;
                            subChart.SetAsPrototype();

                            splomCharts[i, j] = subChart;
                            subChart.transform.SetParent(transform);
                            subCharts.Add(subChart);
                        }
                        // If along the diagonal, create a blank chart (only axis labels with no geometry or collider) and a SPLOM button
                        else
                        {
                            subChart = ChartManager.Instance.CreateVisualisation(DataSource[i].Identifier + ", " + DataSource[j].Identifier);
                            // Get the x and y dimension from the splom button if it exists, otherwise use default
                            subChart.XDimension = (splomButtons[i] != null) ? splomButtons[i].Text : DataSource[i].Identifier;
                            subChart.YDimension = (splomButtons[j] != null) ? splomButtons[j].Text : DataSource[j].Identifier;
                            subChart.GetComponent<Collider>().enabled = false;
                            subChart.transform.SetParent(transform);
                            splomCharts[i, j] = subChart;

                            GameObject go = Instantiate((GameObject)Resources.Load("SPLOMButton"));
                            SPLOMButton button = go.GetComponent<SPLOMButton>();
                            button.SPLOMButtonClicked.AddListener(ScatterplotMatrixDimensionChanged);
                            button.Text = DataSource[i].Identifier;
                            splomButtons[i] = button;
                            go.transform.SetParent(transform);
                        }
                    }

                    // Hide the axis for all but the charts along the edge
                    bool isAlongLeft = (i == 0);
                    bool isAlongBottom = (j == scatterplotMatrixSize - 1);
                    GameObject xAxis = subChart.visualisation.theVisualizationObject.X_AXIS;
                    GameObject yAxis = subChart.visualisation.theVisualizationObject.Y_AXIS;

                    if (xAxis != null && yAxis != null)
                    {
                        xAxis.SetActive(isAlongBottom);
                        yAxis.SetActive(isAlongLeft);
                    }
                }
                // If it is larger, delete any charts if there were any
                else
                {
                    Chart chart = splomCharts[i, j];

                    if (chart != null)
                    {
                        subCharts.Remove(chart);
                        ChartManager.Instance.RemoveVisualisation(chart);
                    }
                }
            }
        }
        
        // Remove any extra splom buttons
        for (int i = scatterplotMatrixSize; i < splomButtons.Length; i++)
        {
            if (splomButtons[i] != null)
            {
                Destroy(splomButtons[i].gameObject);
                splomButtons[i] = null;
            }
        }

        ResizeAndPositionScatterplotMatrix();
    }

    /// <summary>
    /// Resizes and positions the subcharts in the scatterplot matrix
    /// </summary>
    private void ResizeAndPositionScatterplotMatrix()
    {
        if (chartType == AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX && splomCharts != null)
        {
            for (int i = 0; i < scatterplotMatrixSize; i++)
            {
                // Resize splom button
                float w = Width / scatterplotMatrixSize;
                float h = Height / scatterplotMatrixSize;
                float d = Depth / scatterplotMatrixSize;

                // Get the distance from the edge to the centre-point (tentatively called etcp) of each subchart (x and y deltas are the same)
                float delta = Width / (scatterplotMatrixSize * 2);

                // Position such that subcharts start from 1 etcp from the edge, and are spaced two etcp distances from each other
                float x = (-(Width / 2) + delta) + 2 * delta * i;
                float y = ((Height / 2) - delta) - 2 * delta * i;

                SPLOMButton splomButton = splomButtons[i];
                splomButton.transform.localPosition = new Vector3(x, y, 0);
                splomButton.transform.rotation = transform.rotation;
                splomButton.transform.localScale = new Vector3(w * 0.9f, h * 0.9f, 0.05f);

                // Resize charts
                for (int j = 0; j < scatterplotMatrixSize; j++)
                {
                    y = ((Height / 2) - delta) - 2 * delta * j;

                    Chart subChart = splomCharts[i, j];
                    subChart.Width = w * 0.75f;
                    subChart.Height = h * 0.75f;
                    subChart.Depth = d * 0.75f;
                    subChart.transform.localPosition = new Vector3(x, y, 0);
                    subChart.transform.rotation = transform.rotation;
                }
            }
        }
    }

    /// <summary>
    /// Readjust the dimensions of the scatterplot matrix
    /// </summary>
    /// <param name="button"></param>
    private void ScatterplotMatrixDimensionChanged(SPLOMButton button)
    {
        // Find which index the button belongs to (along the diagonal)
        int index = Array.IndexOf(splomButtons, button);

        // Change y-axis of charts along SPLOM's horizontal
        for (int i = 0; i < scatterplotMatrixSize; i++)
        {
            if (splomCharts[i, index].tag == "Chart")
            {
                splomCharts[i, index].GetComponent<Chart>().YDimension = button.Text;
            }
        }

        // Change x-axis of charts along SPLOM's vertical
        for (int i = 0; i < scatterplotMatrixSize; i++)
        {
            if (splomCharts[index, i].tag == "Chart")
            {
                splomCharts[index, i].GetComponent<Chart>().XDimension = button.Text;
            }
        }
    }

    private void AdjustAndUpdateFacet()
    {
        for (int i = 0; i < facetSize; i++)
        {
            Chart subChart = subCharts.ElementAtOrDefault(i);

            // Create a chart if it does not already yet exist
            if (subChart == null)
            {
                subChart = ChartManager.Instance.CreateVisualisation(i.ToString());

                subChart.VisualisationType = AbstractVisualisation.VisualisationTypes.SCATTERPLOT;
                subChart.GeometryType = GeometryType;
                subChart.XDimension = XDimension;
                subChart.YDimension = YDimension;
                subChart.Color = Color;
                subChart.SetAsPrototype();

                // Create attribute filters for each chart
                AttributeFilter af = new AttributeFilter();
                subChart.AttributeFilters = new AttributeFilter[] {af};

                subChart.transform.SetParent(transform);
                subCharts.Add(subChart);
            }

            // Set ranges for the chart
            AttributeFilter filter = subChart.AttributeFilters[0];
            filter.Attribute = facetDimension;
            filter.minFilter = 0;
            filter.maxFilter = 1;
            filter.minScale = i / (float)facetSize;
            filter.maxScale = (i + 1) / (float)facetSize;
            subChart.AttributeFilters[0] = filter;
            subChart.visualisation.updateViewProperties(AbstractVisualisation.PropertyType.AttributeFiltering);
        }

        // Destroy all previous labels (lazy)
        foreach (GameObject label in facetLabels)
        {
            Destroy(label);
        }

        // Position the charts
        int index = 0;
        int numRows = (int)Mathf.Sqrt(facetSize);
        int numCols = (int)Mathf.Ceil(facetSize / (float)numRows);

        // Get the distance from the edge to the centre-point (tentatively called etcp) of each subchart
        float xDelta = Width / (numCols * 2);
        float yDelta = Height / (numRows * 2);

        float w = Width / numCols;
        float h = Height / numRows;

        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                if (index < facetSize)
                {
                    Chart subChart = subCharts[index];
                    // Position such that subcharts start from 1 etcp from the edge, and are spaced two etcp distances from each other
                    float x = (-(Width / 2) + xDelta) + 2 * xDelta * j;
                    float y = ((Height / 2) - yDelta) - 2 * yDelta * i;
                    subChart.Width = w * 0.75f;
                    subChart.Height = h * 0.65f;
                    subChart.transform.localPosition = new Vector3(x, y, 0);
                    subChart.transform.rotation = transform.rotation;

                    // Create and position labels
                    GameObject labelHolder = new GameObject();
                    facetLabels.Add(labelHolder);
                    labelHolder.transform.SetParent(transform);
                    labelHolder.transform.localPosition = new Vector3(x, y + yDelta * 0.9f, 0);
                    labelHolder.transform.rotation = transform.rotation;

                    TextMeshPro label = labelHolder.AddComponent<TextMeshPro>();
                    label.fontSize = 0.25f;
                    label.alignment = TextAlignmentOptions.Center;

                    string range1 = DataSource.getOriginalValue(index / (float) facetSize, FacetDimension).ToString();
                    string range2 = DataSource.getOriginalValue((index + 1) / (float) facetSize, FacetDimension).ToString();
                    label.text = range1 + " ... " + range2;

                    RectTransform container = labelHolder.GetComponent<RectTransform>();
                    container.sizeDelta = new Vector2(w, 0.1f);

                    // Hide the axis for all but the charts along the edge
                    bool isAlongLeft = (j == 0);
                    bool isAlongBottom = (i == numRows - 1);
                    GameObject xAxis = subChart.visualisation.theVisualizationObject.X_AXIS;
                    GameObject yAxis = subChart.visualisation.theVisualizationObject.Y_AXIS;

                    // If the the index of a subchart below this one would be larger than the total number of subcharts, then it does not exist, therefore
                    // this subchart is along the bottom
                    int count = index + 1 + numCols;
                    if (count > facetSize)
                        isAlongBottom = true;

                    if (xAxis != null && yAxis != null)
                    {
                        xAxis.SetActive(isAlongBottom);
                        yAxis.SetActive(isAlongLeft);
                    }
                }
                index++;
            }
        }

        // Destroy any extra subcharts
        while (facetSize < subCharts.Count)
        {
            ChartManager.Instance.RemoveVisualisation(subCharts[subCharts.Count - 1]);
            subCharts.Remove(subCharts[subCharts.Count-1]);
        }
    }

    private void ChartGrabbed(object sender, InteractableObjectEventArgs e)
    {
        if (isPrototype)
        {
            originalPos = transform.position;
            originalRot = transform.rotation;
        }

        rigidbody.isKinematic = false;
        InteractionsManager.Instance.GrabbingStarted();  // TODO: FIX
    }

    private void ChartUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        InteractionsManager.Instance.GrabbingFinished(); // TODO: FIX

        // Animate the work shelf prototype back to its position
        if (isPrototype)
        {
            rigidbody.isKinematic = true;

            AnimateTowards(originalPos, originalRot, 0.2f);
        }
        else
        {
            // Check to see if the chart was thrown
            Vector3 velocity = VRTK_DeviceFinder.GetControllerVelocity(VRTK_ControllerReference.GetControllerReference(e.interactingObject));
            float speed = velocity.magnitude;

            if (speed > 2.5f)
            {
                rigidbody.useGravity = true;
                isThrowing = true;
                deletionTimer = 0;
            }
            else
            {
                rigidbody.isKinematic = true;

                // If it wasn't thrown, check to see if it is being placed on the display screen
                if (isTouchingDisplayScreen)
                {
                    AttachToDisplayScreen();
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        //Unsubscribe to events
        interactableObject.InteractableObjectGrabbed -= ChartGrabbed;
        interactableObject.InteractableObjectUngrabbed -= ChartUngrabbed;
    }

    private void Update()
    {
        if (isPrototype && interactableObject.IsGrabbed())
        {
            if (Vector3.Distance(transform.position, originalPos) > 0.25f)
            {
                // Create a duplicate of this visualisation
                Chart dupe = ChartManager.Instance.DuplicateVisualisation(this);

                VRTK_InteractTouch interactTouch = interactableObject.GetGrabbingObject().GetComponent<VRTK_InteractTouch>();
                VRTK_InteractGrab interactGrab = interactableObject.GetGrabbingObject().GetComponent<VRTK_InteractGrab>();
                
                // Drop this visualisation (it wil return automatically)
                interactGrab.ForceRelease();

                // Grab the duplicate
                interactTouch.ForceTouch(dupe.gameObject);
                interactGrab.AttemptGrab();
            }
        }
        else if (isThrowing)
        {
            if (1 < deletionTimer)
            {
                isThrowing = false;
                transform.DOScale(0, 1f).OnComplete(() => ChartManager.Instance.RemoveVisualisation(this));
            }
            else
            {
                deletionTimer += Time.deltaTime;
            }
        }
    }

    public void SetAsPrototype()
    {
        isPrototype = true;
    }

    public bool IsPrototype()
    {
        return isPrototype;
    }


    private void ForceViewScale()
    {
        foreach (View view in visualisation.theVisualizationObject.viewList)
        {
            view.transform.localScale = new Vector3(
                visualisation.width,
                visualisation.height,
                visualisation.depth
            );
        }
    }

    /// <summary>
    /// Sets the size of the collider based on the size and dimensions stored in the Visualisation. This should be called whenever a dimension is added/changed or when
    /// the size is changed.
    /// </summary>
    private void SetColliderBounds()
    {
        float width = visualisation.width;
        float height = visualisation.height;
        float depth = visualisation.depth;

        string x = visualisation.xDimension.Attribute;
        string y = visualisation.yDimension.Attribute;
        string z = visualisation.zDimension.Attribute;
        
        // Calculate size
        float xSize = (x != "Undefined") ? width + 0.15f : 0.1f;
        float ySize = (y != "Undefined") ? height + 0.15f : 0.1f;
        float zSize = (z != "Undefined") ? depth + 0.15f : 0.1f;
        
        boxCollider.size = new Vector3(xSize, ySize, zSize);
        raycastCollider.size = new Vector3(xSize, ySize, 0.01f);

        // Disable colliders if this is not a scatterplot
        if (VisualisationType != AbstractVisualisation.VisualisationTypes.SCATTERPLOT)
        {
            boxCollider.enabled = false;
            raycastCollider.enabled = false;
        }
    }

    private void CenterVisualisation()
    {
        float x = (XDimension != "Undefined") ? -Width / 2 : 0;
        float y = (YDimension != "Undefined") ? -Height / 2 : 0;
        float z = (ZDimension != "Undefined") ? -Depth / 2 : 0;

        visualisationGameObject.transform.DOLocalMove(new Vector3(x, y, z), 0.1f).SetEase(Ease.OutCubic);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DisplayScreen"))
        {
            isTouchingDisplayScreen = true;

            // If the chart was thrown at the screen, attach it to the screen
            if (isThrowing)
            {
                isThrowing = false;
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
                AttachToDisplayScreen();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DisplayScreen"))
        {
            isTouchingDisplayScreen = false;
        }
    }

    private void AttachToDisplayScreen()
    {
        Vector3 pos = displayScreen.CalculatePositionOnScreen(this);
        Quaternion rot = displayScreen.CalculateRotationOnScreen(this);

        AnimateTowards(pos, rot, 0.2f);
    }

    public void AnimateTowards(Vector3 targetPos, Quaternion targetRot, float duration, bool toDestroy = false)
    {
        if (toDestroy)
            rigidbody.DOMove(targetPos, duration).SetEase(Ease.OutQuint).OnComplete(() => ChartManager.Instance.RemoveVisualisation(this));
        else
            rigidbody.DOMove(targetPos, duration).SetEase(Ease.OutQuint);

        rigidbody.DORotate(targetRot.eulerAngles, duration).SetEase(Ease.OutQuint);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(VisualisationType);
            stream.SendNext(GeometryType);
            stream.SendNext(XDimension);
            stream.SendNext(YDimension);
            stream.SendNext(ZDimension);
            stream.SendNext(Color.r);
            stream.SendNext(Color.g);
            stream.SendNext(Color.b);
            stream.SendNext(Color.a);
            stream.SendNext(Size);
            stream.SendNext(Width);
            stream.SendNext(Height);
            stream.SendNext(Depth);
            //stream.SendNext(AttributeFilters);
            stream.SendNext(ScatterplotMatrixSize);
            stream.SendNext(FacetDimension);
            stream.SendNext(FacetSize);
        }
        else
        {
            VisualisationType = (AbstractVisualisation.VisualisationTypes) stream.ReceiveNext();
            GeometryType = (AbstractVisualisation.GeometryType) stream.ReceiveNext();
            XDimension = (string) stream.ReceiveNext();
            YDimension = (string) stream.ReceiveNext();
            ZDimension = (string) stream.ReceiveNext();
            Color col = new Color((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
            Size = (float) stream.ReceiveNext();
            Width = (float) stream.ReceiveNext();
            Height = (float) stream.ReceiveNext();
            Depth = (float) stream.ReceiveNext();
            //AttributeFilters = (AttributeFilters[]) stream.ReceiveNext();
            ScatterplotMatrixSize = (int) stream.ReceiveNext();
            FacetDimension = (string) stream.ReceiveNext();
            FacetSize = (int) stream.ReceiveNext();
        }
    }
}
