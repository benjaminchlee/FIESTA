using System.Collections;
using System.Collections.Generic;
using System.Text;
using IATK;
using TMPro;
using UnityEngine;

public class DetailsOnDemandLabel : MonoBehaviour {

    [SerializeField]
    private TextMeshPro textMesh;

    private CSVDataSource dataSource;

    private void Awake()
    {
        if (dataSource == null)
            dataSource = ChartManager.Instance.DataSource;
    }

    public void SetText(List<int> indices, Visualisation visualisation)
    {
        StringBuilder stringBuilder = new StringBuilder();

        if (indices.Count > 1)
            stringBuilder.AppendFormat("<i>{0} stacked points</i>\n\n", indices.Count);

        stringBuilder.AppendFormat("<b>x:</b> {0}\n", dataSource.getOriginalValue(indices[0], visualisation.xDimension.Attribute));
        stringBuilder.AppendFormat("<b>y:</b> {0}", dataSource.getOriginalValue(indices[0], visualisation.yDimension.Attribute));

        textMesh.text = stringBuilder.ToString();
    }
}
