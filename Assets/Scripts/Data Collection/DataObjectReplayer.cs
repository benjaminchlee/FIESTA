using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;

public class DataObjectReplayer : MonoBehaviour
{
    public GameObject dummyPanelPrefab;
    public GameObject dummyVisualisationPrefab;
    public GameObject dummyPlayerPrefab;

    public GameObject panelPrefab;
    public GameObject visualisationPrefab;
    public GameObject playerPrefab;

    public TextAsset objectData;

    public TextAsset playerData1;

    public TextAsset playerData2;

    public TextAsset playerData3;

    public string question;

    private DummyObject playerDummy1;
    private DummyObject playerDummy2;
    private DummyObject playerDummy3;

    private DummyObject panelDummy1;
    private DummyObject panelDummy2;
    private DummyObject panelDummy3;

    private List<DummyObject> visualisationDummies;

    public List<TextAsset> heatmapDataFiles;
    
    [Header("Replay And Screenshot")]
    public List<TextAsset> playerDataFiles;
    public List<TextAsset>  objectDataFiles;
    public TextAsset videoCodingDataFile;
    public string groupID;
    public int playerID1;
    public int playerID2;
    public int playerID3;
    private List<GameObject> players;
    private List<GameObject> objects;

    private void Start()
    {
        //StartLiveReplay();
        //StartLiveReplayAndRaycast();
        //ReplayAndRaycast();
        StartFullReplayAndScreenshot();
        //StartCollaborationReplayAndScreenshot();
    }

    private void StartLiveReplay()
    {
        StartCoroutine(LiveReplay());
    }

    private void StartLiveReplayAndRaycast()
    {
        StartCoroutine(LiveReplayAndRaycast());
    }

    private void StartFullReplayAndScreenshot()
    {
        StartCoroutine(FullReplayAndScreenshot());
    }

    private void StartCollaborationReplayAndScreenshot()
    {
        StartCoroutine(CollaborationReplayAndScreenshot());
    }

