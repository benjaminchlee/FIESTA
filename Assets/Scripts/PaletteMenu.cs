using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaletteMenu : Menu
{
    [SerializeField]
    private int maxNumberOfDistinctValues = 10;

    protected override List<string> GetAttributesList()
    {
        List<string> dimensions = new List<string>();
        for (int i = 0; i < dataSource.DimensionCount; ++i)
        {
            if (dataSource[i].Data.Distinct().Count() <= maxNumberOfDistinctValues)
                dimensions.Add(dataSource[i].Identifier);
        }
        return dimensions;
    }
}
