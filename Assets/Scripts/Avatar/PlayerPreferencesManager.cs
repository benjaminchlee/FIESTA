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
        private set { avatarSkinColor = value; }
    }

    [SerializeField]
    private Color avatarHeadsetColor;
    public Color AvatarHeadsetColor
    {
        get { return avatarHeadsetColor; }
        private set { avatarHeadsetColor = value; }
    }

    [SerializeField]
    private Color avatarShirtColor;
    public Color AvatarShirtColor
    {
        get { return avatarShirtColor; }
        private set { avatarShirtColor = value; }
    }

    [SerializeField]
    private Color privateBrushColor;
    public Color PrivateBrushColor
    {
        get { return privateBrushColor; }
        private set { privateBrushColor = value; }
    }

    [SerializeField]
    private Color sharedBrushColor;
    public Color SharedBrushColor
    {
        get { return sharedBrushColor; }
        private set { sharedBrushColor = value; }
    }
    
    [SerializeField]
    private bool spawnVRAvatar;
    public bool SpawnVRAvatar
    {
        get { return spawnVRAvatar; }
    }

    [SerializeField]
    private bool spawnKBMAvatar;
    public bool SpawnKBMAvatar
    {
        get { return spawnKBMAvatar; }
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

    private void Start()
    {
        GameObject go = GameObject.Find("PlayerColorPicker");

        if (go != null)
            go.GetComponent<ColorPicker>().onValueChanged.AddListener(SetSharedBrushColor);
    }

    private void SetSharedBrushColor(Color color)
    {
        SharedBrushColor = color;
    }
}
