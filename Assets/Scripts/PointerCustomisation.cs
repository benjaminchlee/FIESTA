using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PointerCustomisation : MonoBehaviour
{
    [HideInInspector]
    public bool showStickFlag = false;

    public Transform stick;

    private GameObject ball;
    private VRTK_ControllerEvents controllerEvents;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent.Find("RightController") != null) {
            controllerEvents = transform.parent.Find("RightController").GetComponent<VRTK_ControllerEvents>();
            controllerEvents.TouchpadPressed += Enable3DStick;
            controllerEvents.TouchpadReleased += Unable3DStick;
            controllerEvents.TriggerClicked += Enable3DStick;
            controllerEvents.TriggerUnclicked += Unable3DStick;
            controllerEvents.GripClicked += Enable3DStick;
            controllerEvents.GripUnclicked += Unable3DStick;
        }
        
        
    }

    private void Unable3DStick(object sender, ControllerInteractionEventArgs e)
    {
        if (showStickFlag)
        {
            if (stick.gameObject.activeSelf)
                stick.gameObject.SetActive(false); 
        }
        else {
            if (stick.gameObject.activeSelf)
                stick.gameObject.SetActive(false);
        }
        if (ball != null) {
            Destroy(ball);
        }
            
    }

    private void Enable3DStick(object sender, ControllerInteractionEventArgs e)
    {
        if (showStickFlag)
        {
            if (!stick.gameObject.activeSelf) {
                stick.gameObject.SetActive(true);
                if (ball == null)
                {
                    ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    ball.transform.position = new Vector3(0, 0, 0);
                    ball.name = "testingBall";
                    ball.transform.localScale = Vector3.one * 0.001f;
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (stick.gameObject.activeSelf) {
            if(ball != null)
                ball.transform.position = stick.GetChild(0).GetChild(0).position;
            //Debug.Log(stick.GetChild(0).GetChild(0).position);
        }
    }
}
