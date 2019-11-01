using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(DataLogger))]
[CanEditMultipleObjects]
public class DataLoggerEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        DataLogger dataLogger = (DataLogger) target;

        if (dataLogger.isMasterLogger)
        {
            var loggingProperty = serializedObject.FindProperty("isLoggingPlayerData");
            EditorGUILayout.PropertyField(loggingProperty);
            serializedObject.ApplyModifiedProperties();

            //dataLogger.isLoggingPlayerData = EditorGUILayout.Toggle("Is Logging PlayerData", dataLogger.isLoggingPlayerData);
        }
        
        string[] stringOptions = dataLogger.tasks.Select(x => "Task " + x.taskName).ToArray();
        int[] intOptions = Enumerable.Range(0, dataLogger.tasks.Count).ToArray();

        dataLogger.taskID = EditorGUILayout.IntPopup("Current Task", dataLogger.taskID, stringOptions, intOptions);

        if (GUILayout.Button("Start Logging"))
        {
            if (!dataLogger.IsLogging())
            {
                dataLogger.StartLogging();
            }
        }

        if (GUILayout.Button("Stop Logging"))
        {
            if (dataLogger.IsLogging())
            {
                dataLogger.StopLogging();
            }
        }
    }
}
#endif
