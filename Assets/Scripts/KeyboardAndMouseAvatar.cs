using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardAndMouseAvatar : Photon.MonoBehaviour {
    
    float mainSpeed = 1.25f; //regular speed
    float shiftAdd = 2f; //multiplied by how long shift is held.  Basically running
    float maxShift = 1000.0f; //Maximum speed when holdin gshift
    float camSens = 0.1f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;

    private bool isEnabled = false;

    private void Start()
    {
        DontDestroyOnLoad(this);

        if (!photonView.isMine)
        {
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
        }
    }

    private void Update()
    {
        if (photonView.isMine)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                isEnabled = !isEnabled;

            if (isEnabled)
            {
                lastMouse = Input.mousePosition - lastMouse;
                lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
                lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y,
                    0);
                transform.eulerAngles = lastMouse;
                lastMouse = Input.mousePosition;
                //Mouse  camera angle done.  

                //Keyboard commands
                float f = 0.0f;
                Vector3 p = GetBaseInput();
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    totalRun += Time.deltaTime;
                    p = p * totalRun * shiftAdd;
                    p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                    p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                    p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
                }
                else
                {
                    totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                    p = p * mainSpeed;
                }

                p = p * Time.deltaTime;
                
                float prevY = transform.position.y;
                transform.Translate(new Vector3(p.x, 0, p.z));
                transform.position = new Vector3(transform.position.x, prevY, transform.position.z);
                transform.position += Vector3.up * p.y;
            
                //Vector3 newPosition = transform.position;
                ////if (Input.GetKey(KeyCode.Space))
                //if (true)
                //{
                //    //If player wants to move on X and Z axis only
                //    transform.Translate(p);
                //    newPosition.x = transform.position.x;
                //    newPosition.z = transform.position.z;
                //    transform.position = newPosition;
                //}
                //else
                //{
                //    transform.Translate(p);
                //}
            }
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            p_Velocity += new Vector3(0, -0.5f, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            p_Velocity += new Vector3(0, 0.5f, 0);
        }
        return p_Velocity;
    }
}
