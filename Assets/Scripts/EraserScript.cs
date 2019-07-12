using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class EraserScript : MonoBehaviour
{
    GameObject rc;

    VRTK_ControllerEvents rcCE;

    // Start is called before the first frame update
    void Start()
    {
        rc = GameObject.Find("RightController");
        rcCE = rc.GetComponent<VRTK_ControllerEvents>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Line")
        {
            if (rcCE.gripClicked)
            {
                Destroy(other.gameObject);
            }
        }
    }
}