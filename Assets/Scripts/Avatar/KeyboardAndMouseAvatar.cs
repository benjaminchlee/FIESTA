using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class KeyboardAndMouseAvatar : MonoBehaviourPunCallbacks {
    
    float mainSpeed = 1.25f; //regular speed
    float shiftAdd = 2f; //multiplied by how long shift is held.  Basically running
    float maxShift = 1000.0f; //Maximum speed when holding shift
    float camSens = 1f; //How sensitive it with mouse
    private float totalRun = 1.0f;

    private bool isEnabled = true;

    private void Start()
    {
        DontDestroyOnLoad(this);

        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
        }
        else
        {
            Cursor.lockState = isEnabled ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isEnabled;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isEnabled = !isEnabled;

                Cursor.lockState = isEnabled ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !isEnabled;
            }

            if (isEnabled)
            {
                //Mouse  camera angle
                transform.eulerAngles = new Vector3(transform.eulerAngles.x - Input.GetAxis("Mouse Y") * camSens, transform.eulerAngles.y + Input.GetAxis("Mouse X") * camSens, 0);

                //Keyboard commands
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
