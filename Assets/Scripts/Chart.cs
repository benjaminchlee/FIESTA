using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using VRTK;
using VRTK.GrabAttachMechanics;
using System;

/// <summary>
/// Acts as a wrapper for IATK's visualisation script
/// </summary>
//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(SpringJoint))]
//[RequireComponent(typeof(VRTK_MoveTransformGrabAttach))]
//[RequireComponent(typeof(VRTK_InteractableObject))]
//[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Visualisation))]
public class Chart : MonoBehaviour {

    private GameObject screen;
    private Visualisation visualisation;
    private VRTK_InteractableObject interactableObject;
    private VRTK_BaseGrabAttach grabAttach;
    private BoxCollider boxCollider;

    public bool isAnimating = false;
    private bool isAttached = false;
    private Coroutine activeCoroutine;

    /// <summary>
    /// Visualisation properties
    /// </summary>
    public DataSource DataSource
    {
        get { return visualisation.dataSource; }
        set { visualisation.dataSource = value; }
    }

    public AbstractVisualisation.VisualisationTypes VisualisationType
    {
        get { return visualisation.visualisationType; }
        set
        {
            if (value == AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX)
            {
                visualisation.zScatterplotMatrixDimensions = new DimensionFilter[0];
            }
            visualisation.CreateVisualisation(value);
        }
    }

