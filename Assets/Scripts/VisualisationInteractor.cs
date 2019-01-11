using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using VRTK;
using VRTK.GrabAttachMechanics;

/// <summary>
/// Acts as a wrapper for IATK's visualisation script
/// </summary>
[RequireComponent(typeof(VRTK_InteractableObject))]
[RequireComponent(typeof(VRTK_ChildOfControllerGrabAttach))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Visualisation))]
public class VisualisationInteractor : MonoBehaviour {

    private Visualisation visualisation;
    private VRTK_InteractableObject interactableObject;
    private BoxCollider boxCollider;

    private Vector3 scale = new Vector3(0.25f, 0.25f, 0.25f);


    public void Initialise(CSVDataSource dataSource)
    {
        visualisation = gameObject.GetComponent<Visualisation>();

        SetDataSource(dataSource);

        // Set blank values
        visualisation.colourDimension = "Undefined";
        visualisation.sizeDimension = "Undefined";
        visualisation.linkingDimension = "Undefined";
        visualisation.colorPaletteDimension = "Undefined";

        // Add VRTK interactable scripts
        interactableObject = gameObject.GetComponent<VRTK_InteractableObject>();
        interactableObject.isGrabbable = true;
        interactableObject.grabAttachMechanicScript = gameObject.GetComponent<VRTK_ChildOfControllerGrabAttach>();
        interactableObject.grabAttachMechanicScript.precisionGrab = true;

        // Add collider
        boxCollider = gameObject.GetComponent<BoxCollider>();
    }

    public void SetDataSource(CSVDataSource dataSource)
    {
        if (visualisation != null)
        {
            visualisation.dataSource = dataSource;
        }
    }

    public void SetVisualisationType(AbstractVisualisation.VisualisationTypes visualisationType)
    {
        if (visualisation != null)
        {
            visualisation.CreateVisualisation(visualisationType);
        }
    }

    public void SetXDimension(string dimension)
    {
        if (visualisation != null)
        {
            // Resize back up to ensure uniform axes scaling
            gameObject.transform.localScale = Vector3.one;

            visualisation.xDimension = dimension;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.X);
            CalculateColliderBounds();

            // Resize back down
            gameObject.transform.localScale = scale;
        }
    }

    public void SetYDimension(string dimension)
    {
        if (visualisation != null)
        {
            gameObject.transform.localScale = Vector3.one;

            visualisation.yDimension = dimension;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Y);
            CalculateColliderBounds();

            gameObject.transform.localScale = scale;
        }
    }

    public void SetZDimension(string dimension)
    {
        if (visualisation != null)
        {
            gameObject.transform.localScale = Vector3.one;

            visualisation.zDimension = dimension;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Z);
            CalculateColliderBounds();

            gameObject.transform.localScale = scale;
        }
    }

    public void SetGeometry(AbstractVisualisation.GeometryType geometryType)
    {
        if (visualisation != null)
        {
            visualisation.geometry = geometryType;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.GeometryType);
        }
    }

    public void SetColor(Color color)
    {
        if (visualisation != null)
        {
            visualisation.colour = color;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Colour);
        }
    }

    private void CalculateColliderBounds()
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
}
