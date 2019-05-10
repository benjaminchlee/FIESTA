using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataLogger))]
[CanEditMultipleObjects]
public class DataLoggerEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        DataLogger dataLogger = (DataLogger) target;

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
