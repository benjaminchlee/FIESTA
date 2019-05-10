using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupConnectionManager : MonoBehaviourPunCallbacks
{
    private string ipAddress = "";
    private bool isSpectator = false;

    public void SetIPAddress(string value)
    {
        ipAddress = value;
    }

    public void ToggleSpectator(bool value)
    {
        isSpectator = value;
    }

    public void Connect()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;

        if (ipAddress == "")
            ipAddress = "127.0.0.1";

        PhotonNetwork.PhotonServerSettings.AppSettings.Server = ipAddress;

        if (isSpectator)
            SceneManager.LoadScene("NonVR_ConnectingScene");
        else
            SceneManager.LoadScene("ConnectingScene");
    }

    public void ConnectViaCloud()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = "";

        if (isSpectator)
            SceneManager.LoadScene("NonVR_ConnectingScene");
        else
            SceneManager.LoadScene("ConnectingScene");
    }
}
