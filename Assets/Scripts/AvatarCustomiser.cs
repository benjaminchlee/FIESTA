using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AvatarCustomiser : Photon.PunBehaviour {

	public OvrAvatar ovrAvatar;
    public TextMeshPro nameplate;

    private string nameToSet;
    private Color colorToSet;

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
        nameToSet = name;

        if (isDoneLoading)
        {
            // Set the text so that it is the child of the moveable body
            Transform body = transform.Find("body/body_renderPart_0/root_JNT/body_JNT/chest_JNT");

            nameplate.transform.SetParent(body);
            nameplate.transform.localPosition = Vector3.up * 0.5f;

            nameplate.text = nameToSet;

            if (photonView.isMine)
                nameplate.text = "";
        }
    }

    [PunRPC]
    public void SetColor(float r, float g, float b)
    {
        colorToSet = new Color(r, g, b);

        if (isDoneLoading)
        {
            foreach (GameObject child in childGameObjects)
            {
                SkinnedMeshRenderer[] renderers = child.GetComponentsInChildren<SkinnedMeshRenderer>();
                
                foreach (SkinnedMeshRenderer renderer in renderers)
                {
                    renderer.material.SetColor("_BaseColor", colorToSet);
                }
            }
        }
    }

    private void OnAssetsDoneLoading()
    {
        isDoneLoading = true;

        SetColor(colorToSet.r, colorToSet.g, colorToSet.b);
        SetName(nameToSet);
    }
}
