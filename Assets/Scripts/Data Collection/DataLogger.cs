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

    private struct RaycastObjectInfo
    {
        public string ObjectName;
        public string OriginalObjectOwner;
        public string ObjectOwner;
        public string ObjectID;
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
    public List<Task> tasks;

    [SerializeField]
    public bool isMasterLogger;
    [HideInInspector]
    public bool isLoggingPlayerData;

    [HideInInspector]
    public int taskID;

    private Dictionary<int, int> photonToParticipantDictionary = new Dictionary<int, int>();

    private StreamWriter playerStreamWriter;
    private StreamWriter actionsStreamWriter;
    private StreamWriter objectStreamWriter;
    private StreamWriter annotationsStreamWriter;
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
    private RaycastHit gazeRaycast;
    private RaycastHit leftRaycast;
    private RaycastHit rightRaycast;
    private bool gazeHit;
    private bool leftHit;
    private bool rightHit;
    private RaycastObjectInfo gazeInfo;
    private RaycastObjectInfo leftInfo;
    private RaycastObjectInfo rightInfo;

    private List<Panel> dashboards;
    private TextMeshPro textMesh;
    private List<MarkerScript> markers;
    private List<EraserScript> erasers;
    
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

    private void Start()
    {
        // Let everyone know the participant IDs and actor numbers of each other
        photonView.RPC("SendParticipantNumberRPC", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, participantID);
    }

    [PunRPC]
    private void SendParticipantNumberRPC(int actNo, int parNo)
    {
        photonToParticipantDictionary.Add(actNo, parNo);
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
            // Objects
            string path = string.Format("{0}Group{1}_Task{2}_ObjectData.txt", filePath, groupID, tasks[taskID].taskName);
            objectStreamWriter = new StreamWriter(path, true);

            // Write header for object data
            objectStreamWriter.WriteLine("Timestamp\tObjectType\tOriginalOwner\tOwner\tPosition.x\tPosition.y\tPosition.z\tRotation.x\tRotation.y\tRotation.z\tRotation.w\tWidth\tHeight\tDepth" +
                "\tID\txDimension\tyDimension\tzDimension\tSize\tSizeDimension\tColor\tColorDimension\tFacetDimension\tFacetSize\txNormaliser\tyNormaliser\tzNormaliser");

            // Save references of logged entities
            dashboards = FindObjectsOfType<Panel>().ToList();
            if (FindObjectOfType<KeyboardAndMouseAvatar>() != null)
                observer = FindObjectOfType<KeyboardAndMouseAvatar>().transform;
            markers = FindObjectsOfType<MarkerScript>().ToList();
            erasers = FindObjectsOfType<EraserScript>().ToList();

            // Annotations
            path = string.Format("{0}Group{1}_Task{2}_AnnotationData.txt", filePath, groupID, tasks[taskID].taskName);
            annotationsStreamWriter = new StreamWriter(path, true);

            // Write header for annotation data
            annotationsStreamWriter.WriteLine("Timestamp\tOriginalOwner\tOwner\tPositions\tParentChart");

        }
        if (!isMasterLogger || (isMasterLogger && isLoggingPlayerData))
        {
            string path = string.Format("{0}Group{1}_Task{2}_Participant{3}_PlayerData.txt", filePath, groupID, tasks[taskID].taskName, participantID);
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
                                         "RightTrigger\tRightGrip\tRightTouchpad\tRightTouchpadAngle\t" +
                                         "GazeObject\tGazeObjectOriginalOwner\tGazeObjectOwner\tGazeObjectID\t" +
                                         "LeftPointObject\tLeftPointObjectOriginalOwner\tLeftPointObjectOwner\tLeftPointObjectID\t" +
                                         "RightPointObject\tRightPointObjectOriginalOwner\tRightPointObjectOwner\tRightPointObjectID");

            path = string.Format("{0}Group{1}_Task{2}_Participant{3}_ActionData.txt", filePath, groupID, tasks[taskID].taskName, participantID);
            actionsStreamWriter = new StreamWriter(path, true);

            // Write header for action data
            actionsStreamWriter.WriteLine("Timestamp\tObjectType\tOriginalOwner\tOwner\tName\tTargetID\tDescription");

            // Save references of logged entities
            headset = VRTK_DeviceFinder.HeadsetTransform();
            leftController = VRTK_DeviceFinder.GetControllerLeftHand().transform;
            rightController = VRTK_DeviceFinder.GetControllerRightHand().transform;
            leftControllerEvents = leftController.GetComponent<VRTK_ControllerEvents>();
            rightControllerEvents = rightController.GetComponent<VRTK_ControllerEvents>();
        }

        startTime = info.SentServerTime;
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
            annotationsStreamWriter.Close();
        }
        if (!isMasterLogger || (isMasterLogger && isLoggingPlayerData))
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

                string t = (PhotonNetwork.Time - startTime).ToString("F3");

                LogObjectData(t);
                LogAnnotationData(t);
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

            gazeHit = Physics.Raycast(headset.position, headset.forward, out gazeRaycast);
            leftHit = Physics.Raycast(leftController.position, leftController.forward, out leftRaycast);
            rightHit = Physics.Raycast(rightController.position, rightController.forward, out rightRaycast);

            gazeInfo = GetRaycastInfo(gazeHit, gazeRaycast, gazeInfo);
            leftInfo = GetRaycastInfo(leftHit, leftRaycast, leftInfo);
            rightInfo = GetRaycastInfo(rightHit, rightRaycast, rightInfo);

            playerStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}\t{27}\t{28}\t{29}" +
                "\t{30}\t{31}\t{32}\t{33}\t{34}\t{35}\t{36}\t{37}\t{38}\t{39}\t{40}\t{41}",
                (info.SentServerTime - startTime).ToString("F3"),
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
                rightControllerEvents.touchpadPressed ? rightControllerEvents.GetTouchpadAxisAngle().ToString(format) : "-1",
                // Raycasting
                gazeInfo.ObjectName,
                gazeInfo.OriginalObjectOwner,
                gazeInfo.ObjectOwner,
                gazeInfo.ObjectID,
                leftInfo.ObjectName,
                leftInfo.OriginalObjectOwner,
                leftInfo.ObjectOwner,
                leftInfo.ObjectID,
                rightInfo.ObjectName,
                rightInfo.OriginalObjectOwner,
                rightInfo.ObjectOwner,
                rightInfo.ObjectID
            );

            playerStreamWriter.Flush();
        }
    }

    private RaycastObjectInfo GetRaycastInfo(bool isHit, RaycastHit hit, RaycastObjectInfo info)
    {
        if (!isHit)
            return new RaycastObjectInfo();

        GameObject hitObj = hit.collider.gameObject;

        OvrAvatar ovrAvatar = hitObj.GetComponent<OvrAvatar>();
        if (ovrAvatar == null) ovrAvatar = hitObj.GetComponentInParent<OvrAvatar>();
        if (ovrAvatar != null)
        {
            info.ObjectName = "Player Avatar";
            info.OriginalObjectOwner = photonToParticipantDictionary[ovrAvatar.GetComponent<PhotonView>().Owner.ActorNumber].ToString();
            info.ObjectOwner = info.OriginalObjectOwner;
            info.ObjectID = "";

            return info;
        }

        Chart chart = hitObj.GetComponent<Chart>();
        if (chart == null) chart = hitObj.GetComponentInParent<Chart>();
        if (chart != null)
        {
            info.ObjectName = "Chart";
            info.OriginalObjectOwner = photonToParticipantDictionary[chart.OriginalOwner.ActorNumber].ToString();
            info.ObjectOwner = photonToParticipantDictionary[chart.photonView.OwnerActorNr].ToString();
            info.ObjectID = chart.ID;

            return info;
        }

        Panel panel = hitObj.GetComponent<Panel>();
        if (panel == null) panel = hitObj.GetComponentInParent<Panel>();
        if (panel != null)
        {
            info.ObjectName = "Panel";
            info.OriginalObjectOwner = photonToParticipantDictionary[panel.OriginalOwner.ActorNumber].ToString();
            info.ObjectOwner = info.OriginalObjectOwner;
            info.ObjectID = "";

            return info;
        }

        return new RaycastObjectInfo();
    }

    public void LogActionData(object obj, Photon.Realtime.Player originalOwner, Photon.Realtime.Player currentOwner, string description, string targetID = "", string name = "")
    {
        if (isLogging && actionsStreamWriter != null)
        {
            actionsStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                (PhotonNetwork.Time - startTime).ToString("F3"),
                obj.GetType(),
                photonToParticipantDictionary[originalOwner.ActorNumber],
                photonToParticipantDictionary[currentOwner.ActorNumber],
                name,
                targetID,
                description);

            actionsStreamWriter.Flush();
        }
    }

    private void LogObjectData(string timestamp)
    {
        if (isLogging && objectStreamWriter != null)
        {
            foreach (var dashboard in dashboards)
            {
                objectStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}",
                    timestamp,
                    "Panel",
                    photonToParticipantDictionary[dashboard.OriginalOwner.ActorNumber],
                    photonToParticipantDictionary[dashboard.photonView.OwnerActorNr],
                    dashboard.transform.position.x.ToString(format),
                    dashboard.transform.position.y.ToString(format),
                    dashboard.transform.position.z.ToString(format),
                    dashboard.transform.rotation.x.ToString(format),
                    dashboard.transform.rotation.y.ToString(format),
                    dashboard.transform.rotation.z.ToString(format),
                    dashboard.transform.rotation.w.ToString(format),
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    ""
                );
            }
            
            foreach (var chart in ChartManager.Instance.Charts)
            {
                objectStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}",
                    timestamp,
                    "Visualisation",
                    photonToParticipantDictionary[chart.OriginalOwner.ActorNumber],
                    photonToParticipantDictionary[chart.photonView.OwnerActorNr],
                    chart.transform.position.x.ToString(format),
                    chart.transform.position.y.ToString(format),
                    chart.transform.position.z.ToString(format),
                    chart.transform.rotation.x.ToString(format),
                    chart.transform.rotation.y.ToString(format),
                    chart.transform.rotation.z.ToString(format),
                    chart.transform.rotation.w.ToString(format),
                    chart.Width.ToString(format),
                    chart.Height.ToString(format),
                    chart.Depth.ToString(format),
                    chart.ID,
                    chart.XDimension,
                    chart.YDimension,
                    chart.ZDimension,
                    chart.Size,
                    chart.SizeDimension,
                    chart.Color,
                    chart.ColorDimension,
                    chart.FacetDimension,
                    chart.FacetSize,
                    chart.XNormaliser.ToString(format),
                    chart.YNormaliser.ToString(format),
                    chart.ZNormaliser.ToString(format)
                );
            }

            if (observer != null)
            {
                objectStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}",
                    timestamp,
                    "Observer",
                    -1,
                    -1,
                    observer.position.x.ToString(format),
                    observer.position.y.ToString(format),
                    observer.position.z.ToString(format),
                    observer.rotation.x.ToString(format),
                    observer.rotation.y.ToString(format),
                    observer.rotation.z.ToString(format),
                    observer.rotation.w.ToString(format),
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    ""
                    );
            }

            foreach (var marker in markers)
            {
                objectStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}",
                    timestamp,
                    "Marker",
                    photonToParticipantDictionary[marker.OriginalOwner.ActorNumber],
                    photonToParticipantDictionary[marker.photonView.OwnerActorNr],
                    marker.transform.position.x.ToString(format),
                    marker.transform.position.y.ToString(format),
                    marker.transform.position.z.ToString(format),
                    marker.transform.rotation.x.ToString(format),
                    marker.transform.rotation.y.ToString(format),
                    marker.transform.rotation.z.ToString(format),
                    marker.transform.rotation.w.ToString(format),
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    ""
                );
            }

            foreach (var eraser in erasers)
            {
                objectStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}",
                    timestamp,
                    "Eraser",
                    photonToParticipantDictionary[eraser.OriginalOwner.ActorNumber],
                    photonToParticipantDictionary[eraser.photonView.OwnerActorNr],
                    eraser.transform.position.x.ToString(format),
                    eraser.transform.position.y.ToString(format),
                    eraser.transform.position.z.ToString(format),
                    eraser.transform.rotation.x.ToString(format),
                    eraser.transform.rotation.y.ToString(format),
                    eraser.transform.rotation.z.ToString(format),
                    eraser.transform.rotation.w.ToString(format),
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    ""
                );
            }

            objectStreamWriter.Flush();
        }
    }

    private void LogAnnotationData(string timestamp)
    {
        if (isLogging && annotationsStreamWriter != null)
        {
            foreach (GameObject line in GameObject.FindGameObjectsWithTag("Line"))
            {
                LineScript lineScript = line.GetComponent<LineScript>();
                Chart parent = line.GetComponentInParent<Chart>();

                annotationsStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}",
                    timestamp,
                    photonToParticipantDictionary[lineScript.OriginalOwner.ActorNumber],
                    photonToParticipantDictionary[lineScript.photonView.OwnerActorNr],
                    lineScript.GetLinePositions(),
                    (parent != null) ? parent.ID : ""
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
