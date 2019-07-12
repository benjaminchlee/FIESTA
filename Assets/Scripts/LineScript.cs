using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class LineScript : MonoBehaviour
{
    GameObject lc;

    VRTK_ControllerEvents lcCE;

    // Start is called before the first frame update
    void Start()
    {
        lc = GameObject.Find("LeftController");
        lcCE = lc.GetComponent<VRTK_ControllerEvents>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lcCE.gripClicked)
        {
            DestroyGameObject();
        }
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
