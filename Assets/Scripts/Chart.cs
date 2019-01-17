using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using VRTK;
using VRTK.GrabAttachMechanics;
using System;
using DG.Tweening;

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

    private DisplayScreen displayScreen;
    private Visualisation visualisation;
    private VRTK_InteractableObject interactableObject;
    private VRTK_BaseGrabAttach grabAttach;
    private BoxCollider boxCollider;
    private Rigidbody rigidbody;
    
    private bool isPrototype = false;
    private bool isThrowing = false;
    private bool isDestroying = false;
    private bool isTouchingDisplayScreen = false;

    private Vector3 originalPos;
    private Quaternion originalRot;

    private float deletionTimer = 0;

    #region VisualisationProperties

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
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.SizeValues);
        }
    }

    public float Width
    {
        get { return visualisation.width; }
        set
        {
            visualisation.width = value;

            // Update the axis object with the length
            GameObject axis = visualisation.theVisualizationObject.X_AXIS;
            if (axis != null)
                axis.GetComponent<Axis>().Length = value;

            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            SetColliderBounds();
        }
    }

    public float Height
    {
        get { return visualisation.height; }
        set
        {
            visualisation.height = value;

            // Update the axis object with the length
            GameObject axis = visualisation.theVisualizationObject.Y_AXIS;
            if (axis != null)
                axis.GetComponent<Axis>().Length = value;

            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            SetColliderBounds();
        }
    }

    public float Depth
    {
        get { return visualisation.depth; }
        set
        {
            visualisation.depth = value;

            // Update the axis object with the length
            GameObject axis = visualisation.theVisualizationObject.Z_AXIS;
            if (axis != null)
                axis.GetComponent<Axis>().Length = value;

            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            SetColliderBounds();
        }
    }

    #endregion

    public void Initialise(CSVDataSource dataSource)
    {
        visualisation = gameObject.GetComponent<Visualisation>();
        gameObject.tag = "Chart";

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

        // Subscribe to events
        interactableObject.InteractableObjectGrabbed += ChartGrabbed;
        interactableObject.InteractableObjectUngrabbed += ChartUngrabbed;

        // Add collider
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        // Configure rigidbody
        rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
    }

    private void ChartGrabbed(object sender, InteractableObjectEventArgs e)
    {
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
                isDestroying = true;
            }
            else
            {
                deletionTimer += Time.deltaTime;
            }
        }
        else if (isDestroying)
        {
            float size = transform.localScale.x;
            size -= 0.005f;

            if (size > 0)
            {
                transform.localScale = Vector3.one * size;
            }
            else
            {
                ChartManager.Instance.RemoveVisualisation(this);
                Destroy(gameObject);
            }
        }
    }

    public void SetAsPrototype()
    {
        isPrototype = true;

        originalPos = transform.position;
        originalRot = transform.rotation; 
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
        if (other.tag == "DisplayScreen")
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
        if (other.tag == "DisplayScreen")
        {
            isTouchingDisplayScreen = false;
        }
    }

    private void AttachToDisplayScreen()
    {
        Vector3 pos = displayScreen.CalculatePositionOnScreen(this);
        Quaternion rot = displayScreen.CalculateRotationOnScreen(this);

        AnimateTowards(pos, rot, 0.1f);
    }

    public void AnimateTowards(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        rigidbody.DOMove(targetPos, duration).SetEase(Ease.OutBack);
        rigidbody.DORotate(targetRot.eulerAngles, duration).SetEase(Ease.OutQuad);
    }
}
