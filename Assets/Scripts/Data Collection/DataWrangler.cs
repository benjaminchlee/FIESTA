using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class DataWrangler : MonoBehaviour
{
    public List<TextAsset> playerDataFiles;
    public List<TextAsset> actionDataFiles;
    public List<TextAsset> annotationDataFiles;
    public List<TextAsset> objectDataFiles;

    [Range(1, 5)]
    public int groupID;
    public string outputFolder;

    public void ConsolidateAllPlayerAndActionData()
    {
        string path = string.Format("{0}Group{1}_AllData.txt", outputFolder, groupID);
        StreamWriter streamWriter = new StreamWriter(path, true);

        // Write header for question and participant
        streamWriter.Write("QuestionID\tParticipantID\tParticipantNo");
        // Write header for player data
        streamWriter.Write("\tTimestamp\tHeadPosition.x\tHeadPosition.y\tHeadPosition.z\t" +
                            "HeadRotation.x\tHeadRotation.y\tHeadRotation.z\tHeadRotation.w\t" +
                            "LeftPosition.x\tLeftPosition.y\tLeftPosition.z\t" +
                            "LeftRotation.x\tLeftRotation.y\tLeftRotation.z\tLeftRotation.w\t" +
                            "LeftTrigger\tLeftGrip\tLeftTouchpad\tLeftTouchpadAngle\t" +
                            "RightPosition.x\tRightPosition.y\tRightPosition.z\t" +
                            "RightRotation.x\tRightRotation.y\tRightRotation.z\tRightRotation.w\t" +
                            "RightTrigger\tRightGrip\tRightTouchpad\tRightTouchpadAngle\t" +
                            "GazeObject\tGazeObjectOriginalOwnerID\tGazeObjectOriginalOwnerNo\tGazeObjectOwnerID\tGazeObjectOwnerNo\tGazeObjectID\t" +
                            "LeftPointObject\tLeftPointObjectOriginalOwnerID\tLeftPointObjectOriginalOwnerNo\tLeftPointObjectOwnerID\tLeftPointOriginalObjectOwnerNo\tLeftPointObjectID\t" +
                            "RightPointObject\tRightPointObjectOriginalOwnerID\tRightPointObjectOriginalOwnerNo\tRightPointObjectOwnerID\tRightPointObjectOwnerNo\tRightPointObjectID");
        // Then write the rest of the header for action data
        streamWriter.Write("\tIsActionPerformed\tObjectType\tObjectOriginalOwnerID\tObjectOriginalOwnerNo\tObjectOwnerID\tObjectOwnerNo\tName\tTargetID\tDescription\r\n");

        int startIndex = (groupID - 1) * 12;

        for (int a = startIndex ; a < startIndex + 12; a++)
        {
            TextAsset playerData = playerDataFiles[a];
            TextAsset actionData = actionDataFiles[a];

            string[] playerDataLines = playerData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] actionDataLines = actionData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Get question and participant ID from file name
            string[] vals = playerData.name.Split('_');
            string questionID = vals[1][vals[1].Length - 1].ToString().Replace("k", "4");
            string participantID = vals[2].Replace("Participant", "");
            string participantNo = CalculateNumberFromID(participantID);

            // Write values
            int j = 1;
            float actionTimestamp = float.Parse(actionDataLines[1].Split('\t')[0]);

            for (int i = 1; i < playerDataLines.Length; i++)
            {
                // Write question, participant IDs and number
                streamWriter.Write("{0}\t{1}\t{2}", questionID, participantID, participantNo);

                // Write player values
                string[] playerValues = playerDataLines[i].Split('\t');
                streamWriter.Write("\t{0}", string.Join("\t", new string[] {
                    playerValues[0],    // Timestamp
                    playerValues[1],    // HeadPosition.x
                    playerValues[2],    // HeadPosition.y
                    playerValues[3],    // HeadPosition.z
                    playerValues[4],    // HeadRotation.x
                    playerValues[5],    // HeadRotation.y
                    playerValues[6],    // HeadRotation.z
                    playerValues[7],    // HeadRotation.w
                    playerValues[8],    // LeftPosition.x
                    playerValues[9],    // LeftPosition.y
                    playerValues[10],    // LeftPosition.z
                    playerValues[11],    // LeftRotation.x
                    playerValues[12],    // LeftRotation.y
                    playerValues[13],    // LeftRotation.z
                    playerValues[14],    // LeftRotation.w
                    playerValues[15],    // LeftTrigger
                    playerValues[16],    // LeftGrip
                    playerValues[17],    // LeftTouchpad
                    playerValues[18],    // LeftTouchpadAngle
                    playerValues[19],    // RightPosition.x
                    playerValues[20],    // RightPosition.y
                    playerValues[21],    // RightPosition.z
                    playerValues[22],    // RightRotation.x
                    playerValues[23],    // RightRotation.y
                    playerValues[24],    // RightRotation.z
                    playerValues[25],    // RightRotation.w
                    playerValues[26],    // RightTrigger
                    playerValues[27],    // RightGrip
                    playerValues[28],    // RightTouchpad
                    playerValues[29],    // RightTouchpadAngle
                    playerValues[30],    // GazeObject
                    playerValues[31],    // GazeObjectOriginalOwnerID
                    CalculateNumberFromID(playerValues[31]),    // GazeObjectOriginalOwnerNo
                    playerValues[32],    // GazeObjectOwnerID
                    CalculateNumberFromID(playerValues[32]),    // GazeObjectOwnerNo
                    playerValues[33],    // GazeObjectID
                    playerValues[34],    // LeftPointObject
                    playerValues[35],    // LeftPointObjectOriginalOwnerID
                    CalculateNumberFromID(playerValues[35]),    // LeftPointObjectOriginalOwnerNo
                    playerValues[36],    // LeftPointObjectOwnerID
                    CalculateNumberFromID(playerValues[36]),    // LeftPointObjectOwnerNo
                    playerValues[37],    // LeftPointObjectID
                    playerValues[38],    // RightPointObject
                    playerValues[39],    // RightPointObjectOriginalOwnerID
                    CalculateNumberFromID(playerValues[39]),    // RightPointObjectOriginalOwnerNo
                    playerValues[40],    // RightPointObjectOwnerID
                    CalculateNumberFromID(playerValues[40]),    // RightPointObjectOwnerNo
                    playerValues[41]    // RightPointObjectID
                }));
                
                // Write action values
                float timestamp = float.Parse(playerDataLines[i].Split('\t')[0]);
                if (actionTimestamp < timestamp && j < actionDataLines.Length)
                {
                    string[] actionValues = actionDataLines[j].Split('\t');
                    streamWriter.Write("\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}",
                        "True",
                        actionValues[1],    // ObjectType
                        actionValues[2],    // ObjectOriginalOwnerID
                        CalculateNumberFromID(actionValues[2]),    // ObjectOriginalOwnerNo
                        actionValues[3],    // ObjectOwnerID
                        CalculateNumberFromID(actionValues[3]),    // ObjectOwnerNo
                        actionValues[4],    // Name
                        actionValues[5],    // TargetID
                        actionValues[6]     // Description
                    );

                    j++;

                    if (j < actionDataLines.Length)
                        actionTimestamp = float.Parse(actionDataLines[j].Split('\t')[0]);
                }
                else
                {
                    streamWriter.Write("\tFalse\t\t\t\t\t\t\t\t");
                }
                streamWriter.Write("\r\n");
            }
        }

        streamWriter.Close();

        Debug.Log("Consolidating all player and action data done!");
    }

    public void ConsolidateObjectData()
    {
        string path = string.Format("{0}Study1_Group{1}_AllObjectData.txt", outputFolder, groupID);

        StreamWriter streamWriter = new StreamWriter(path, true);

        int startIndex = (groupID - 1) * 4;
        bool isHeaderWritten = false;
        int questionNo = 1;

        for (int a = startIndex; a < startIndex + 4; a++)
        {
            TextAsset objectData = objectDataFiles[a];

            string[] objectDataLines = objectData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (!isHeaderWritten)
            {
                // Write header
                streamWriter.Write("QuestionID\t");
                streamWriter.WriteLine(objectDataLines[0]);

                isHeaderWritten = true;
            }

            for (int i = 1; i < objectDataLines.Length; i++)
            {
                streamWriter.Write(questionNo + "\t");
                streamWriter.WriteLine(objectDataLines[i]);
            }

            questionNo++;
        }

        streamWriter.Close();

        Debug.Log("Consolidating all object data done!");
    }
    
    public void ProcessAllActionData()
    {
        string path = string.Format("{0}Group{1}_ProcessedActions.txt", outputFolder, groupID);
        StreamWriter streamWriter = new StreamWriter(path, true);

        // Write header
        streamWriter.WriteLine("QuestionID\tParticipantID\tParticipantNo\tStartTime\tEndTime\tObjectType\tObjectOriginalOwnerID\tObjectOriginalOwnerNo" +
            "\tObjectOwnerID\tObjectOwnerNo\tObjectName\tTargetID\tAction");

        int startIndex = (groupID - 1) * 12;

        for (int a = startIndex; a < startIndex + 12; a++)
        {
            TextAsset actionData = actionDataFiles[a];

            // Get question and participant ID from file name
            string[] vals = actionData.name.Split('_');
            string questionID = vals[1][vals[1].Length - 1].ToString().Replace("k", "4");
            string participantID = vals[2].Replace("Participant", "");
            string participantNo = CalculateNumberFromID(participantID);

            string[] actionDataLines = actionData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < actionDataLines.Length; i++)
            {
                string[] values = actionDataLines[i].Split('\t');

                string[] descriptionWords = values[6].Split(' ');
                string lastWord = descriptionWords[descriptionWords.Length - 1].ToLower();
                string action = RemoveLastWord(values[6]).ToLower().Replace("-", " ");

                switch (lastWord)
                {
                    case "start":
                        // Loop forward to find the closing action
                        for (int j = i + 1; j < actionDataLines.Length; j++)
                        {
                            string[] thisValues = actionDataLines[j].Split('\t');
                            string[] thisWords = thisValues[6].Split(' ');
                            string thisLastWord = thisWords[thisWords.Length - 1].ToLower();
                            string thisAction = RemoveLastWord(thisValues[6]).ToLower();

                            if (action == thisAction)
                            {
                                // Only write if this is the terminating word. If the same start event is found that means the original one is bugged and can be discarded
                                if (thisLastWord == "end")
                                {
                                    streamWriter.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}",
                                        questionID,
                                        participantID,
                                        participantNo,
                                        values[0],
                                        thisValues[0],
                                        values[1],
                                        values[2],
                                        CalculateNumberFromID(values[2]),
                                        values[3],
                                        CalculateNumberFromID(values[3]),
                                        values[4],
                                        values[5],
                                        action
                                    ));
                                }

                                break;
                            }
                        }
                        break;

                    case "end":
                        break;

                    default:
                        streamWriter.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}",
                            questionID,
                            participantID,
                            participantNo,
                            values[0],
                            values[0],
                            values[1],
                            values[2],
                            CalculateNumberFromID(values[2]),
                            values[3],
                            CalculateNumberFromID(values[3]),
                            values[4],
                            values[5],
                            values[6].ToLower()
                        ));
                        break;
                }
            }
        }

        streamWriter.Close();

        Debug.Log("Processing all action data done!");
    }

    public void ProcessLookingAtVisualisations()
    {
        string path = string.Format("{0}Study2_Group{1}_LookingAtVisualisationDuration.txt", outputFolder, groupID);
        StreamWriter streamWriter = new StreamWriter(path, true);

        // Write header
        streamWriter.WriteLine("QuestionID\tParticipantID\tParticipantNo\t2DVisualisationDuration\t3DVisualisationDuration\tTableDuration\tWallDuration\tInBetweenDuration\tTotalDuration");

        int startIndex = (groupID - 1) * 12;
        int startObjectIndex = (groupID - 1) * 4;

        for (int a = startIndex; a < startIndex + 12; a++)
        {
            TextAsset playerData = playerDataFiles[a];
            TextAsset objectData = objectDataFiles[startObjectIndex + Mathf.FloorToInt(a / 3) % 4];

            string[] playerDataLines = playerData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] objectDataLines = objectData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Get question and participant ID from file name
            string[] vals = playerData.name.Split('_');
            string questionID = vals[1][vals[1].Length - 1].ToString().Replace("k", "4");
            string participantID = vals[2].Replace("Participant", "");
            string participantNo = CalculateNumberFromID(participantID);

            // Write question, participant IDs and number
            streamWriter.Write("{0}\t{1}\t{2}", questionID, participantID, participantNo);

            // Calculate time spent looking at 2D and 3D visualisations
            int objectIndex = 1;  // an index to somewhat improve performance
            float duration2D = 0;
            float duration3D = 0;
            float durationTable = 0;
            float durationWall = 0;
            float durationInBetween = 0;

            for (int i = 2; i < playerDataLines.Length; i++)
            {
                string[] playerValues = playerDataLines[i].Split('\t');
                string visualisationID = playerValues[33];

                if (visualisationID != "")
                {
                    // Get duration from previous timestamp to this timestamp
                    float currentTimestamp = float.Parse(playerValues[0]);
                    float timestampDuration = currentTimestamp - float.Parse(playerDataLines[i - 1].Split('\t')[0]);

                    // Loop through object data to find the timestamp of the looked at visualisation
                    for (int j = objectIndex; j < objectDataLines.Length; j++)
                    {
                        string[] objectValues = objectDataLines[j].Split('\t');

                        if (float.Parse(objectValues[0]) > currentTimestamp && objectValues[14] == visualisationID)
                        {
                            objectIndex = j;

                            if (objectValues[17] == "Undefined")
                            {
                                duration2D += timestampDuration;
                            }
                            else
                            {
                                duration3D += timestampDuration;
                            }

                            float x = float.Parse(objectValues[4]);
                            float z = float.Parse(objectValues[6]);

                            // If the visualisation is in the table region
                            if ((-0.5f < x && x < 0.5f) && (-0.5f < z && z < 0.5f))
                            {
                                durationTable += timestampDuration;
                            }
                            // If the visualisation is in the in-between region
                            else if ((-1.5f < x && x < 1.5f) && (-1.5f < z && z < 1.5f))
                            {
                                durationInBetween += timestampDuration;
                            }
                            // If the visualisation is in the wall region
                            else if ((-2f < x && x < 2f) && (-2f < z && z < 2f))
                            {
                                durationWall += timestampDuration;
                            }

                            break;
                        }
                    }

                    // Reset object index to something further back
                    objectIndex -= 100;
                    objectIndex = Mathf.Max(objectIndex, 1);
                }
            }

            // Write rest of the calculated values
            streamWriter.WriteLine("\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}", duration2D, duration3D, durationTable, durationWall, durationInBetween, float.Parse(objectDataLines[objectDataLines.Length - 1].Split('\t')[0]));
        }

        streamWriter.Close();

        Debug.Log("Processing players looking at visualisations done!");
    }


    private string RemoveLastWord(string sentence)
    {
        List<string> values = sentence.Split(' ').ToList();
        values.RemoveAt(values.Count - 1);
        return string.Join(" ", values.ToArray()).Trim();
    }

    private string CalculateNumberFromID(int id)
    {
        return ((id - 1) % 3 + 1).ToString();
    }

    private string CalculateNumberFromID(string id)
    {
        if (id == "")
            return "";

        return CalculateNumberFromID(int.Parse(id));
    }
}
