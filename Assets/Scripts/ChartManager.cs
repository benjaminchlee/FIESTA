using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

public class ChartManager : MonoBehaviour {

    public CSVDataSource dataSource;
    private List<Chart> visualisations;

    private void Start()
    {
        visualisations = new List<Chart>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            AddVisualisation();
        }
        else if (Input.GetKeyDown("s"))
        {
            asd();
        }
    }

    private void AddVisualisation()
    {
        GameObject vis = new GameObject();
        Chart interactor = vis.AddComponent<Chart>();
        visualisations.Add(interactor);

        interactor.Initialise(dataSource);
        interactor.SetVisualisationType(AbstractVisualisation.VisualisationTypes.SCATTERPLOT);
        interactor.SetGeometry(AbstractVisualisation.GeometryType.Points);
        interactor.SetXDimension("mpg");
        interactor.SetYDimension("cylinders");
    }

    private void asd()
    {
        GameObject vis = new GameObject();
        Chart interactor = vis.AddComponent<Chart>();
        visualisations.Add(interactor);

        interactor.Initialise(dataSource);
        interactor.SetVisualisationType(AbstractVisualisation.VisualisationTypes.SCATTERPLOT);
        interactor.SetGeometry(AbstractVisualisation.GeometryType.Points);
        interactor.SetXDimension("mpg");
        interactor.SetZDimension("cylinders");
    }
}
