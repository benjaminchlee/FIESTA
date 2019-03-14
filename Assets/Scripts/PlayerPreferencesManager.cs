using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreferencesManager : MonoBehaviour {

    public static PlayerPreferencesManager Instance { get; private set; }

    [SerializeField]
    private string avatarName;
    public string AvatarName
    {
        get { return avatarName; }
    }

    [SerializeField]
    private Color avatarSkinColor;
    public Color AvatarSkinColor
    {
        get { return avatarSkinColor; }
    }

    [SerializeField]
    private Color avatarHeadsetColor;
    public Color AvatarHeadsetColor
    {
        get { return avatarHeadsetColor; }
    }

    [SerializeField]
    private Color avatarShirtColor;
    public Color AvatarShirtColor
    {
        get { return avatarShirtColor; }
    }

    [SerializeField]
    private Color sharedBrushColor;
    public Color SharedBrushColor
    {
        get { return sharedBrushColor; }
    }
    
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
    }
}
