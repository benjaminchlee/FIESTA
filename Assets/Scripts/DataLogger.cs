using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class DataLogger : Photon.PunBehaviour {

    [Serializable]
    public struct Task
    {
        public string taskName;
        public string taskDescription;
    }

    public static DataLogger Instance { get; private set; }
    
    [SerializeField]    
    public float timeBetweenLogs = 0.050f;
    [SerializeField]
    public string filePath = "Assets/Resources/Logs/";
    [SerializeField]
    public int groupID;
    [SerializeField]
    public int participantID;
    [SerializeField]
    public bool isMasterLogger = false;
    [SerializeField]
    public List<Task> tasks;

    [HideInInspector]
    public int taskID;

    private StreamWriter playerStreamWriter;
    private StreamWriter actionsStreamWriter;
    private StreamWriter objectStreamWriter;
    private bool isLogging = false;
    private float time;

    private double startTime;
    private Transform headset;
    private Transform leftController;
    private Transform rightController;
    private VRTK_ControllerEvents leftControllerEvents;
    private VRTK_ControllerEvents rightControllerEvents;

    private List<Dashboard> dashboards;
    private TextMeshPro textMesh;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += FindTaskText;
    }

    private void FindTaskText(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "MainScene")
        {
            textMesh = GameObject.FindGameObjectWithTag("TaskText").GetComponent<TextMeshPro>();
        }
    }

    public void StartLogging()
    {
        if (isMasterLogger)
        {
            photonView.RPC("StartLogging", PhotonTargets.AllViaServer, groupID, taskID);
        }
    }

    [PunRPC]
    private void StartLogging(int groupID, int taskID, PhotonMessageInfo info)
    {
        this.taskID = taskID;
        isLogging = true;
        startTime = info.timestamp;

        if (!filePath.EndsWith("\\"))
            filePath += "\\";

        string path = string.Format("{0}Group{1}_Participant{2}_PhotonPlayer{3}_Task{4}_PlayerData.txt", filePath, groupID, participantID, PhotonNetwork.player.ID, tasks[taskID].taskName);
        playerStreamWriter = new StreamWriter(path, true);

        // Write header
        playerStreamWriter.WriteLine("Timestamp\tHeadPosition\tHeadRotation\t" +
                                 "LeftPosition\tLeftRotation\tLeftTrigger\tLeftGrip\tLeftTouchpad\tLeftTouchpadAngle" +
                                 "RightPosition\tRightRotation\tRightTrigger\tRightGrip\tRightTouchpad\tRightTouchpadAngle");

        path = string.Format("{0}Group{1}_Participant{2}_PhotonPlayer{3}_Task{4}_ActionData.txt", filePath, groupID, participantID, PhotonNetwork.player.ID, tasks[taskID].taskName);
        actionsStreamWriter = new StreamWriter(path, true);

        // Write header
        actionsStreamWriter.WriteLine("Timestamp\tObjectType\tOriginalOwner\tName\tDescription");

        if (isMasterLogger)
        {
            path = string.Format("{0}Group{1}_Participant{2}_PhotonPlayer{3}_Task{4}_ObjectData.txt", filePath, groupID, participantID, PhotonNetwork.player.ID, tasks[taskID].taskName);
            objectStreamWriter = new StreamWriter(path, true);

            // Write header
            objectStreamWriter.WriteLine("Timestamp\tObjectType\tOwner\tPosition\tRotation");
        }

        // Get references of logged entities
        headset = VRTK_DeviceFinder.HeadsetTransform();
        leftController = VRTK_DeviceFinder.GetControllerLeftHand().transform;
        rightController = VRTK_DeviceFinder.GetControllerRightHand().transform;
        leftControllerEvents = leftController.GetComponent<VRTK_ControllerEvents>();
        rightControllerEvents = rightController.GetComponent<VRTK_ControllerEvents>();

        dashboards = FindObjectsOfType<Dashboard>().ToList();

        textMesh.text = tasks[taskID].taskDescription;

        Debug.Log("Logging started");
    }

    public bool IsLogging()
    {
        return isLogging;
    }

    public void StopLogging()
    {
        if (isMasterLogger)
        {
            photonView.RPC("PhotonStopLogging", PhotonTargets.AllViaServer);
        }
    }

    [PunRPC]
    private void PhotonStopLogging()
    {
        isLogging = false;
        time = 0;

        playerStreamWriter.Close();

        actionsStreamWriter.Close();

        if (isMasterLogger)
            objectStreamWriter.Close();

        textMesh.text = "";

        Debug.Log("Logging stopped");
    }

    private void FixedUpdate()
    {
        if (isMasterLogger && isLogging)
        {
            time += Time.fixedDeltaTime;

            if (timeBetweenLogs <= time)
            {
                time = 0f;

                photonView.RPC("LogPlayerData", PhotonTargets.AllViaServer);
                LogObjectData();
            }
        }   
    }

    [PunRPC]
    private void LogPlayerData(PhotonMessageInfo info)
    {
        if (playerStreamWriter != null)
        {
            playerStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}",
                (info.timestamp - startTime).ToString("F3"),
                headset.position.ToString("F4"),
                headset.rotation.ToString("F4"),
                leftController.position.ToString("F4"),
                leftController.rotation.ToString("F4"),
                leftControllerEvents.triggerClicked.ToString(),
                leftControllerEvents.gripClicked.ToString(),
                leftControllerEvents.touchpadPressed.ToString(),
                leftControllerEvents.GetTouchpadAxisAngle().ToString("F4"),
                rightController.position.ToString("F4"),
                rightController.rotation.ToString("F4"),
                rightControllerEvents.triggerClicked.ToString(),
                rightControllerEvents.gripClicked.ToString(),
                rightControllerEvents.touchpadPressed.ToString(),
                rightControllerEvents.GetTouchpadAxisAngle().ToString("F4")
            );
            }
    }

    public void LogActionData(object obj, PhotonPlayer originalOwner, string description, string name = "")
    {
        if (isLogging && actionsStreamWriter != null)
        {
            actionsStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}",
                (PhotonNetwork.time - startTime).ToString("F3"),
                obj.GetType(),
                originalOwner.ID,
                name,
                description);
        }
    }

    private void LogObjectData()
    {
        if (objectStreamWriter != null)
        {
            foreach (var dashboard in dashboards)
            {
                objectStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}",
                    (PhotonNetwork.time - startTime).ToString("F3"),
                    "Panel",
                    dashboard.photonView.ownerId,
                    dashboard.transform.position.ToString("F4"),
                    dashboard.transform.rotation.ToString("F4")
                );
            }

            foreach (var chart in ChartManager.Instance.Charts)
            {
                objectStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}",
                    (PhotonNetwork.time - startTime).ToString("F3"),
                    "Visualisation",
                    chart.photonView.ownerId,
                    chart.transform.position.ToString("F4"),
                    chart.transform.rotation.ToString("F4")
                );
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (isLogging)
        {
            PhotonStopLogging();
        }
    }
}
