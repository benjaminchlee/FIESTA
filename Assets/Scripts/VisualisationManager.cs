using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

public class VisualisationManager : MonoBehaviour {

    public CSVDataSource dataSource;
    private List<VisualisationInteractor> visualisations;

    private void Start()
    {
        visualisations = new List<VisualisationInteractor>();
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
        VisualisationInteractor interactor = vis.AddComponent<VisualisationInteractor>();
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
        VisualisationInteractor interactor = vis.AddComponent<VisualisationInteractor>();
        visualisations.Add(interactor);

        interactor.Initialise(dataSource);
        interactor.SetVisualisationType(AbstractVisualisation.VisualisationTypes.SCATTERPLOT);
        interactor.SetGeometry(AbstractVisualisation.GeometryType.Points);
        interactor.SetXDimension("mpg");
        interactor.SetZDimension("cylinders");
    }
}
