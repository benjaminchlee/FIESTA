using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReconstructor : MonoBehaviour
{
    public GameObject dummyVisualisationPrefab;

    public List<TextAsset> playerDataFiles;
    public List<TextAsset> objectDataFiles;

    [Range(1, 5)]
    public int groupID = 1;
    [Range(1, 4)]
    public int questionID = 1;
    
    private void StartReconstructingVisualisationLocations()
    {

    }

    private void ReconstructVisualisationLocations()
    {
        string[] playerDataLines1 = playerDataFiles[(groupID - 1) * 12 + (questionID - 1) * 3].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines2 = playerDataFiles[(groupID - 1) * 12 + (questionID - 1) * 3 + 1].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines3 = playerDataFiles[(groupID - 1) * 12 + (questionID - 1) * 3 + 2].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] objectDataLines = objectDataFiles[(groupID - 1) * 4 + questionID - 1].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < playerDataLines1.Length; i++)
        {
            
        }

    }
}
