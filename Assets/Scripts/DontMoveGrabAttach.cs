using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

/// <summary>
/// A dummy script which causes the grabbed gameobject to not move with the controller
/// </summary>
public class DontMoveGrabAttach : VRTK_BaseGrabAttach
{
    protected override void Initialise()
    {
        tracked = false;
        climbable = false;
        kinematic = false;
    }
    
    public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
    {
        return true;
    }
    
    public override void StopGrab(bool applyGrabbingObjectVelocity)
    {
        return;
    }
}
