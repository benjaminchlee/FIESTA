using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Brush : MonoBehaviour {

    BrushMode Mode = BrushMode.Select;
    GameObject InteractingController;

    public enum BrushMode
    {
        Select,
        Deselect
    }

    public void SetBrushMode(BrushMode mode)
    {
        Mode = mode;
    }

    public void SetInteractingController(GameObject interactingController)
    {
        InteractingController = interactingController;
    }
}