    private IEnumerator LiveReplay()
    {
        string[] objectDataLines = heatmapDataFiles[0].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines1 = heatmapDataFiles[1].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines2 = heatmapDataFiles[2].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines3 = heatmapDataFiles[3].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int j = 1;

        for (int i = 1; i < playerDataLines1.Length; i+=100)
        {
            float currentTime = float.Parse(playerDataLines1[i].Split('\t')[0]);

            // Position objects at this timestamp
            while (j < objectDataLines.Length)
            {
                string[] values = objectDataLines[j].Split('\t');

                if (IsEqual(float.Parse(values[0]), currentTime))
                {
                    switch (values[1])
                    {
                        case "Panel":
                            GameObject panel = Instantiate(panelPrefab);
                            panel.transform.position = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
                            panel.transform.rotation = new Quaternion(float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]), float.Parse(values[9]));
                            ColorObject(panel, int.Parse(values[2]));
                            break;

                        case "Visualisation":
                            GameObject vis = Instantiate(visualisationPrefab);
                            vis.transform.position = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
                            vis.transform.rotation = new Quaternion(float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]), float.Parse(values[9]));
                            vis.transform.localScale = new Vector3(float.Parse(values[10]), float.Parse(values[11]), 1);
                            ColorObject(vis, int.Parse(values[2]));
                            break;
                    }

                }
                else
                {
                    if (float.Parse(values[0]) > currentTime)
                    {
                        break;
                    }
                }

                j++;
            }
            /*
            // Position players at this timestamp
            // Player 1
            string[] playerValues1 = playerDataLines1[i].Split('\t');

            GameObject player1 = Instantiate(playerPrefab);
            player1.transform.position = new Vector3(float.Parse(playerValues1[1]), float.Parse(playerValues1[2]), float.Parse(playerValues1[3]));
            player1.transform.rotation = new Quaternion(float.Parse(playerValues1[4]), float.Parse(playerValues1[5]), float.Parse(playerValues1[6]), float.Parse(playerValues1[7]));
            ColorObject(player1, 1);

            // Player 2
            string[] playerValues2 = playerDataLines2[i].Split('\t');

            GameObject player2 = Instantiate(playerPrefab);
            player2.transform.position = new Vector3(float.Parse(playerValues2[1]), float.Parse(playerValues2[2]), float.Parse(playerValues2[3]));
            player2.transform.rotation = new Quaternion(float.Parse(playerValues2[4]), float.Parse(playerValues2[5]), float.Parse(playerValues2[6]), float.Parse(playerValues2[7]));
            ColorObject(player2, 2);

            // Player 3
            string[] playerValues3 = playerDataLines3[i].Split('\t');

            GameObject player3 = Instantiate(playerPrefab);
            player3.transform.position = new Vector3(float.Parse(playerValues3[1]), float.Parse(playerValues3[2]), float.Parse(playerValues3[3]));
            player3.transform.rotation = new Quaternion(float.Parse(playerValues3[4]), float.Parse(playerValues3[5]), float.Parse(playerValues3[6]), float.Parse(playerValues3[7]));
            ColorObject(player3, 3);
            */
            //yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private IEnumerator CollaborationReplayAndScreenshot()
    {
        players = new List<GameObject>();
        objects = new List<GameObject>();

        string[] videoCodingDataLines = videoCodingDataFile.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int q = 1; q <= 4; q++)
        {
            string[] playerDataLines1 = playerDataFiles[(q - 1) * 3].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] playerDataLines2 = playerDataFiles[(q - 1) * 3 + 1].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] playerDataLines3 = playerDataFiles[(q - 1) * 3 + 2].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] objectDataLines = objectDataFiles[q - 1].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Filter video coding events to only include collaborative events for this question
            List<string> filteredVideoCodingLines = new List<string>();

            string ques = (q == 4) ? "n" : q.ToString();

            for (int i = 1; i < videoCodingDataLines.Length; i++)
            {
                string[] values = videoCodingDataLines[i].Split('\t');

                if (values[0].EndsWith(ques) && values[7].ToLower() == "close collaboration")
                {
                    filteredVideoCodingLines.Add(videoCodingDataLines[i]);
                }
            }

            // Loop through each event
            for (int i = 0; i < filteredVideoCodingLines.Count; i++)
            {
                string[] values = filteredVideoCodingLines[i].Split('\t');

                float start = float.Parse(values[10]);
                float end = float.Parse(values[11]);
                string subjects = values[5];
                string behavior = values[6];

                // Find the start index for this event
                int playerIdx = -1;

                for (int j = 1; j < playerDataLines1.Length; j++)
                {
                    if (float.Parse(playerDataLines1[j].Split('\t')[0]) >= start)
                    {
                        playerIdx = j;
                        break;
                    }
                }

                // If an index was found
                if (playerIdx != -1)
                {
                    // Replay events from this index
                    for (int j = playerIdx; j < playerDataLines1.Length; j += 10)
                    {
                        string[] playerValues1 = playerDataLines1[j].Split('\t');
                        string[] playerValues2 = playerDataLines2[j].Split('\t');
                        string[] playerValues3 = playerDataLines3[j].Split('\t');

                        // If the replay goes over the end time, stop
                        if (float.Parse(playerValues1[0]) > end)
                        {
                            break;
                        }

                        // Position players at this timestamp
                        // Player 1
                        GameObject player1 = Instantiate(playerPrefab);
                        player1.transform.position = new Vector3(float.Parse(playerValues1[1]), float.Parse(playerValues1[2]), float.Parse(playerValues1[3]));
                        player1.transform.rotation = new Quaternion(float.Parse(playerValues1[4]), float.Parse(playerValues1[5]), float.Parse(playerValues1[6]), float.Parse(playerValues1[7]));
                        players.Add(player1);

                        // Player 2
                        GameObject player2 = Instantiate(playerPrefab);
                        player2.transform.position = new Vector3(float.Parse(playerValues2[1]), float.Parse(playerValues2[2]), float.Parse(playerValues2[3]));
                        player2.transform.rotation = new Quaternion(float.Parse(playerValues2[4]), float.Parse(playerValues2[5]), float.Parse(playerValues2[6]), float.Parse(playerValues2[7]));
                        players.Add(player2);

                        // Player 3
                        GameObject player3 = Instantiate(playerPrefab);
                        player3.transform.position = new Vector3(float.Parse(playerValues3[1]), float.Parse(playerValues3[2]), float.Parse(playerValues3[3]));
                        player3.transform.rotation = new Quaternion(float.Parse(playerValues3[4]), float.Parse(playerValues3[5]), float.Parse(playerValues3[6]), float.Parse(playerValues3[7]));
                        players.Add(player3);

                        // Color players based on the event
                        ColorObject(player1, (subjects.Contains("R")) ? 1 : -1);
                        ColorObject(player2, (subjects.Contains("B")) ? 2 : -1);
                        ColorObject(player3, (subjects.Contains("G")) ? 3 : -1);

                        // Position objects
                        // Find index in object files that matches the current time
                        float currentTime = float.Parse(playerDataLines1[j].Split('\t')[0]);
                        int k = 0;
                        for (int l = 1; l < objectDataLines.Length; l++)
                        {
                            float t = float.Parse(objectDataLines[l].Split('\t')[0]);
                            if (IsEqual(currentTime, t))
                            {
                                k = l;
                                break;
                            }
                        }

                        // Position objects at this timestamp
                        while (k < objectDataLines.Length)
                        {
                            string[] objectValues = objectDataLines[k].Split('\t');

                            if (IsEqual(float.Parse(objectValues[0]), currentTime))
                            {
                                GameObject go = null;

                                switch (objectValues[1])
                                {
                                    case "Panel":
                                        go = Instantiate(panelPrefab);
                                        go.transform.position = new Vector3(float.Parse(objectValues[3]), float.Parse(objectValues[4]), float.Parse(objectValues[5]));
                                        go.transform.rotation = new Quaternion(float.Parse(objectValues[6]), float.Parse(objectValues[7]), float.Parse(objectValues[8]), float.Parse(objectValues[9]));
                                        break;

                                    case "Visualisation":
                                        go = Instantiate(visualisationPrefab);
                                        go.transform.position = new Vector3(float.Parse(objectValues[3]), float.Parse(objectValues[4]), float.Parse(objectValues[5]));
                                        go.transform.rotation = new Quaternion(float.Parse(objectValues[6]), float.Parse(objectValues[7]), float.Parse(objectValues[8]), float.Parse(objectValues[9]));
                                        go.transform.localScale = new Vector3(float.Parse(objectValues[10]), float.Parse(objectValues[11]), 1);
                                        break;
                                }

                                if (go != null)
                                {
                                    int owner = int.Parse(objectValues[2]) % 3;
                                    if (owner == 0) owner = 3;
                                    switch (owner)
                                    {
                                        case 1:
                                            ColorObject(go, (subjects.Contains("R")) ? 1 : -1);
                                            break;
                                        case 2:
                                            ColorObject(go, (subjects.Contains("B")) ? 2 : -1);
                                            break;
                                        case 3:
                                            ColorObject(go, (subjects.Contains("G")) ? 3 : -1);
                                            break;
                                    }

                                    objects.Add(go);
                                }
                            }
                            else
                            {
                                if (float.Parse(objectValues[0]) > currentTime)
                                {
                                    break;
                                }
                            }
                            
                            k++;
                        }
                    }
                }

                yield return new WaitForEndOfFrame();

                string filename = string.Format("C:\\Users\\blee33\\Desktop\\Group{0}_Q{1}_{2}_{3}_{4}.png",
                    groupID,
                    q,
                    behavior.Replace(" ", "").Replace("/", "Or"),
                    start,
                    subjects.Replace("\"", "")
                );

                ScreenCapture.CaptureScreenshot(filename);

                foreach (var go in players)
                {
                    DestroyImmediate(go);
                }

                foreach (var go in objects)
                {
                    DestroyImmediate(go);
                }

                yield return new WaitForEndOfFrame();
            }

        }
    }

    private IEnumerator FullReplayAndScreenshot()
    {
        players = new List<GameObject>();
        objects = new List<GameObject>();
        
        for (int q = 1; q <= 4; q++)
        {
            string[] playerDataLines1 = playerDataFiles[(q - 1) * 3].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] playerDataLines2 = playerDataFiles[(q - 1) * 3 + 1].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] playerDataLines3 = playerDataFiles[(q - 1) * 3 + 2].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] objectDataLines = objectDataFiles[q - 1].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int numLines = Mathf.Min(playerDataLines1.Length, playerDataLines2.Length, playerDataLines3.Length);
            int j = 1;

            // Replay
            for (int i = 1; i < numLines; i += 100)
            {
                string[] playerValues1;
                string[] playerValues2;
                string[] playerValues3;

                try
                {
                    playerValues1 = playerDataLines1[i].Split('\t');
                    playerValues2 = playerDataLines2[i].Split('\t');
                    playerValues3 = playerDataLines3[i].Split('\t');
                }
                catch (Exception e)
                {
                    break;
                }

                // Position players at this timestamp
                // Player 1
                GameObject player1 = Instantiate(playerPrefab);
                player1.transform.position = new Vector3(float.Parse(playerValues1[1]), float.Parse(playerValues1[2]), float.Parse(playerValues1[3]));
                player1.transform.rotation = new Quaternion(float.Parse(playerValues1[4]), float.Parse(playerValues1[5]), float.Parse(playerValues1[6]), float.Parse(playerValues1[7]));
                players.Add(player1);

                // Player 2
                GameObject player2 = Instantiate(playerPrefab);
                player2.transform.position = new Vector3(float.Parse(playerValues2[1]), float.Parse(playerValues2[2]), float.Parse(playerValues2[3]));
                player2.transform.rotation = new Quaternion(float.Parse(playerValues2[4]), float.Parse(playerValues2[5]), float.Parse(playerValues2[6]), float.Parse(playerValues2[7]));
                players.Add(player2);

                // Player 3
                GameObject player3 = Instantiate(playerPrefab);
                player3.transform.position = new Vector3(float.Parse(playerValues3[1]), float.Parse(playerValues3[2]), float.Parse(playerValues3[3]));
                player3.transform.rotation = new Quaternion(float.Parse(playerValues3[4]), float.Parse(playerValues3[5]), float.Parse(playerValues3[6]), float.Parse(playerValues3[7]));
                players.Add(player3);

                // Color players
                ColorObject(player1, playerID1);
                ColorObject(player2, playerID2);
                ColorObject(player3, playerID3);

                float currentTime = float.Parse(playerDataLines1[i].Split('\t')[0]);
                
                // Position objects at this timestamp
                while (j < objectDataLines.Length)
                {
                    string[] objectValues = objectDataLines[j].Split('\t');

                    if (IsEqual(float.Parse(objectValues[0]), currentTime))
                    {
                        GameObject go = null;

                        switch (objectValues[1])
                        {
                            case "Panel":
                                go = Instantiate(panelPrefab);
                                go.transform.position = new Vector3(float.Parse(objectValues[3]), float.Parse(objectValues[4]), float.Parse(objectValues[5]));
                                go.transform.rotation = new Quaternion(float.Parse(objectValues[6]), float.Parse(objectValues[7]), float.Parse(objectValues[8]), float.Parse(objectValues[9]));
                                break;

                            case "Visualisation":
                                go = Instantiate(visualisationPrefab);
                                go.transform.position = new Vector3(float.Parse(objectValues[3]), float.Parse(objectValues[4]), float.Parse(objectValues[5]));
                                go.transform.rotation = new Quaternion(float.Parse(objectValues[6]), float.Parse(objectValues[7]), float.Parse(objectValues[8]), float.Parse(objectValues[9]));
                                //go.transform.localScale = new Vector3(float.Parse(objectValues[10]), float.Parse(objectValues[11]), 1);
                                go.transform.localScale = new Vector3(0.6f, 0.6f, 1);
                                break;
                        }

                        if (go != null)
                        {
                            int owner = int.Parse(objectValues[2]) % 3;
                            if (owner == 0) owner = 3;

                            ColorObject(go, owner);
                            objects.Add(go);
                        }
                    }

                    if (float.Parse(objectValues[0]) > currentTime)
                    {
                        break;
                    }
                    else
                    {
                        j++;
                    }
                }
            }

            yield return new WaitForEndOfFrame();

            string filename = string.Format("C:\\Users\\blee33\\Desktop\\Group{0}_Q{1}_ObjectHeatmap.png",
                groupID,
                q
            );

            ScreenCapture.CaptureScreenshot(filename);

            foreach (var go in players)
            {
                DestroyImmediate(go);
            }

            foreach (var go in objects)
            {
                DestroyImmediate(go);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void ColorObject(GameObject go, int playerID)
    {
        int id = playerID % 3;
        if (id == 0) id = 3;

        Color col = Color.black;;

        switch (id)
        {
            case 1:
                col = new Color(0.843f, 0.188f, 0.153f, 0.075f);
                break;

            case 2:
                col = new Color(0.263f, 0.576f, 0.765f, 0.075f);
                break;

            case 3:
                col = new Color(0.4f, 0.741f, 0.388f, 0.075f);
                break;

            default:
                col = new Color(0.9f, 0.9f, 0.9f, 0.075f);
                break;
        }

        var renderers = go.GetComponentsInChildren<Renderer>();

        foreach (var renderer in renderers)
        {
            renderer.material.shader = Shader.Find("Unlit/OverdrawShader");
            renderer.material.SetColor("_Color", col);
        }
    }

    private IEnumerator LiveReplayAndRaycast()
    {
        playerDummy1 = Instantiate(dummyPlayerPrefab).GetComponent<DummyObject>();
        playerDummy1.originalOwner = playerID1;

        playerDummy2 = Instantiate(dummyPlayerPrefab).GetComponent<DummyObject>();
        playerDummy2.originalOwner = playerID2;

        playerDummy3 = Instantiate(dummyPlayerPrefab).GetComponent<DummyObject>();
        playerDummy3.originalOwner = playerID3;

        panelDummy1 = Instantiate(dummyPanelPrefab).GetComponent<DummyObject>();
        panelDummy1.originalOwner = playerID1;

        panelDummy2 = Instantiate(dummyPanelPrefab).GetComponent<DummyObject>();
        panelDummy2.originalOwner = playerID2;

        panelDummy3 = Instantiate(dummyPanelPrefab).GetComponent<DummyObject>();
        panelDummy3.originalOwner = playerID3;

        visualisationDummies = new List<DummyObject>();

        StreamWriter streamWriter1 = new StreamWriter(string.Format("C:\\Users\\blee33\\Desktop\\q{1}_p{0}_raycast.txt", playerID1, question), true);
        StreamWriter streamWriter2 = new StreamWriter(string.Format("C:\\Users\\blee33\\Desktop\\q{1}_p{0}_raycast.txt", playerID2, question), true);
        StreamWriter streamWriter3 = new StreamWriter(string.Format("C:\\Users\\blee33\\Desktop\\q{1}_p{0}_raycast.txt", playerID3, question), true);

        streamWriter1.WriteLine("Timestamp\tGazeObject\tGazeObjectOwner\tLeftPointObject\tLeftPointObjectOwner\tRightPointObject\tRightPointObjectOwner");
        streamWriter2.WriteLine("Timestamp\tGazeObject\tGazeObjectOwner\tLeftPointObject\tLeftPointObjectOwner\tRightPointObject\tRightPointObjectOwner");
        streamWriter3.WriteLine("Timestamp\tGazeObject\tGazeObjectOwner\tLeftPointObject\tLeftPointObjectOwner\tRightPointObject\tRightPointObjectOwner");

        string[] objectDataLines = objectData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines1 = playerData1.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines2 = playerData2.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines3 = playerData3.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int j = 1;

        for (int i = 1; i < playerDataLines1.Length; i++)
        {
            float currentTime = float.Parse(playerDataLines1[i].Split('\t')[0]);

            int visualisationIndex = 0;
            int panelIndex = 0;

            // Position objects at this timestamp
            while (j < objectDataLines.Length)
            {
                string[] values = objectDataLines[j].Split('\t');

                if (IsEqual(float.Parse(values[0]), currentTime))
                {
                    switch (values[1])
                    {
                        case "Panel":
                            DummyObject panel = null;

                            if (int.Parse(values[2]) == playerID1)
                                panel = panelDummy1;
                            else if (int.Parse(values[2]) == playerID2)
                                panel = panelDummy2;
                            else if (int.Parse(values[2]) == playerID3)
                                panel = panelDummy3;

                            panel.transform.position = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
                            panel.transform.rotation = new Quaternion(float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]), float.Parse(values[9]));

                            break;

                        case "Visualisation":
                            DummyObject vis;

                            if (visualisationIndex < visualisationDummies.Count)
                            {
                                vis = visualisationDummies[visualisationIndex];
                            }
                            else
                            {
                                vis = Instantiate(dummyVisualisationPrefab).GetComponent<DummyObject>();
                                visualisationDummies.Add(vis);
                            }

                            visualisationIndex++;

                            vis.originalOwner = int.Parse(values[2]);

                            vis.transform.position = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
                            vis.transform.rotation = new Quaternion(float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]), float.Parse(values[9]));
                            vis.transform.localScale = new Vector3(float.Parse(values[10]), float.Parse(values[11]), 0.1f);

                            break;
                    }

                    j++;
                }
                else
                {
                    break;
                }
            }

            // Remove unncessary visualisations
            for (int k = visualisationDummies.Count - 1; k > visualisationIndex; k--)
            {
                DummyObject d = visualisationDummies[k];
                visualisationDummies.Remove(d);
                DestroyImmediate(d.gameObject);
            }

            // Position players at this timestamp
            // Player 1
            string[] playerValues1 = playerDataLines1[i].Split('\t');

            playerDummy1.transform.position = new Vector3(float.Parse(playerValues1[1]), float.Parse(playerValues1[2]), float.Parse(playerValues1[3]));
            playerDummy1.transform.rotation = new Quaternion(float.Parse(playerValues1[4]), float.Parse(playerValues1[5]), float.Parse(playerValues1[6]), float.Parse(playerValues1[7]));

            // Player 2
            string[] playerValues2 = playerDataLines2[i].Split('\t');

            playerDummy2.transform.position = new Vector3(float.Parse(playerValues2[1]), float.Parse(playerValues2[2]), float.Parse(playerValues2[3]));
            playerDummy2.transform.rotation = new Quaternion(float.Parse(playerValues2[4]), float.Parse(playerValues2[5]), float.Parse(playerValues2[6]), float.Parse(playerValues2[7]));

            // Player 3
            string[] playerValues3 = playerDataLines3[i].Split('\t');

            playerDummy3.transform.position = new Vector3(float.Parse(playerValues3[1]), float.Parse(playerValues3[2]), float.Parse(playerValues3[3]));
            playerDummy3.transform.rotation = new Quaternion(float.Parse(playerValues3[4]), float.Parse(playerValues3[5]), float.Parse(playerValues3[6]), float.Parse(playerValues3[7]));

            // Perform raycasts
            // Gaze
            RaycastedObject p1 = DoRaycast(playerDummy1.transform.position, playerDummy1.transform.forward, playerID1, true);
            RaycastedObject p2 = DoRaycast(playerDummy2.transform.position, playerDummy2.transform.forward, playerID2, true);
            RaycastedObject p3 = DoRaycast(playerDummy3.transform.position, playerDummy3.transform.forward, playerID3, true);

            // Left point
            RaycastedObject lp1 = DoRaycast(new Vector3(float.Parse(playerValues1[8]), float.Parse(playerValues1[9]), float.Parse(playerValues1[10])),
                new Quaternion(float.Parse(playerValues1[11]), float.Parse(playerValues1[12]), float.Parse(playerValues1[13]), float.Parse(playerValues1[14])) * Vector3.forward,
                playerID1
                );
            RaycastedObject lp2 = DoRaycast(new Vector3(float.Parse(playerValues2[8]), float.Parse(playerValues2[9]), float.Parse(playerValues2[10])),
                new Quaternion(float.Parse(playerValues2[11]), float.Parse(playerValues2[12]), float.Parse(playerValues2[13]), float.Parse(playerValues2[14])) * Vector3.forward,
                playerID2
            );
            RaycastedObject lp3 = DoRaycast(new Vector3(float.Parse(playerValues3[8]), float.Parse(playerValues3[9]), float.Parse(playerValues3[10])),
                new Quaternion(float.Parse(playerValues3[11]), float.Parse(playerValues3[12]), float.Parse(playerValues3[13]), float.Parse(playerValues3[14])) * Vector3.forward,
                playerID3
            );

            // Right point
            RaycastedObject rp1 = DoRaycast(new Vector3(float.Parse(playerValues1[19]), float.Parse(playerValues1[20]), float.Parse(playerValues1[21])),
                new Quaternion(float.Parse(playerValues1[22]), float.Parse(playerValues1[23]), float.Parse(playerValues1[24]), float.Parse(playerValues1[25])) * Vector3.forward,
                playerID1
            );
            RaycastedObject rp2 = DoRaycast(new Vector3(float.Parse(playerValues2[19]), float.Parse(playerValues2[20]), float.Parse(playerValues2[21])),
                new Quaternion(float.Parse(playerValues2[22]), float.Parse(playerValues2[23]), float.Parse(playerValues2[24]), float.Parse(playerValues2[25])) * Vector3.forward,
                playerID2
            );
            RaycastedObject rp3 = DoRaycast(new Vector3(float.Parse(playerValues3[19]), float.Parse(playerValues3[20]), float.Parse(playerValues3[21])),
                new Quaternion(float.Parse(playerValues3[22]), float.Parse(playerValues3[23]), float.Parse(playerValues3[24]), float.Parse(playerValues3[25])) * Vector3.forward,
                playerID3
            );

            // Write to file
            streamWriter1.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", playerValues1[0], p1.type, p1.originalOwner, lp1.type, lp1.originalOwner, rp1.type, rp1.originalOwner);

            streamWriter2.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", playerValues2[0], p2.type, p2.originalOwner, lp2.type, lp2.originalOwner, rp2.type, rp2.originalOwner);

            streamWriter3.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", playerValues3[0], p3.type, p3.originalOwner, lp3.type, lp3.originalOwner, rp3.type, rp3.originalOwner);
            
            yield return new WaitForEndOfFrame();
        }

        streamWriter1.Close();
        streamWriter2.Close();
        streamWriter3.Close();
    }

    private void ReplayAndRaycast()
    {
        playerDummy1 = Instantiate(dummyPlayerPrefab).GetComponent<DummyObject>();
        playerDummy1.originalOwner = playerID1;

        playerDummy2 = Instantiate(dummyPlayerPrefab).GetComponent<DummyObject>();
        playerDummy2.originalOwner = playerID2;

        playerDummy3 = Instantiate(dummyPlayerPrefab).GetComponent<DummyObject>();
        playerDummy3.originalOwner = playerID3;

        panelDummy1 = Instantiate(dummyPanelPrefab).GetComponent<DummyObject>();
        panelDummy1.originalOwner = playerID1;

        panelDummy2 = Instantiate(dummyPanelPrefab).GetComponent<DummyObject>();
        panelDummy2.originalOwner = playerID2;

        panelDummy3 = Instantiate(dummyPanelPrefab).GetComponent<DummyObject>();
        panelDummy3.originalOwner = playerID3;

        visualisationDummies = new List<DummyObject>();
        
        StreamWriter streamWriter1 = new StreamWriter(string.Format("C:\\Users\\blee33\\Desktop\\q{1}_p{0}_raycast.txt", playerID1, question), true);
        StreamWriter streamWriter2 = new StreamWriter(string.Format("C:\\Users\\blee33\\Desktop\\q{1}_p{0}_raycast.txt", playerID2, question), true);
        StreamWriter streamWriter3 = new StreamWriter(string.Format("C:\\Users\\blee33\\Desktop\\q{1}_p{0}_raycast.txt", playerID3, question), true);

        streamWriter1.WriteLine("Timestamp\tGazeObject\tGazeObjectOwner\tLeftPointObject\tLeftPointObjectOwner\tRightPointObject\tRightPointObjectOwner");
        streamWriter2.WriteLine("Timestamp\tGazeObject\tGazeObjectOwner\tLeftPointObject\tLeftPointObjectOwner\tRightPointObject\tRightPointObjectOwner");
        streamWriter3.WriteLine("Timestamp\tGazeObject\tGazeObjectOwner\tLeftPointObject\tLeftPointObjectOwner\tRightPointObject\tRightPointObjectOwner");

        string[] objectDataLines = objectData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines1 = playerData1.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines2 = playerData2.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines3 = playerData3.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int j = 1;

        for (int i = 1; i < playerDataLines1.Length; i++)
        {
            float currentTime = float.Parse(playerDataLines1[i].Split('\t')[0]);

            int visualisationIndex = 0;
            int panelIndex = 0;

            // Position objects at this timestamp
            while (j < objectDataLines.Length)
            {
                string[] values = objectDataLines[j].Split('\t');

                if (IsEqual(float.Parse(values[0]), currentTime))
                {
                    switch (values[1])
                    {
                        case "Panel":
                            DummyObject panel = null;

                            if (int.Parse(values[2]) == playerID1)
                                panel = panelDummy1;
                            else if (int.Parse(values[2]) == playerID2)
                                panel = panelDummy2;
                            else if (int.Parse(values[2]) == playerID3)
                                panel = panelDummy3;

                            panel.transform.position = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
                            panel.transform.rotation = new Quaternion(float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]), float.Parse(values[9]));

                            break;

                        case "Visualisation":
                            DummyObject vis;

                            if (visualisationIndex < visualisationDummies.Count)
                            {
                                vis = visualisationDummies[visualisationIndex];
                            }
                            else
                            {
                                vis = Instantiate(dummyVisualisationPrefab).GetComponent<DummyObject>();
                                visualisationDummies.Add(vis);
                            }

                            visualisationIndex++;

                            vis.originalOwner = int.Parse(values[2]);

                            vis.transform.position = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
                            vis.transform.rotation = new Quaternion(float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]), float.Parse(values[9]));
                            vis.transform.localScale = new Vector3(float.Parse(values[10]), float.Parse(values[11]), 0.1f);

                            break;
                    }

                    j++;
                }
                else
                {
                    break;
                }
            }

            // Remove unncessary visualisations
            for (int k = visualisationDummies.Count - 1; k > visualisationIndex; k--)
            {
                DummyObject d = visualisationDummies[k];
                visualisationDummies.Remove(d);
                DestroyImmediate(d.gameObject);
            }

            // Position players at this timestamp
            // Player 1
            string[] playerValues1 = playerDataLines1[i].Split('\t');

            playerDummy1.transform.position = new Vector3(float.Parse(playerValues1[1]), float.Parse(playerValues1[2]), float.Parse(playerValues1[3]));
            playerDummy1.transform.rotation = new Quaternion(float.Parse(playerValues1[4]), float.Parse(playerValues1[5]), float.Parse(playerValues1[6]), float.Parse(playerValues1[7]));

            // Player 2
            string[] playerValues2 = playerDataLines2[i].Split('\t');

            playerDummy2.transform.position = new Vector3(float.Parse(playerValues2[1]), float.Parse(playerValues2[2]), float.Parse(playerValues2[3]));
            playerDummy2.transform.rotation = new Quaternion(float.Parse(playerValues2[4]), float.Parse(playerValues2[5]), float.Parse(playerValues2[6]), float.Parse(playerValues2[7]));

            // Player 3
            string[] playerValues3 = playerDataLines3[i].Split('\t');

            playerDummy3.transform.position = new Vector3(float.Parse(playerValues3[1]), float.Parse(playerValues3[2]), float.Parse(playerValues3[3]));
            playerDummy3.transform.rotation = new Quaternion(float.Parse(playerValues3[4]), float.Parse(playerValues3[5]), float.Parse(playerValues3[6]), float.Parse(playerValues3[7]));

            // Perform raycasts
            // Gaze
            RaycastedObject p1 = DoRaycast(playerDummy1.transform.position, playerDummy1.transform.forward, playerID1, true);
            RaycastedObject p2 = DoRaycast(playerDummy2.transform.position, playerDummy2.transform.forward, playerID2, true);
            RaycastedObject p3 = DoRaycast(playerDummy3.transform.position, playerDummy3.transform.forward, playerID3, true);
            
            // Left point
            RaycastedObject lp1 = DoRaycast(new Vector3(float.Parse(playerValues1[8]), float.Parse(playerValues1[9]), float.Parse(playerValues1[10])),
                new Quaternion(float.Parse(playerValues1[11]), float.Parse(playerValues1[12]), float.Parse(playerValues1[13]), float.Parse(playerValues1[14])) * Vector3.forward,
                playerID1
                );
            RaycastedObject lp2 = DoRaycast(new Vector3(float.Parse(playerValues2[8]), float.Parse(playerValues2[9]), float.Parse(playerValues2[10])),
                new Quaternion(float.Parse(playerValues2[11]), float.Parse(playerValues2[12]), float.Parse(playerValues2[13]), float.Parse(playerValues2[14])) * Vector3.forward,
                playerID2
            );
            RaycastedObject lp3 = DoRaycast(new Vector3(float.Parse(playerValues3[8]), float.Parse(playerValues3[9]), float.Parse(playerValues3[10])),
                new Quaternion(float.Parse(playerValues3[11]), float.Parse(playerValues3[12]), float.Parse(playerValues3[13]), float.Parse(playerValues3[14])) * Vector3.forward,
                playerID3
            );

            // Right point
            RaycastedObject rp1 = DoRaycast(new Vector3(float.Parse(playerValues1[19]), float.Parse(playerValues1[20]), float.Parse(playerValues1[21])),
                new Quaternion(float.Parse(playerValues1[22]), float.Parse(playerValues1[23]), float.Parse(playerValues1[24]), float.Parse(playerValues1[25])) * Vector3.forward,
                playerID1
            );
            RaycastedObject rp2 = DoRaycast(new Vector3(float.Parse(playerValues2[19]), float.Parse(playerValues2[20]), float.Parse(playerValues2[21])),
                new Quaternion(float.Parse(playerValues2[22]), float.Parse(playerValues2[23]), float.Parse(playerValues2[24]), float.Parse(playerValues2[25])) * Vector3.forward,
                playerID2
            );
            RaycastedObject rp3 = DoRaycast(new Vector3(float.Parse(playerValues3[19]), float.Parse(playerValues3[20]), float.Parse(playerValues3[21])),
                new Quaternion(float.Parse(playerValues3[22]), float.Parse(playerValues3[23]), float.Parse(playerValues3[24]), float.Parse(playerValues3[25])) * Vector3.forward,
                playerID3
            );
            
            // Write to file
            streamWriter1.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", playerValues1[0], p1.type, p1.originalOwner, lp1.type, lp1.originalOwner, rp1.type, rp1.originalOwner);
            
            streamWriter2.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", playerValues2[0], p2.type, p2.originalOwner, lp2.type, lp2.originalOwner, rp2.type, rp2.originalOwner);
            
            streamWriter3.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", playerValues3[0], p3.type, p3.originalOwner, lp3.type, lp3.originalOwner, rp3.type, rp3.originalOwner);
        }

        streamWriter1.Close();
        streamWriter2.Close();
        streamWriter3.Close();
    }

    private bool IsEqual(float val1, float val2)
    {
        return Mathf.Abs(val1 - val2) < 0.03f;
    }

    private RaycastedObject DoRaycast(Vector3 origin, Vector3 forward, int player, bool isHead = false)
    {
        RaycastHit hit;

        if (isHead)
            origin = origin + forward * 0.2f;

        if (Physics.Raycast(origin + forward * 0.2f, forward, out hit))
        {
            DummyObject dummy = hit.collider.transform.root.GetComponent<DummyObject>();

            if (dummy != null)
            {
                return new RaycastedObject()
                {
                    type = dummy.tag.Replace("Dummy", "").Replace("Avatar", "Player"),
                    originalOwner = dummy.originalOwner.ToString()
                };
            }

        }

        return new RaycastedObject()
        {
            type = "",
            originalOwner =  ""
        };
    }

    private struct RaycastedObject
    {
        public string type;
        public string originalOwner;
    }
}