    public AbstractVisualisation.GeometryType GeometryType
    {
        get { return visualisation.geometry; }
        set
        {
            visualisation.geometry = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.GeometryType);
        }
    }

    public string XDimension
    {
        get { return visualisation.xDimension.Attribute; }
        set
        {
            // Resize to zero to ensure uniform axes scaling
            Vector3 previousScale = transform.localScale;
            transform.localScale = Vector3.one;

            visualisation.xDimension = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.X);
            SetColliderBounds();

            // Resize back to original
            transform.localScale = previousScale;
        }
    }

    public string YDimension
    {
        get { return visualisation.yDimension.Attribute; }
        set
        {
            // Resize to zero to ensure uniform axes scaling
            Vector3 previousScale = transform.localScale;
            transform.localScale = Vector3.one;

            visualisation.yDimension = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Y);
            SetColliderBounds();

            // Resize back to original
            transform.localScale = previousScale;
        }
    }

    public string ZDimension
    {
        get { return visualisation.zDimension.Attribute; }
        set
        {
            // Resize to zero to ensure uniform axes scaling
            Vector3 previousScale = transform.localScale;
            transform.localScale = Vector3.one;

            visualisation.zDimension = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Z);
            SetColliderBounds();

            // Resize back to original
            transform.localScale = previousScale;
        }
    }

    public Color Color
    {
        get { return visualisation.colour; }
        set
        {
            visualisation.colour = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Colour);
        }
    }

    public float Size
    {
        get { return visualisation.size; }
        set
        {
            visualisation.size = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Size);
        }
    }

    public float Width
    {
        get { return visualisation.width; }
        set
        {
            visualisation.width = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.VisualisationWidth);
            SetColliderBounds();
        }
    }

    public float Height
    {
        get { return visualisation.height; }
        set
        {
            visualisation.height = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.VisualisationHeight);
            SetColliderBounds();
        }
    }

    public float Depth
    {
        get { return visualisation.depth; }
        set
        {
            visualisation.depth = value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.VisualisationLength);
            SetColliderBounds();
        }
    }


    public void Initialise(CSVDataSource dataSource)
    {
        visualisation = gameObject.GetComponent<Visualisation>();
        gameObject.tag = "Chart";

        DataSource = dataSource;

        screen = GameObject.FindGameObjectWithTag("Screen");
        
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

        // Subscribe to events
        interactableObject.InteractableObjectGrabbed += ChartGrabbed;
        interactableObject.InteractableObjectUngrabbed += ChartUngrabbed;

        // Add collider
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        // Configure rigidbody
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void ChartGrabbed(object sender, InteractableObjectEventArgs e)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        InteractionsManager.Instance.GrabbingStarted();  // TODO: FIX
    }

    private void ChartUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        InteractionsManager.Instance.GrabbingFinished();  // TODO: FIX
    }

    private void OnDestroy()
    {
        //Unsubscribe to events
        interactableObject.InteractableObjectGrabbed -= ChartGrabbed;
        interactableObject.InteractableObjectUngrabbed -= ChartUngrabbed;
    }

    private void Update()
    {
        // TODO: MAKE BETTER
        if (isAttached && interactableObject.IsGrabbed())
        {
            if (Vector3.Distance(screen.GetComponent<Collider>().ClosestPoint(gameObject.transform.TransformPoint(boxCollider.bounds.center)), gameObject.transform.TransformPoint(boxCollider.bounds.center)) > 0.25f)
            {
                DetachFromScreen();
            }
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

        // Calculate center
        float xCenter = (x != "Undefined") ? width / 2 : 0;
        float yCenter = (y != "Undefined") ? height / 2 : 0;
        float zCenter = (z != "Undefined") ? depth/ 2 : 0;

        // Calculate size
        float xSize = (x != "Undefined") ? width + 0.3f : 0.2f;
        float ySize = (y != "Undefined") ? height + 0.3f : 0.2f;
        float zSize = (z != "Undefined") ? depth + 0.3f : 0.2f;

        boxCollider.center = new Vector3(xCenter, yCenter, zCenter);
        boxCollider.size = new Vector3(xSize, ySize, zSize);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAnimating && !isAttached && other.tag == "Screen")
        {
            other.gameObject.GetComponent<Screen>().AttachChart(this);
            isAttached = true;
            AttachToScreen();
        }
    }

    public void AnimateTowards(Vector3 targetPos, float duration)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(AnimateTo(targetPos, duration));
    }

    public void AnimateTowards(GameObject targetObj, float duration)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(AnimateTo(targetObj, duration));
    }

    private IEnumerator AnimateTo(Vector3 targetPos, float duration)
    {
        SetAnimating(true);
        float time = 0;
        Vector3 startPos = gameObject.transform.position;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        while (time < duration)
        {
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / duration));
            time += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = gameObject.transform.position;
        SetAnimating(false);
    }

    private IEnumerator AnimateTo(GameObject targetObj, float duration)
    {

        SetAnimating(true);
        float time = 0;
        Vector3 startPos = gameObject.transform.position;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        while (time < duration)
        {
            rb.MovePosition(Vector3.Lerp(startPos, targetObj.transform.position, time / duration));
            time += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = targetObj.transform.position;
        SetAnimating(false);
    }

    private void SetAnimating(bool flag)
    {
        if (flag)
        {
            isAnimating = true;
            grabAttach.enabled = false;
        }
        else
        {
            isAnimating = false;
            grabAttach.enabled = true;
        }
    }

    private void AttachToScreen()
    {
        screen.GetComponent<Screen>().AttachChart(this);
        isAttached = true;
        
        //Destroy(grabAttach);
        //grabAttach = gameObject.AddComponent<VRTK_SpringJointGrabAttach>();
        //SpringJoint springJoint = gameObject.AddComponent<SpringJoint>();
        //springJoint.spring = 100000;
        //interactableObject.grabAttachMechanicScript = grabAttach;

        Vector3 newPos = screen.GetComponent<Collider>().ClosestPoint(gameObject.transform.TransformPoint(boxCollider.bounds.center));
        newPos = screen.transform.InverseTransformPoint(newPos);
        newPos.z = -0.025f;
        newPos = screen.transform.TransformPoint(newPos);
        AnimateTowards(newPos, 0.2f);
    }

    private void DetachFromScreen()
    {
        isAttached = false;

        //Destroy(grabAttach);
        //Destroy(gameObject.GetComponent<SpringJoint>());
        //grabAttach = gameObject.AddComponent<VRTK_ChildOfControllerGrabAttach>();
        //interactableObject.grabAttachMechanicScript = grabAttach;

        screen.GetComponent<Screen>().DetachChart(this);
        //AnimateTowards(interactableObject.GetGrabbingObject(), 0.2f);
    }
}
