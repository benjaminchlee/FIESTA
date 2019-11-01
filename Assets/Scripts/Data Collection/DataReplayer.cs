using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReplayer : MonoBehaviour
{
    private enum Mode
    {
        REPLAY,
        SCREENSHOT
    }

    [Header("Prefabs")]
    public GameObject replayPlayerPrefab;
    public GameObject replayLeftHandPrefab;
    public GameObject replayRightHandPrefab;
    public GameObject replayVisualisation2DPrefab;
    public GameObject replayVisualisation3DPrefab;
    public GameObject replayPanelPrefab;
    public GameObject replayMarkerPrefab;
    public GameObject replayEraserPrefab;
    public GameObject replayLinePrefab;

    [Header("General Settings")]
    [Range(1, 5)]
    public int groupID = 1;
    [Range(1, 4)]
    public int questionID = 1;
    public bool useDynamicOwnershipColors = false;
    public Color player1Color = Color.red;
    public Color player2Color = Color.blue;
    public Color player3Color = Color.green;
    public List<TextAsset> playerDataFiles;
    public List<TextAsset> objectDataFiles;
    public List<TextAsset> annotationDataFiles;

    [Header("Replay Settings")]
    public int replayInterval = 100;

    [Header("Screenshot Settings")]
    public string outputFolder;
    public string suffix;
    public int screenshotInterval = 100;
    public bool showPanels = true;
    public bool showPlayers = true;
    public bool showVisualisations = true;
    public bool showMarkers = true;
    public bool showErasers = true;
    public bool showAnnotations = true;
    public bool showSpectator = true;
    public bool use2D3DVisualisationColors = false;

    private Mode mode;
    private bool isRunnning;
    private Coroutine replayCoroutine;

    private void Awake()
    {
        UnityEngine.XR.XRSettings.enabled = false;
    }

    public void StartReplay()
    {
        if (!isRunnning)
        {
            mode = Mode.REPLAY;
            replayCoroutine = StartCoroutine(LiveReplay());
        }
    }

    public void ClearReplay()
    {
        StopCoroutine(replayCoroutine);

        ClearScene();

        isRunnning = false;
    }

    public void StartScreenshot()
    {
        if (!isRunnning)
        {
            mode = Mode.SCREENSHOT;
            StartCoroutine(ReplayAndScreenshot());
        }
    }
    
    private IEnumerator LiveReplay()
    {
        isRunnning = true;

        List<GameObject> players = new List<GameObject>();
        List<GameObject> leftHands = new List<GameObject>();
        List<GameObject> rightHands = new List<GameObject>();
        List<GameObject> panels = new List<GameObject>();
        List<GameObject> markers = new List<GameObject>();
        List<GameObject> erasers = new List<GameObject>();
        List<LineRenderer> lines = new List<LineRenderer>();
        Dictionary<string, Chart> chartsDictionary = new Dictionary<string, Chart>();
        Dictionary<string, bool> visitedCharts = new Dictionary<string, bool>();

        // Instantiate and pool objects
        for (int i = 0; i < 3; i++)
        {
            GameObject player = Instantiate(replayPlayerPrefab);
            ColorObject(player, i);
            players.Add(player);

            GameObject leftHand = Instantiate(replayLeftHandPrefab);
            ColorObject(leftHand, i);
            leftHands.Add(leftHand);

            GameObject rightHand = Instantiate(replayRightHandPrefab);
            ColorObject(rightHand, i);
            rightHands.Add(rightHand);

            GameObject panel = Instantiate(replayPanelPrefab);
            //ColorObject(panel, i);
            panels.Add(panel);

            GameObject marker = Instantiate(replayMarkerPrefab);
            //ColorObject(marker, i);
            markers.Add(marker);

            GameObject eraser = Instantiate(replayEraserPrefab);
            //ColorObject(eraser, i);
            erasers.Add(eraser);
        }
        GameObject spectator = Instantiate(replayPlayerPrefab);
        //ColorObject(spectator, 3);
        players.Add(spectator);


        string[] playerDataLines1 = playerDataFiles[(groupID - 1) * 12 + (questionID - 1) * 3].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines2 = playerDataFiles[(groupID - 1) * 12 + (questionID - 1) * 3 + 1].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] playerDataLines3 = playerDataFiles[(groupID - 1) * 12 + (questionID - 1) * 3 + 2].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] objectDataLines = objectDataFiles[(groupID - 1) * 4 + questionID - 1].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] annotationDataLines = annotationDataFiles[(groupID - 1) * 4 + questionID - 1].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int j = 1;
        int l = 1;

        for (int i = 1; i < playerDataLines1.Length; i += replayInterval)
        {
            float currentTime = float.Parse(playerDataLines1[i].Split('\t')[0]);
            
            int lineCounter = 0;
            
            foreach (var key in chartsDictionary.Keys)
            {
                visitedCharts[key] = false;
            }

            // Position objects at this timestamp
            while (j < objectDataLines.Length)
            {
                string[] values = objectDataLines[j].Split('\t');
                float thisTime = float.Parse(values[0]);

                if (IsEqual(thisTime, currentTime, 0.04f))
                {
                    string type = values[1];
                    int originalOwner = GetID(values[2]);
                    int currentOwner = GetID(values[3]);

                    Vector3 pos = new Vector3(float.Parse(values[4]), float.Parse(values[5]), float.Parse(values[6]));
                    Quaternion rot = new Quaternion(float.Parse(values[7]), float.Parse(values[8]), float.Parse(values[9]), float.Parse(values[10]));
                    
                    switch (type)
                    {
                        case "Panel":
                            GameObject panel = panels[originalOwner];
                            ColorObject(panel, originalOwner, currentOwner);
                            panel.transform.position = pos;
                            panel.transform.rotation = rot;
                            break;

                        case "Marker":
                            GameObject marker = markers[originalOwner];
                            ColorObject(marker, originalOwner, currentOwner);
                            marker.transform.position = pos;
                            marker.transform.rotation = rot;
                            break;

                        case "Eraser":
                            GameObject eraser = erasers[originalOwner];
                            ColorObject(eraser, originalOwner, currentOwner);
                            eraser.transform.position = pos;
                            eraser.transform.rotation = rot;
                            break;

                        case "Observer":
                            ColorObject(spectator, originalOwner, currentOwner);
                            spectator.transform.position = pos;
                            spectator.transform.rotation = rot;
                            break;

                        case "Visualisation":
                            string id = values[14];
                            // Make sure vis has an ID
                            if (id == "")
                                break;
                            
                            Chart chart;
                            if (chartsDictionary.ContainsKey(id))
                            {
                                chart = chartsDictionary[id];
                            }
                            else
                            {
                                chart = ChartManager.Instance.CreateVisualisation("");
                                chartsDictionary[id] = chart;
                            }

                            visitedCharts[id] = true;

                            float width = float.Parse(values[11]);
                            float height = float.Parse(values[12]);
                            float depth = float.Parse(values[13]);
                            string xDimension = values[15];
                            string yDimension = values[16];
                            string zDimension = values[17];
                            float size = float.Parse(values[18]);
                            string sizeDimension = values[19];
                            string colorTmp = values[20];
                            string colorDimension = values[21];
                            string facetDimension = values[22];
                            int facetSize = int.Parse(values[23]);
                            string xNormaliserTmp = values[24];
                            string yNormaliserTmp = values[25];
                            string zNormaliserTmp = values[26];

                            string[] colorSplit = colorTmp.Replace("RGBA(", "").Replace(")", "").Split(',');
                            Color color = new Color(float.Parse(colorSplit[0].Trim()), float.Parse(colorSplit[1].Trim()), float.Parse(colorSplit[2].Trim()), float.Parse(colorSplit[3].Trim()));

                            string[] xNormSplit = xNormaliserTmp.Replace("(", "").Replace(")", "").Split(',');
                            Vector2 xNormaliser = new Vector2(float.Parse(xNormSplit[0].Trim()), float.Parse(xNormSplit[1].Trim()));
                            string[] yNormSplit = yNormaliserTmp.Replace("(", "").Replace(")", "").Split(',');
                            Vector2 yNormaliser = new Vector2(float.Parse(yNormSplit[0].Trim()), float.Parse(yNormSplit[1].Trim()));
                            string[] zNormSplit = zNormaliserTmp.Replace("(", "").Replace(")", "").Split(',');
                            Vector2 zNormaliser = new Vector2(float.Parse(zNormSplit[0].Trim()), float.Parse(zNormSplit[1].Trim()));

                            chart.VisualisationType = IATK.AbstractVisualisation.VisualisationTypes.SCATTERPLOT;
                            chart.XDimension = xDimension;
                            chart.YDimension = yDimension;
                            chart.ZDimension = zDimension;
                            chart.XNormaliser = xNormaliser;
                            chart.YNormaliser = yNormaliser;
                            chart.ZNormaliser = zNormaliser;
                            chart.Size = size;
                            chart.SizeDimension = sizeDimension;
                            chart.Color = color;
                            chart.ColorDimension = colorDimension;
                            chart.FacetDimension = facetDimension;
                            chart.FacetSize = facetSize;
                            chart.Width = width;
                            chart.Height = height;
                            chart.Depth = depth;
                            chart.GeometryType = IATK.AbstractVisualisation.GeometryType.Points;

                            // Create placeholder gradient
                            Gradient grad = new Gradient();
                            grad.colorKeys = new GradientColorKey[] {
                                new GradientColorKey() { color = Color.white, time = 0.0f },
                                new GradientColorKey() { color = Color.red, time = 1.0f }
                            };
                            grad.alphaKeys = new GradientAlphaKey[] {
                                new GradientAlphaKey() { alpha = 1.0f, time = 0.0f },
                                new GradientAlphaKey() { alpha = 1.0f, time = 1.0f }
                            };
                            chart.Gradient = grad;

                            ColorObject(chart.gameObject, originalOwner, currentOwner);
                            chart.transform.position = pos;
                            chart.transform.rotation = rot;

                            break;
                    }

                    j++;
                }
                else
                {
                    if (thisTime > currentTime)
                        break;
                    else
                        j++;
                }
            }

            // Remove unncessary visualisations
            List<string> idsToRemove = new List<string>();
            foreach (var key in chartsDictionary.Keys)
            {
                if (visitedCharts[key] == false)
                {
                    idsToRemove.Add(key);
                }
            }
            // If suddenly all visualisations are to be removed, skip over it and don't delete any since its probably a bug
            if (idsToRemove.Count < chartsDictionary.Count)
            {
                foreach (var id in idsToRemove)
                {
                    ChartManager.Instance.RemoveVisualisation(chartsDictionary[id]);
                    chartsDictionary.Remove(id);
                    visitedCharts.Remove(id);
                }
            }

            // Position players at this timestamp
            // Player 1
            string[] playerValues1 = playerDataLines1[i].Split('\t');
            players[0].transform.position = new Vector3(float.Parse(playerValues1[1]), float.Parse(playerValues1[2]), float.Parse(playerValues1[3]));
            players[0].transform.rotation = new Quaternion(float.Parse(playerValues1[4]), float.Parse(playerValues1[5]), float.Parse(playerValues1[6]), float.Parse(playerValues1[7]));
            leftHands[0].transform.position = new Vector3(float.Parse(playerValues1[8]), float.Parse(playerValues1[9]), float.Parse(playerValues1[10]));
            leftHands[0].transform.rotation = new Quaternion(float.Parse(playerValues1[11]), float.Parse(playerValues1[12]), float.Parse(playerValues1[13]), float.Parse(playerValues1[14]));
            rightHands[0].transform.position = new Vector3(float.Parse(playerValues1[19]), float.Parse(playerValues1[20]), float.Parse(playerValues1[21]));
            rightHands[0].transform.rotation = new Quaternion(float.Parse(playerValues1[22]), float.Parse(playerValues1[23]), float.Parse(playerValues1[24]), float.Parse(playerValues1[25]));

            // Player 2
            string[] playerValues2 = playerDataLines2[i].Split('\t');
            players[1].transform.position = new Vector3(float.Parse(playerValues2[1]), float.Parse(playerValues2[2]), float.Parse(playerValues2[3]));
            players[1].transform.rotation = new Quaternion(float.Parse(playerValues2[4]), float.Parse(playerValues2[5]), float.Parse(playerValues2[6]), float.Parse(playerValues2[7]));
            leftHands[1].transform.position = new Vector3(float.Parse(playerValues2[8]), float.Parse(playerValues2[9]), float.Parse(playerValues2[10]));
            leftHands[1].transform.rotation = new Quaternion(float.Parse(playerValues2[11]), float.Parse(playerValues2[12]), float.Parse(playerValues2[13]), float.Parse(playerValues2[14]));
            rightHands[1].transform.position = new Vector3(float.Parse(playerValues2[19]), float.Parse(playerValues2[20]), float.Parse(playerValues2[21]));
            rightHands[1].transform.rotation = new Quaternion(float.Parse(playerValues2[22]), float.Parse(playerValues2[23]), float.Parse(playerValues2[24]), float.Parse(playerValues2[25]));

            // Player 3
            string[] playerValues3 = playerDataLines3[i].Split('\t');
            players[2].transform.position = new Vector3(float.Parse(playerValues3[1]), float.Parse(playerValues3[2]), float.Parse(playerValues3[3]));
            players[2].transform.rotation = new Quaternion(float.Parse(playerValues3[4]), float.Parse(playerValues3[5]), float.Parse(playerValues3[6]), float.Parse(playerValues3[7]));
            leftHands[2].transform.position = new Vector3(float.Parse(playerValues3[8]), float.Parse(playerValues3[9]), float.Parse(playerValues3[10]));
            leftHands[2].transform.rotation = new Quaternion(float.Parse(playerValues3[11]), float.Parse(playerValues3[12]), float.Parse(playerValues3[13]), float.Parse(playerValues3[14]));
            rightHands[2].transform.position = new Vector3(float.Parse(playerValues3[19]), float.Parse(playerValues3[20]), float.Parse(playerValues3[21]));
            rightHands[2].transform.rotation = new Quaternion(float.Parse(playerValues3[22]), float.Parse(playerValues3[23]), float.Parse(playerValues3[24]), float.Parse(playerValues3[25]));


            // Position annotations at this timestep
            while (l < annotationDataLines.Length)
            {
                string[] values = annotationDataLines[l].Split('\t');
                float thisTime = float.Parse(values[0]);

                if (IsEqual(thisTime, currentTime))
                {
                    int originalOwner = GetID(values[1]);
                    int currentOwner = GetID(values[2]);

                    // Convert string representation of vector3 array into an actual array
                    string positionsTmp = values[3].Replace(")|(", ",").Replace("(", "").Replace(")", ""); // Remove all brackets
                    string[] positionsSplit = positionsTmp.Split(','); // Split to get individual strings of floats

                    int numPositions = positionsSplit.Length / 3;
                    Vector3[] positions = new Vector3[numPositions];
                    int index = 0;
                    for (int m = 0; m < positionsSplit.Length; m += 3)
                    {
                        positions[index] = new Vector3(float.Parse(positionsSplit[m].Trim()),
                            float.Parse(positionsSplit[m + 1].Trim()),
                            float.Parse(positionsSplit[m + 2].Trim()));
                        index++;
                    }

                    // Get line renderer
                    LineRenderer lineRenderer;
                    if (lineCounter < lines.Count)
                    {
                        lineRenderer = lines[lineCounter];
                    }
                    else
                    {
                        lineRenderer = Instantiate(replayLinePrefab).GetComponent<LineRenderer>();
                        lines.Add(lineRenderer);
                    }
                    lineCounter++;

                    lineRenderer.positionCount = numPositions;
                    lineRenderer.SetPositions(positions);
                    ColorObject(lineRenderer.gameObject, originalOwner, currentOwner);

                    l++;
                }
                else
                {
                    if (thisTime > currentTime)
                        break;
                    else
                        l++;
                }
            }

            // Remove unnecessary lines
            for (int k = lines.Count - 1; k > lineCounter; k--)
            {
                LineRenderer lr = lines[k];
                lines.Remove(lr);
                DestroyImmediate(lr.gameObject);
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }

        isRunnning = false;
    }

    private IEnumerator ReplayAndScreenshot()
    {
        isRunnning = true;

        List<GameObject> players = new List<GameObject>();
        List<GameObject> objects = new List<GameObject>();

        for (int group = 0; group < 5; group++)
        {
            for (int question = 0; question < 4; question++)
            {
                string[] playerDataLines1 = playerDataFiles[group * 12 + question * 3].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] playerDataLines2 = playerDataFiles[group * 12 + question * 3 + 1].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] playerDataLines3 = playerDataFiles[group * 12 + question * 3 + 2].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] objectDataLines = objectDataFiles[group * 4 + question].text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                int numLines = Mathf.Min(playerDataLines1.Length, playerDataLines2.Length, playerDataLines3.Length);
                int j = 1;

                // Replay
                for (int i = 1; i < numLines; i += screenshotInterval)
                {
                    string[] playerValues1 = playerDataLines1[i].Split('\t'); ;
                    string[] playerValues2 = playerDataLines2[i].Split('\t'); ;
                    string[] playerValues3 = playerDataLines3[i].Split('\t'); ;

                    if (showPlayers)
                    {
                        // Position players at this timestamp
                        // Player 1
                        GameObject player1 = Instantiate(replayPlayerPrefab);
                        player1.transform.position = new Vector3(float.Parse(playerValues1[1]), float.Parse(playerValues1[2]), float.Parse(playerValues1[3]));
                        player1.transform.rotation = new Quaternion(float.Parse(playerValues1[4]), float.Parse(playerValues1[5]), float.Parse(playerValues1[6]), float.Parse(playerValues1[7]));
                        players.Add(player1);

                        // Player 2
                        GameObject player2 = Instantiate(replayPlayerPrefab);
                        player2.transform.position = new Vector3(float.Parse(playerValues2[1]), float.Parse(playerValues2[2]), float.Parse(playerValues2[3]));
                        player2.transform.rotation = new Quaternion(float.Parse(playerValues2[4]), float.Parse(playerValues2[5]), float.Parse(playerValues2[6]), float.Parse(playerValues2[7]));
                        players.Add(player2);

                        // Player 3
                        GameObject player3 = Instantiate(replayPlayerPrefab);
                        player3.transform.position = new Vector3(float.Parse(playerValues3[1]), float.Parse(playerValues3[2]), float.Parse(playerValues3[3]));
                        player3.transform.rotation = new Quaternion(float.Parse(playerValues3[4]), float.Parse(playerValues3[5]), float.Parse(playerValues3[6]), float.Parse(playerValues3[7]));
                        players.Add(player3);

                        // Color players
                        ColorObject(player1, 0, 0);
                        ColorObject(player2, 1, 1);
                        ColorObject(player3, 2, 2);
                    }

                    float currentTime = float.Parse(playerDataLines1[i].Split('\t')[0]);

                    // Position objects at this timestamp
                    while (j < objectDataLines.Length)
                    {
                        string[] objectValues = objectDataLines[j].Split('\t');

                        if (IsEqual(float.Parse(objectValues[0]), currentTime))
                        {
                            GameObject go = null;
                            Vector3 pos = new Vector3(float.Parse(objectValues[4]), float.Parse(objectValues[5]), float.Parse(objectValues[6]));
                            Quaternion rot = new Quaternion(float.Parse(objectValues[7]), float.Parse(objectValues[8]), float.Parse(objectValues[9]), float.Parse(objectValues[10]));

                            switch (objectValues[1])
                            {
                                case "Panel":
                                    if (showPanels)
                                    {
                                        go = Instantiate(replayPanelPrefab);
                                        go.transform.position = pos;
                                        go.transform.rotation = rot;
                                    }
                                    break;

                                case "Marker":
                                    if (showMarkers)
                                    {
                                        go = Instantiate(replayMarkerPrefab);
                                        go.transform.position = pos;
                                        go.transform.rotation = rot;
                                    }
                                    break;

                                case "Eraser":
                                    if (showErasers)
                                    {
                                        go = Instantiate(replayEraserPrefab);
                                        go.transform.position = pos;
                                        go.transform.rotation = rot;
                                    }
                                    break;

                                case "Observer":
                                    if (question == 3 && showSpectator)
                                    {
                                        go = Instantiate(replayPlayerPrefab);
                                        go.transform.position = pos;
                                        go.transform.rotation = rot;
                                    }
                                    break;

                                case "Visualisation":
                                    if (showVisualisations)
                                    {
                                        // If is 2D
                                        if (objectValues[17] == "Undefined")
                                            go = Instantiate(replayVisualisation2DPrefab);
                                        // If is 3D
                                        else
                                            go = Instantiate(replayVisualisation3DPrefab);

                                        Vector3 scale = new Vector3(float.Parse(objectValues[11]), float.Parse(objectValues[12]), float.Parse(objectValues[13]));
                                        go.transform.position = pos;
                                        go.transform.rotation = rot;
                                        go.transform.localScale = scale;
                                    }
                                    break;
                            }

                            if (go != null)
                            {
                                int originalOwner = GetID(objectValues[2]);
                                int currentOwner = GetID(objectValues[3]);

                                if (objectValues[1] == "Visualisation")
                                {
                                    ColorVisualisationObject(go, objectValues[17] != "Undefined");
                                }
                                else
                                {
                                    ColorObject(go, originalOwner, currentOwner);
                                }

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

                string filename = string.Format("{0}G{1}_Q{2}_Heatmap{3}.png",
                    outputFolder,
                    group + 1,
                    question + 1,
                    suffix
                );

                ScreenCapture.CaptureScreenshot(filename, ScreenCapture.StereoScreenCaptureMode.BothEyes);

                foreach (var go in players)
                {
                    Destroy(go);
                }

                foreach (var go in objects)
                {
                    Destroy(go);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        isRunnning = false;
    }

    private bool IsEqual(float val1, float val2, float delta = 0.03f)
    {
        return Mathf.Abs(val1 - val2) < delta;
    }

    private int GetID(string text)
    {
        int id = int.Parse(text); // 0: player 1 || 1: player 2 || 2: player 3 || 3: spectator
        id = (id == -1) ? 3 : (id - 1) % 3;
        return id;
    }
        
    private void ColorObject(GameObject go, int originalOwnerID, int currentOwnerID = 99)
    {
        if (currentOwnerID == 99) currentOwnerID = originalOwnerID;

        Color col = Color.black;
        int id = useDynamicOwnershipColors ? currentOwnerID : originalOwnerID;

        if (mode == Mode.REPLAY)
        {
            switch (id)
            {
                case 0:
                    col = player1Color;
                    break;

                case 1:
                    col = player2Color;
                    break;

                case 2:
                    col = player3Color;
                    break;

                case 3:
                    col = Color.black;
                    break;
            }
        }
        else if (mode == Mode.SCREENSHOT)
        {
            switch (id)
            {
                case 0:
                    col = new Color(0.843f, 0.188f, 0.153f, 0.075f);
                    break;

                case 1:
                    col = new Color(0.263f, 0.576f, 0.765f, 0.075f);
                    break;

                case 2:
                    col = new Color(0.4f, 0.741f, 0.388f, 0.075f);
                    break;

                case 3:
                    col = new Color(0.9f, 0.9f, 0.9f, 0.075f);
                    break;
            }
        }

        var renderers = go.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (mode == Mode.SCREENSHOT)
            {
                renderer.material.shader = Shader.Find("Unlit/OverdrawShader");
            }
            renderer.material.SetColor("_Color", col);
        }

        // Tag every object so that we can easily clear them up later
        // We do it here since all objects we instantiate flow through this function
        go.tag = "Replay";
    }

    private void ColorVisualisationObject(GameObject vis, bool is3D)
    {
        if (mode == Mode.SCREENSHOT)
        {
            Color col = Color.black;

            if (!is3D)
            {
                col = new Color(99 / 256f, 110 / 256f, 250 / 256f, 0.025f);
            }
            else
            {
                col = new Color(239 / 256f, 85 / 256f, 59 / 256f, 0.025f);
            }

            var renderers = vis.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.material.shader = Shader.Find("Unlit/OverdrawShader");
                renderer.material.SetColor("_Color", col);
            }
        }
    }
    
    private void ClearScene()
    {
        foreach (var go in GameObject.FindGameObjectsWithTag("Replay"))
        {
            Destroy(go);
        }
    }
}
