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
    private Color avatarColor;
    public Color AvatarColor
    {
        get { return avatarColor; }
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
