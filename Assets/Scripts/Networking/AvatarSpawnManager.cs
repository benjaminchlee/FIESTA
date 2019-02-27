namespace PlayoVR {
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using VRTK;
    using Hashtable = ExitGames.Client.Photon.Hashtable;

    public class AvatarSpawnManager : Photon.PunBehaviour {
        [Tooltip("Reference to the local avatar prefab")]
        public GameObject localAvatar;
        [Tooltip("Reference to the remote avatar prefab")]
        public GameObject remoteAvatar;

        private GameObject[] spawnPoints;
        private bool sceneLoaded = false;
        private bool connected = false;

        void Awake() {
            if (localAvatar == null) {
                Debug.LogError("AvatarSpawnManager is missing a reference to the local avatar prefab!");
            }
            if (remoteAvatar == null) {
                Debug.LogError("AvatarSpawnManager is missing a reference to the remote avatar prefab!");
            }
        }

        void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Debug.Log("Scene loaded");
            sceneLoaded = true;
        }

        public override void OnJoinedRoom() {
            connected = true;
            // Player sets its own name when joining
            PhotonNetwork.playerName = playerName(PhotonNetwork.player);
            // Initialize the master client
            InitPlayer(PhotonNetwork.player);
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
            InitPlayer(newPlayer);
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        }

        void InitPlayer(PhotonPlayer newPlayer) {
            if (PhotonNetwork.isMasterClient && connected && sceneLoaded) {
                // The master client tells everyone about the new player
                //Hashtable props = new Hashtable();
                //props[PlayerPropNames.PLAYER_NR] = newPlayer.ID;
                //newPlayer.SetCustomProperties(props);
                photonView.RPC("SpawnAvatar", PhotonTargets.AllBuffered, newPlayer.ID, PhotonNetwork.AllocateViewID());
            }
        }

        [PunRPC]
        void SpawnAvatar(int senderId, int newViewId) {
            //if (!PhotonNetwork.player.CustomProperties.ContainsKey(PlayerPropNames.PLAYER_NR)) {
            //    Debug.LogError("Player does not have a PLAYER_NR property!");
            //    return;
            //}
            if (isPlayerConnected(senderId))
            {
                GameObject player;

                if (senderId == PhotonNetwork.player.ID)
                {
                    // Create a new player at the appropriate spawn spot
                    var name = PhotonNetwork.playerName;

                    player = Instantiate(localAvatar) as GameObject;

                    // Set the new colliders
                    // TODO: Improve
                    GameObject leftHand = player.transform.Find("hand_left").gameObject;
                    leftHand.transform.SetParent(VRTK_DeviceFinder.GetControllerLeftHand().transform);
                    VRTK_InteractTouch leftTouch = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_InteractTouch>();
                    leftTouch.customColliderContainer = leftHand;
                    LockPosition leftLock = leftHand.AddComponent<LockPosition>();
                    leftLock.isWorldSpace = false;
                    leftLock.SetPosition(new Vector3(0.025f, 0, -0.04f), Quaternion.identity);

                    GameObject rightHand = player.transform.Find("hand_right").gameObject;
                    rightHand.transform.SetParent(VRTK_DeviceFinder.GetControllerRightHand().transform);
                    VRTK_InteractTouch rightTouch = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_InteractTouch>();
                    rightTouch.customColliderContainer = rightHand;
                    LockPosition rightLock = rightHand.AddComponent<LockPosition>();
                    rightLock.isWorldSpace = false;
                    rightLock.SetPosition(new Vector3(-0.025f, 0, -0.04f), Quaternion.identity);
                }
                else
                {
                    player = Instantiate(remoteAvatar) as GameObject;
                }

                DontDestroyOnLoad(player);

                if (player != null)
                {
                    PhotonView photonView = player.GetComponent<PhotonView>();

                    if (photonView != null)
                    {
                        photonView.viewID = newViewId;
                        photonView.TransferOwnership(senderId);
                    }

                    if (photonView.isMine)
                    {
                        AvatarCustomiser avatarCustomiser = player.GetComponent<AvatarCustomiser>();
                        avatarCustomiser.photonView.RPC("SetColor", PhotonTargets.AllBuffered, PlayerPreferencesManager.Instance.AvatarColor);
                        avatarCustomiser.photonView.RPC("SetName", PhotonTargets.AllBuffered, PlayerPreferencesManager.Instance.AvatarName);
                    }
                }
            }
        }

        private string playerName(PhotonPlayer ply) {
            return "Player " + ply.ID;
        }

        private bool isPlayerConnected(int id)
        {
            foreach (PhotonPlayer ply in PhotonNetwork.playerList)
            {
                if (ply.ID == id)
                    return true;
            }

            return false;
        }
    }
}

