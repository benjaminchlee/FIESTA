using System.Collections;
using System.Collections.Generic;
using System.Text;
using IATK;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class NetworkedDetailsOnDemandLabel : MonoBehaviourPunCallbacks {

    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private TextMeshPro frontText;
    [SerializeField]
    private TextMeshPro backText;
    [SerializeField]
    private LineRenderer leaderLineRenderer;

    private CSVDataSource dataSource;

    private void Awake()
    {
        if (dataSource == null)
            dataSource = ChartManager.Instance.DataSource;
    }

    public void ToggleState(bool value)
    {
        photonView.RPC("PropagateToggleState", RpcTarget.All, value);
    }

    [PunRPC]
    private void PropagateToggleState(bool value)
    {
        panel.SetActive(value);
        frontText.gameObject.SetActive(value);
        backText.gameObject.SetActive(value);
        leaderLineRenderer.enabled = value;
    }

    public void SetLinePosition(Vector3 pos)
    {
        photonView.RPC("PropagateSetLinePosition", RpcTarget.All, pos);
    }

    [PunRPC]
    private void PropagateSetLinePosition(Vector3 pos)
    {
        leaderLineRenderer.SetPositions(new [] { transform.TransformPoint(Vector3.zero), pos });
    }

    public void SetText(List<int> indices, Visualisation visualisation)
    {
        int viewID = visualisation.GetComponentInParent<PhotonView>().ViewID;
        photonView.RPC("PropagateSetText", RpcTarget.All, indices.ToArray(), viewID);
    }

    [PunRPC]
    private void PropagateSetText(int[] indices, int viewID) 
    {
        // Get the visualisation again
        Visualisation visualisation = PhotonView.Find(viewID).GetComponentInChildren<Visualisation>();

        StringBuilder stringBuilder = new StringBuilder();
        
        stringBuilder.AppendFormat("<b>{0} (x):</b> {1}\n", visualisation.xDimension.Attribute, dataSource.getOriginalValue(indices[0], visualisation.xDimension.Attribute));
        stringBuilder.AppendFormat("<b>{0} (y):</b> {1}\n", visualisation.yDimension.Attribute, dataSource.getOriginalValue(indices[0], visualisation.yDimension.Attribute));
        if (visualisation.zDimension.Attribute != "Undefined") stringBuilder.AppendFormat("<b>{0} (z):</b> {1}\n", visualisation.zDimension.Attribute, dataSource.getOriginalValue(indices[0], visualisation.zDimension.Attribute));

        stringBuilder.Append("--------\n");

        if (indices.Length > 1)
        {
            stringBuilder.AppendFormat("<i>{0} stacked points</i>", indices.Length);
        }
        else
        {
            for (int i = 0; i < dataSource.DimensionCount; i++)
            {
                string dimension = dataSource[i].Identifier;
                object value = dataSource.getOriginalValue(indices[0], dimension);
                if (dataSource[dimension].MetaData.type == DataType.Float && value.ToString().Length > 4)
                {
                    stringBuilder.AppendFormat("<b>{0}:</b> {1}\n", dimension, ((float)value).ToString("#,##0.00"));
                }
                else
                {
                    stringBuilder.AppendFormat("<b>{0}:</b> {1}\n", dimension, value);
                }
            }
        }

        frontText.text = stringBuilder.ToString();
        frontText.ForceMeshUpdate();

        // Resize the panel based on the text
        Vector2 scale = frontText.GetRenderedValues();
        scale.x = scale.x / 2 + 0.01f;
        scale.y = scale.y / 2 + 0.01f;
        panel.transform.localScale = new Vector3(scale.x, scale.y, 0.01f);

        panel.transform.localPosition = new Vector3(scale.x / 2 + 0.01f, scale.y / 2 + 0.01f, 0);
        frontText.transform.localPosition = new Vector3(0.01f, scale.y + 0.01f, -0.008f);

        // Set the text and position the back
        backText.text = stringBuilder.ToString();
        backText.ForceMeshUpdate();
        backText.transform.localPosition = new Vector3(scale.x + 0.01f, scale.y + 0.01f, 0.008f);
    }
}
