using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaletteMenu : Menu
{
    [SerializeField]
    private int maxNumberOfDistinctValues = 10;
    [SerializeField]
    private ColorPaletteBinderMenu paletteBinderMenu;  // lazy, required to update palette

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

    public override void ChartTransferred(Chart chart)
    {
        base.ChartTransferred(chart);

        if (chart.ColorPaletteDimension != "Undefined")
        {
            paletteBinderMenu.CreateColorButtonsFromPalette(chart.ColorPaletteDimension, chart.ColorPalette);
        }
    }
}
