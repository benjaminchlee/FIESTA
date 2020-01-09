using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(DataWrangler))]
[CanEditMultipleObjects]
[ExecuteInEditMode]
public class DataWranglerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DataWrangler dataWrangler = (DataWrangler)target;

        if (GUILayout.Button("Consolidate All Player and Action Data"))
        {
            dataWrangler.ConsolidateAllPlayerAndActionData();
        }

        if (GUILayout.Button("Consolidate All Object Data"))
        {
            dataWrangler.ConsolidateObjectData();
        }

        if (GUILayout.Button("Process Action Data"))
        {
            dataWrangler.ProcessAllActionData();
        }

        if (GUILayout.Button("Process Player Looking at Visualisation Duration"))
        {
            dataWrangler.ProcessLookingAtVisualisations();
        }

        //if (GUILayout.Button("Calculate Player Distances"))
        //{
        //    //dataWrangler.CalculateDistanceTravelled();
        //}
    }
}
#endif