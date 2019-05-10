using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataWrangler : MonoBehaviour {

    public List<TextAsset> playerDataFiles;
    public List<TextAsset> actionDataFiles;
    public List<TextAsset> raycastDataFiles;

    public List<TextAsset> mergedDataFiles;
    public string group;

    public List<TextAsset> allDataFiles;

    public bool mergeActionPlayer = false;
    public bool mergeParticipantQuestion = false;
    public bool mergeAllFiles = false;
    public bool processActionFiles = false;
    public bool calculateDistanceTravelled = false;


    private void Start()
    {
        if (playerDataFiles.Count == actionDataFiles.Count && mergeActionPlayer)
            MergeActionAndPlayerData();

        if (mergeParticipantQuestion)
            MergeParticipantsAndQuestions();

        if (mergeAllFiles)
            MergeAllFiles();

        if (processActionFiles)
            ProcessActionFiles();;

        if (calculateDistanceTravelled)
            CalculateDistanceTravelled();
    }

    private void MergeActionAndPlayerData()
    {
        for (int a = 0; a < actionDataFiles.Count; a++)
        {
            TextAsset playerData = playerDataFiles[a];
            TextAsset actionData = actionDataFiles[a];

            string[] vals = playerData.name.Split('_');
            
            string path = string.Format("C:\\Users\\blee33\\Desktop\\{0}_{1}.txt", vals[1][vals[1].Length - 1], vals[2].Replace("Participant", ""));
            StreamWriter streamWriter = new StreamWriter(path, true);

            string[] playerDataLines = playerData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] actionDataLines = actionData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Write header for player data
            streamWriter.Write("Timestamp\t" +
                                   "HeadPosition.x\tHeadPosition.y\tHeadPosition.z\t" +
                                   "HeadRotation.x\tHeadRotation.y\tHeadRotation.z\tHeadRotation.w\t" +
                                   "LeftPosition.x\tLeftPosition.y\tLeftPosition.z\t" +
                                   "LeftRotation.x\tLeftRotation.y\tLeftRotation.z\tLeftRotation.w\t" +
                                   "LeftTrigger\tLeftGrip\tLeftTouchpad\tLeftTouchpadAngle\t" +
                                   "RightPosition.x\tRightPosition.y\tRightPosition.z\t" +
                                   "RightRotation.x\tRightRotation.y\tRightRotation.z\tRightRotation.w\t" +
                                   "RightTrigger\tRightGrip\tRightTouchpad\tRightTouchpadAngle");

            // Write header for action data
            streamWriter.Write("\tIsActionPerformed\tObjectType\tOriginalOwner\tName\tDescription\r\n");

            int j = 1;
            float actionTimestamp = float.Parse(actionDataLines[1].Split('\t')[0]);

            for (int i = 1; i < playerDataLines.Length; i++)
            {
                streamWriter.Write(playerDataLines[i]);

                float timestamp = float.Parse(playerDataLines[i].Split('\t')[0]);

                if (actionTimestamp < timestamp && j < actionDataLines.Length)
                {
                    string[] actionValues = actionDataLines[j].Split('\t');

                    streamWriter.Write("\t{0}\t{1}\t{2}\t{3}\t{4}",
                        "True",
                        actionValues[1],
                        actionValues[2],
                        actionValues[3],
                        actionValues[4]);
                    
                    j++;

                    if (j < actionDataLines.Length)
                        actionTimestamp = float.Parse(actionDataLines[j].Split('\t')[0]);
                }
                else
                {
                    streamWriter.Write("\tFalse\t\t\t\t");
                }

                streamWriter.Write("\r\n");
            }

            streamWriter.Close();
        }
    }

    private void MergeParticipantsAndQuestions()
    {
        string path = string.Format("C:\\Users\\blee33\\Desktop\\Group{0}_MergedData.txt", group);
        StreamWriter streamWriter = new StreamWriter(path, true);

        // Write header
        streamWriter.WriteLine("Timestamp\tParticipantID\tQuestionID\t" +
                           "HeadPosition.x\tHeadPosition.y\tHeadPosition.z\t" +
                           "HeadRotation.x\tHeadRotation.y\tHeadRotation.z\tHeadRotation.w\t" +
                           "LeftPosition.x\tLeftPosition.y\tLeftPosition.z\t" +
                           "LeftRotation.x\tLeftRotation.y\tLeftRotation.z\tLeftRotation.w\t" +
                           "LeftTrigger\tLeftGrip\tLeftTouchpad\tLeftTouchpadAngle\t" +
                           "RightPosition.x\tRightPosition.y\tRightPosition.z\t" +
                           "RightRotation.x\tRightRotation.y\tRightRotation.z\tRightRotation.w\t" +
                           "RightTrigger\tRightGrip\tRightTouchpad\tRightTouchpadAngle\t" +
                           "IsActionPerformed\tObjectType\tOriginalOwner\tName\tDescription");
        
        for (int a = 0; a < mergedDataFiles.Count; a++)
        {
            TextAsset fileToMerge = mergedDataFiles[a];

            string participantID = fileToMerge.name.Split('_')[1];
            string questionID = fileToMerge.name.Split('_')[0];

            string[] lines = fileToMerge.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i++)
            {
                List<string> values = lines[i].Split('\t').ToList();

                values.Insert(1, participantID);
                values.Insert(2, questionID);

                streamWriter.WriteLine(string.Join("\t", values.ToArray()));
            }
        }

        streamWriter.Close();
    }

    private void MergeAllFiles()
    {
        string path = string.Format("C:\\Users\\blee33\\Desktop\\Group{0}_AllMergedData.txt", group);
        StreamWriter streamWriter = new StreamWriter(path, true);

        // Write header
        streamWriter.WriteLine("Timestamp\tParticipantID\tQuestionID\t" +
                               "HeadPosition.x\tHeadPosition.y\tHeadPosition.z\t" +
                               "HeadRotation.x\tHeadRotation.y\tHeadRotation.z\tHeadRotation.w\t" +
                               "LeftPosition.x\tLeftPosition.y\tLeftPosition.z\t" +
                               "LeftRotation.x\tLeftRotation.y\tLeftRotation.z\tLeftRotation.w\t" +
                               "LeftTrigger\tLeftGrip\tLeftTouchpad\tLeftTouchpadAngle\t" +
                               "RightPosition.x\tRightPosition.y\tRightPosition.z\t" +
                               "RightRotation.x\tRightRotation.y\tRightRotation.z\tRightRotation.w\t" +
                               "RightTrigger\tRightGrip\tRightTouchpad\tRightTouchpadAngle\t" +
                               "GazeObject\tGazeObjectOwner\tLeftPointObject\tLeftPointObjectOwner\tRightPointObject\tRightPointObjectOwner\t" +
                               "IsActionPerformed\tObjectType\tOriginalOwner\tName\tDescription");

        // Combine a participant's player data, action data, and raycast data
        for (int a = 0; a < playerDataFiles.Count; a++)
        {
            TextAsset playerData = playerDataFiles[a];
            TextAsset actionData = actionDataFiles[a];
            TextAsset raycastData = raycastDataFiles[a];

            string[] playerDataLines = playerData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] actionDataLines = actionData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] raycastDataLines = raycastData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Get question and participant ID from file name
            string[] vals = playerData.name.Split('_');
            string questionID = vals[1][vals[1].Length - 1].ToString().Replace("n", "4");
            string participantID = vals[2].Replace("Participant", "");
            
            // Write values
            int j = 1;
            float actionTimestamp = float.Parse(actionDataLines[1].Split('\t')[0]);

            for (int i = 1; i < playerDataLines.Length; i++)
            {
                // Write player values with question and participant IDs
                List<string> playerValues = playerDataLines[i].Split('\t').ToList();
                playerValues.Insert(1, participantID);
                playerValues.Insert(2, questionID);
                streamWriter.Write(string.Join("\t", playerValues.ToArray()));

                // Write raycast values
                List<string> raycastValues = raycastDataLines[i].Split('\t').ToList();
                raycastValues.RemoveAt(0);
                streamWriter.Write("\t" + string.Join("\t", raycastValues.ToArray()));

                // Write action values
                float timestamp = float.Parse(playerDataLines[i].Split('\t')[0]);

                if (actionTimestamp < timestamp && j < actionDataLines.Length)
                {
                    string[] actionValues = actionDataLines[j].Split('\t');

                    streamWriter.Write("\t{0}\t{1}\t{2}\t{3}\t{4}",
                        "True",
                        actionValues[1],
                        actionValues[2],
                        actionValues[3],
                        actionValues[4]);

                    j++;

                    if (j < actionDataLines.Length)
                        actionTimestamp = float.Parse(actionDataLines[j].Split('\t')[0]);
                }
                else
                {
                    streamWriter.Write("\tFalse\t\t\t\t");
                }

                streamWriter.Write("\r\n");
            }
        }
        
        streamWriter.Close();
    }

    private void ProcessActionFiles()
    {
        string path = string.Format("C:\\Users\\blee33\\Desktop\\Group{0}_ProcessedActions.txt", group);
        StreamWriter streamWriter = new StreamWriter(path, true);

        // Write header
        streamWriter.WriteLine("Question\tParticipantID\tParticipantNo\tStartTime\tEndTime\tObjectType\tObjectName\tObjectOriginalOwnerID\tObjectOriginalOwnerNo\tAction");

        for (int a = 0; a < actionDataFiles.Count; a++)
        {
            TextAsset actionData = actionDataFiles[a];

            // Get question and participant ID from file name
            string[] vals = actionData.name.Split('_');
            string questionID = vals[1][vals[1].Length - 1].ToString().Replace("n", "4");
            string participantID = vals[2].Replace("Participant", "");
            int participantNo = GetPlayerNumber(participantID);

            string[] actionDataLines = actionData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < actionDataLines.Length; i++)
            {
                string[] values = actionDataLines[i].Split('\t');

                string[] descriptionWords = values[4].Split(' ');
                string lastWord = descriptionWords[descriptionWords.Length - 1].ToLower();
                string action = RemoveLastWord(values[4]).ToLower();

                switch (lastWord)
                {
                    case "start":
                        // Loop forward to find the closing action
                        for (int j = i + 1; j < actionDataLines.Length; j++)
                        {
                            string[] thisValues = actionDataLines[j].Split('\t');
                            string[] thisWords = thisValues[4].Split(' ');
                            string thisLastWord = thisWords[thisWords.Length - 1].ToLower();
                            string thisAction = RemoveLastWord(thisValues[4]).ToLower();

                            if (action == thisAction)
                            {
                                // Only write if this is the terminating word. If the same start event is found that means the original one is bugged and can be discarded
                                if (thisLastWord == "end")
                                {
                                    streamWriter.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}",
                                        questionID,
                                        participantID,
                                        participantNo,
                                        values[0],
                                        thisValues[0],
                                        values[1],
                                        values[3],
                                        values[2],
                                        GetPlayerNumber(values[2]),
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
                        streamWriter.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}",
                            questionID,
                            participantID,
                            participantNo,
                            values[0],
                            values[0],
                            values[1],
                            values[3],
                            values[2],
                            GetPlayerNumber(values[2]),
                            values[4]
                        ));
                        break;

                }
            }
        }

        streamWriter.Close();
    }

    private void CalculateDistanceTravelled()
    {
        for (int i = 0; i < allDataFiles.Count; i++)
        {
            string[] lines = allDataFiles[i].text.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            List<string> valueLines = lines.ToList();
            valueLines.RemoveAt(0);

            valueLines = valueLines.OrderBy(x => x.Split('\t')[1])
                .ThenBy(x => x.Split('\t')[3])
                .ThenBy(x => float.Parse(x.Split('\t')[0]))
                .ToList();

            valueLines.Insert(0, lines[0]);
            lines = valueLines.ToArray();

            string path = string.Format("H:\\Group{0}_AllDataDistances.txt", i);
            StreamWriter streamWriter = new StreamWriter(path, true);

            // Write header
            streamWriter.WriteLine(lines[0] + "\tDistanceTravelled");

            // Count up distance travelled per each participant/question
            string participant = "";
            string question = "";
            float distance = 0;
            Vector2 previousPos = Vector2.zero;

            OneEuroFilter<Vector2> filter = new OneEuroFilter<Vector2>(20f);

            for (int j = 1; j < lines.Length; j++)
            {
                string[] values = lines[j].Split('\t');

                Vector2 newPos = filter.Filter(new Vector2(float.Parse(values[4]), float.Parse(values[6])));

                // If new question
                if (values[1] != participant || values[3] != question)
                {
                    participant = values[1];
                    question = values[3];
                    distance = 0;
                }
                else
                {
                    distance += Vector2.Distance(newPos, previousPos);
                }

                // Write distance to file
                streamWriter.WriteLine(lines[j] + '\t' + distance);
                previousPos = newPos;
            }

            streamWriter.Close();
        }
    }

    private string RemoveLastWord(string sentence)
    {
        List<string> values = sentence.Split(' ').ToList();
        
        values.RemoveAt(values.Count - 1);

        return string.Join(" ", values.ToArray()).Trim();
    }

    private int GetPlayerNumber(string value)
    {
        int participantNo = int.Parse(value.Trim()) % 3;
        if (participantNo == 0) participantNo = 3;

        return participantNo;
    }
}
