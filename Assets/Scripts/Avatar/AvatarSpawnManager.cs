using Oculus.Platform.Samples.VrHoops;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class AvatarSpawnManager : MonoBehaviourPunCallbacks {

    [SerializeField] [Tooltip("Reference to the local avatar prefab")]
    private GameObject localAvatar;
    [SerializeField] [Tooltip("Reference to the remote avatar prefab")]
    private GameObject remoteAvatar;
    private void Awake()
    {
        if (localAvatar == null) {
            Debug.LogError("AvatarSpawnManager is missing a reference to the local avatar prefab!");
        }
        if (remoteAvatar == null) {
            Debug.LogError("AvatarSpawnManager is missing a reference to the remote avatar prefab!");
        }
    }

    public override void OnJoinedRoom()
    {
        int senderNo = PhotonNetwork.LocalPlayer.ActorNumber;
        bool vrAvatar = PlayerPreferencesManager.Instance.SpawnVRAvatar;
        bool kbmAvatar = PlayerPreferencesManager.Instance.SpawnKBMAvatar;

        photonView.RPC("PlayerJoinedRoom", RpcTarget.MasterClient, senderNo, vrAvatar, kbmAvatar);
    }

    [PunRPC]
    private void PlayerJoinedRoom(int senderNo, bool vrAvatar, bool kbmAvatar)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (vrAvatar)
                SpawnVRAvatar(senderNo);

            if (kbmAvatar)
                SpawnKBMAvatar(senderNo);
        }
    }

    /// <summary>
    /// This method instantiates the VR avatar gameobjects for all players.
    ///
    /// The method distinguishes between the master client and other clients. The master client should be the first one to run this method, which assigns a unique
    /// view ID to the avatar. The master client then runs this method on all other clients, using the same view ID on their avatar gameobjects.
    /// </summary>
    /// <param name="senderNo"></param>
    /// <param name="viewID"></param>
    [PunRPC]
    private void SpawnVRAvatar(int senderNo, int viewID = -1)
    {
        GameObject player;

        // If the call to spawn the avatar was made by this player, instantiate local avatar
        if (senderNo == PhotonNetwork.LocalPlayer.ActorNumber)
            player = Instantiate(localAvatar) as GameObject;
        // Otherwise it was made by another player, instantiate remote avatar instead
        else
            player = Instantiate(remoteAvatar) as GameObject;
        
        // Assign view ID of player PhotonView
        PhotonView playerPhotonView = player.GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            // Get view ID of master client's version of new player gameobject and instantiate on other clients
            if (PhotonNetwork.AllocateViewID(playerPhotonView))
            {
                photonView.RPC("SpawnVRAvatar", RpcTarget.OthersBuffered, senderNo, playerPhotonView.ViewID);
            }
            else
            {
                Debug.LogError("Couldn't assign PhotonView ID to new player model.");
            }
        }
        else
        {
            // Assign view ID from master
            playerPhotonView.ViewID = viewID;
        }

        playerPhotonView.TransferOwnership(senderNo);
        DontDestroyOnLoad(player);

        if (playerPhotonView.IsMine)
        {
            // Customise avatar based off defaults
            AvatarCustomiser avatarCustomiser = player.GetComponent<AvatarCustomiser>();
            avatarCustomiser.SetColor(PlayerPreferencesManager.Instance.AvatarSkinColor, PlayerPreferencesManager.Instance.AvatarHeadsetColor, PlayerPreferencesManager.Instance.AvatarShirtColor);
            avatarCustomiser.SetName(PlayerPreferencesManager.Instance.AvatarName);

            // Add event listeners to change avatar customisation options
            GameObject.Find("PlayerNameInputField").GetComponent<InputField>().onValueChanged.AddListener(avatarCustomiser.SetName);
            GameObject.Find("PlayerColorPicker").GetComponent<ColorPicker>().onValueChanged.AddListener(avatarCustomiser.SetShirtColor);
        }
    }

    /// <summary>
    /// This method spawns the keyboard and mouse avatar for the specified player.
    ///
    /// As there is only one type of KBM avatar for both the owner and non-owners, Photon's instantiate method is used to allocate the view ID for us.
    /// </summary>
    /// <param name="senderNo"></param>
    [PunRPC]
    private void SpawnKBMAvatar(int senderNo)
    {
        if (senderNo == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            PhotonNetwork.Instantiate("KBMAvatar", Vector3.zero, Quaternion.identity, 0);
        }
        else
        {
            photonView.RPC("SpawnKBMAvatar", PhotonNetwork.LocalPlayer.Get(senderNo), senderNo);
        }
    }
}

