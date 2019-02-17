using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AvatarCustomiser : Photon.PunBehaviour {

	public OvrAvatar ovrAvatar;
    public TextMeshPro nameplate;

    public string Name { get; private set; }
    public Color Color { get; private set; }

    private bool isDoneLoading = false;

    private List<GameObject> childGameObjects;

    private void Awake()
    {
        ovrAvatar.AssetsDoneLoading.AddListener(OnAssetsDoneLoading);

        childGameObjects = new List<GameObject>();
        foreach (Transform child in transform)
        {
            childGameObjects.Add(child.gameObject);
        }
    }

    [PunRPC]
    public void SetName(string name)
    {
        this.Name = name;

        if (isDoneLoading)
        {
            // Set the text so that it is the child of the moveable body
            Transform body = transform.Find("body/body_renderPart_0/root_JNT/body_JNT/chest_JNT");

            nameplate.transform.SetParent(body);
            nameplate.transform.localPosition = Vector3.up * 0.5f;

            nameplate.text = this.Name;

            if (photonView.isMine)
                nameplate.text = "";
        }
    }

    [PunRPC]
    public void SetColor(float r, float g, float b)
    {
        Color = new Color(r, g, b);

        if (isDoneLoading)
        {
            foreach (GameObject child in childGameObjects)
            {
                SkinnedMeshRenderer[] renderers = child.GetComponentsInChildren<SkinnedMeshRenderer>();
                
                foreach (SkinnedMeshRenderer renderer in renderers)
                {
                    renderer.material.SetColor("_BaseColor", Color);
                }
            }
        }
    }

    private void OnAssetsDoneLoading()
    {
        isDoneLoading = true;

        SetColor(Color.r, Color.g, Color.b);
        SetName(Name);
    }
}
