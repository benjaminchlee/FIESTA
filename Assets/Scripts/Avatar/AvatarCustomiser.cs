using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AvatarCustomiser : MonoBehaviourPunCallbacks {

	public OvrAvatar ovrAvatar;
    public TextMeshPro nameplate;

    public string Name { get; private set; }
    public Color SkinColor { get; private set; }
    public Color HeadsetColor { get; private set; }
    public Color ShirtColor { get; private set; }

    private bool isDoneLoading = false;
    private bool isMainSceneLoaded = false;
    
    private void Awake()
    {
        ovrAvatar.AssetsDoneLoading.AddListener(OnAssetsDoneLoading);
        
        SceneManager.sceneLoaded += OnSceneDoneLoading;
    }

    public void SetName(string name)
    {
        photonView.RPC("PropagateSetName", RpcTarget.AllBuffered, name);
    }

    [PunRPC]
    private void PropagateSetName(string name)
    {
        Name = name;

        SetName();
    }

    private void SetName()
    {
        if (isDoneLoading)
        {
            // Set the text so that it is the child of the moveable body
            Transform body = transform.Find("body/body_renderPart_0/root_JNT/body_JNT/chest_JNT");

            nameplate.transform.SetParent(body);
            nameplate.transform.localPosition = Vector3.up * 0.5f;

            nameplate.text = this.Name;

            if (photonView.IsMine)
                nameplate.text = "";
        }
    }

    public void SetColor(Color skin, Color headset, Color shirt)
    {
        photonView.RPC("PropagateSetColor", RpcTarget.AllBuffered, skin, headset, shirt);
    }

    [PunRPC]
    private void PropagateSetColor(Color skin, Color headset, Color shirt)
    {
        SkinColor = skin;
        HeadsetColor = headset;
        ShirtColor = shirt;

        SetColor();
    }

    private void SetColor()
    {
        if (isDoneLoading)
        {
            SkinnedMeshRenderer[] renderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                if (renderer.gameObject.name == "body_renderPart_0")
                    renderer.material.SetColor("_BaseColor", SkinColor);
                else if (renderer.gameObject.name == "body_renderPart_1")
                    renderer.material.SetColor("_BaseColor", ShirtColor);
                else if (renderer.gameObject.name == "body_renderPart_2")
                    renderer.material.SetColor("_BaseColor", HeadsetColor);
                else
                    renderer.material.SetColor("_BaseColor", ShirtColor);
            }
        }
    }

    public void SetShirtColor(Color shirt)
    {
        photonView.RPC("PropagateSetColor", RpcTarget.All, SkinColor, HeadsetColor, shirt);
    }

    private void OnAssetsDoneLoading()
    {
        isDoneLoading = true;

        if (photonView.IsMine)
        {
            SetColor(SkinColor, HeadsetColor, ShirtColor);
            SetName(Name);
        }
    }

    private void Update()
    {
        if (!isMainSceneLoaded)
        {
            SetColor();
            SetName();
        }
    }

    private void OnSceneDoneLoading(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "MainScene")
        {
            SetColor(SkinColor, HeadsetColor, ShirtColor);
            PhotonNetwork.RemoveRPCs(photonView);
            isMainSceneLoaded = true;
        }
    }
}
