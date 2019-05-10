using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class DataLogger : MonoBehaviourPunCallbacks {

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
    public bool isMasterLogger;
    [SerializeField]
    public List<Task> tasks;

    [HideInInspector]
    public int taskID;

    private StreamWriter playerStreamWriter;
    private StreamWriter actionsStreamWriter;
    private StreamWriter objectStreamWriter;
    private bool isLogging;
    private float time;
    private string format = "F4";

    private double startTime;
    private Transform headset;
    private Transform leftController;
    private Transform rightController;
    private Transform observer;
    private VRTK_ControllerEvents leftControllerEvents;
    private VRTK_ControllerEvents rightControllerEvents;
    private Vector3 headPos;
    private Quaternion headRot;
    private Vector3 leftPos;
    private Quaternion leftRot;
    private Vector3 rightPos;
    private Quaternion rightRot;

    private List<Panel> dashboards;
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
            photonView.RPC("StartLogging", RpcTarget.AllViaServer, groupID, taskID, tasks[taskID].taskName, tasks[taskID].taskDescription);
        }
    }

    [PunRPC]
    private void StartLogging(int groupID, int taskID, string taskName, string taskDescription, PhotonMessageInfo info)
    {
        isLogging = true;

        this.groupID = groupID;
        this.taskID = taskID;

        if (!filePath.EndsWith("\\"))
            filePath += "\\";

        if (isMasterLogger)
        {
            string path = string.Format("{0}Group{1}_Task{2}_Participant{3}_Photon.Realtime.Player{4}_ObjectData.txt", filePath, groupID, tasks[taskID].taskName, participantID, PhotonNetwork.LocalPlayer.ActorNumber);
            objectStreamWriter = new StreamWriter(path, true);

            // Write header for object data
            objectStreamWriter.WriteLine("Timestamp\tObjectType\tOwner\tPosition.x\tPosition.y\tPosition.z\tRotation.x\tRotation.y\tRotation.z\tRotation.w\tWidth\tHeight\tDepth");

            // Save references of logged entities
            dashboards = FindObjectsOfType<Panel>().ToList();
            observer = FindObjectOfType<KeyboardAndMouseAvatar>().transform;
        }
        else
        {
            string path = string.Format("{0}Group{1}_Task{2}_Participant{3}_Photon.Realtime.Player{4}_PlayerData.txt", filePath, groupID, tasks[taskID].taskName, participantID, PhotonNetwork.LocalPlayer.ActorNumber);
            playerStreamWriter = new StreamWriter(path, true);

            // Write header for player data
            playerStreamWriter.WriteLine("Timestamp\t" +
                                         "HeadPosition.x\tHeadPosition.y\tHeadPosition.z\t" +
                                         "HeadRotation.x\tHeadRotation.y\tHeadRotation.z\tHeadRotation.w\t" +
                                         "LeftPosition.x\tLeftPosition.y\tLeftPosition.z\t" +
                                         "LeftRotation.x\tLeftRotation.y\tLeftRotation.z\tLeftRotation.w\t" +
                                         "LeftTrigger\tLeftGrip\tLeftTouchpad\tLeftTouchpadAngle\t" +
                                         "RightPosition.x\tRightPosition.y\tRightPosition.z\t" +
                                         "RightRotation.x\tRightRotation.y\tRightRotation.z\tRightRotation.w\t" +
                                         "RightTrigger\tRightGrip\tRightTouchpad\tRightTouchpadAngle");

            path = string.Format("{0}Group{1}_Task{2}_Participant{3}_Photon.Realtime.Player{4}_ActionData.txt", filePath, groupID, tasks[taskID].taskName, participantID, PhotonNetwork.LocalPlayer.ActorNumber);
            actionsStreamWriter = new StreamWriter(path, true);

            // Write header for action data
            actionsStreamWriter.WriteLine("Timestamp\tObjectType\tOriginalOwner\tName\tDescription");

            // Save references of logged entities
            headset = VRTK_DeviceFinder.HeadsetTransform();
            leftController = VRTK_DeviceFinder.GetControllerLeftHand().transform;
            rightController = VRTK_DeviceFinder.GetControllerRightHand().transform;
            leftControllerEvents = leftController.GetComponent<VRTK_ControllerEvents>();
            rightControllerEvents = rightController.GetComponent<VRTK_ControllerEvents>();
        }

        startTime = info.timestamp;
        textMesh.text = string.Format("<b>{0}</b>\n{1}", taskName, taskDescription.Replace("\\n", "\n"));

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
            photonView.RPC("PhotonStopLogging", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    private void PhotonStopLogging()
    {
        isLogging = false;
        time = 0;

        if (isMasterLogger)
        {
            objectStreamWriter.Close();
        }
        else
        {
            playerStreamWriter.Close();
            actionsStreamWriter.Close();
        }

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

                photonView.RPC("LogPlayerData", RpcTarget.AllViaServer);

                LogObjectData();
            }
        }   
    }

    [PunRPC]
    private void LogPlayerData(PhotonMessageInfo info)
    {
        if (isLogging && playerStreamWriter != null)
        {
            headPos = headset.position;
            headRot = headset.rotation;
            leftPos = leftController.position;
            leftRot = leftController.rotation;
            rightPos = rightController.position;
            rightRot = rightController.rotation;

            playerStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}\t{27}\t{28}\t{29}",
                (info.timestamp - startTime).ToString("F3"),
                // Head position
                headPos.x.ToString(format),
                headPos.y.ToString(format),
                headPos.z.ToString(format),
                // Head rotation
                headRot.x.ToString(format),
                headRot.y.ToString(format),
                headRot.z.ToString(format),
                headRot.w.ToString(format),
                // Left controller position
                leftPos.x.ToString(format),
                leftPos.y.ToString(format),
                leftPos.z.ToString(format),
                // Left controller rotation
                leftRot.x.ToString(format),
                leftRot.y.ToString(format),
                leftRot.z.ToString(format),
                leftRot.w.ToString(format),
                // Left controller events
                leftControllerEvents.triggerClicked.ToString(),
                leftControllerEvents.gripClicked.ToString(),
                leftControllerEvents.touchpadPressed.ToString(),
                leftControllerEvents.touchpadPressed ? leftControllerEvents.GetTouchpadAxisAngle().ToString(format) : "-1",
                // Right controller position
                rightPos.x.ToString(format),
                rightPos.y.ToString(format),
                rightPos.z.ToString(format),
                // Right controller rotation
                rightRot.x.ToString(format),
                rightRot.y.ToString(format),
                rightRot.z.ToString(format),
                rightRot.w.ToString(format),
                // Right controller events
                rightControllerEvents.triggerClicked.ToString(),
                rightControllerEvents.gripClicked.ToString(),
                rightControllerEvents.touchpadPressed.ToString(),
                rightControllerEvents.touchpadPressed ? rightControllerEvents.GetTouchpadAxisAngle().ToString(format) : "-1"
            );

            playerStreamWriter.Flush();
        }
    }

    public void LogActionData(object obj, Photon.Realtime.Player originalOwner, string description, string name = "")
    {
        if (isLogging && actionsStreamWriter != null)
        {
            actionsStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}",
                (PhotonNetwork.Time - startTime).ToString("F3"),
                obj.GetType(),
                originalOwner.ActorNumber,
                name,
                description);

            actionsStreamWriter.Flush();
        }
    }

    private void LogObjectData()
    {
        if (isLogging && objectStreamWriter != null)
        {
            string t = (PhotonNetwork.Time - startTime).ToString("F3");

            foreach (var dashboard in dashboards)
            {
                objectStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}",
                    t,
                    "Panel",
                    dashboard.photonView.OwnerActorNr,
                    dashboard.transform.position.x.ToString(format),
                    dashboard.transform.position.y.ToString(format),
                    dashboard.transform.position.z.ToString(format),
                    dashboard.transform.rotation.x.ToString(format),
                    dashboard.transform.rotation.y.ToString(format),
                    dashboard.transform.rotation.z.ToString(format),
                    dashboard.transform.rotation.w.ToString(format),
                    "",
                    "",
                    ""
                );
            }

            foreach (var chart in ChartManager.Instance.Charts)
            {
                objectStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}",
                    t,
                    "Visualisation",
                    chart.photonView.OwnerActorNr,
                    chart.transform.position.x.ToString(format),
                    chart.transform.position.y.ToString(format),
                    chart.transform.position.z.ToString(format),
                    chart.transform.rotation.x.ToString(format),
                    chart.transform.rotation.y.ToString(format),
                    chart.transform.rotation.z.ToString(format),
                    chart.transform.rotation.w.ToString(format),
                    chart.Width.ToString(format),
                    chart.Height.ToString(format),
                    chart.Depth.ToString(format)
                );
            }

            objectStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}",
                t,
                "Observer",
                0,
                observer.position.x.ToString(format),
                observer.position.y.ToString(format),
                observer.position.z.ToString(format),
                observer.rotation.x.ToString(format),
                observer.rotation.y.ToString(format),
                observer.rotation.z.ToString(format),
                observer.rotation.w.ToString(format),
                "",
                "",
                ""
                );

            objectStreamWriter.Flush();
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
