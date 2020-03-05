using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(DataReplayer))]
[CanEditMultipleObjects]
public class DataReplayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DataReplayer dataReplayer = (DataReplayer)target;
        
        if (GUILayout.Button("Replay Data"))
        {
            if (Application.isPlaying)
            {
                dataReplayer.StartReplay();
            }
            else
            {
                Debug.LogError("Game must be playing to start replay!");
            }
        }

        if (GUILayout.Button("Pause/Unpause Replay"))
        {
            if (Application.isPlaying)
            {
                dataReplayer.StartStopReplay();
            }
            else
            {
                Debug.LogError("Game must be playing to pause replay!");
            }
        }

        if (GUILayout.Button("Screenshot Data"))
        {
            if (Application.isPlaying)
            {
                dataReplayer.StartScreenshot();
            }
            else
            {
                Debug.LogError("Game must be playing to start screenshot!");
            }
        }

        if (GUILayout.Button("Clear Scene"))
        {
            dataReplayer.ClearReplay();
        }
    }
}
#endif