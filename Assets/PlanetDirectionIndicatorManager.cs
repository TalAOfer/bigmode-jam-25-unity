using System.Collections.Generic;
using UnityEngine;

public class PlanetDirectionIndicatorManager : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private IndicatorData data;

    [SerializeField] private GameObject indicatorPrefab;

    private List<PlanetDirectionIndicator> activeIndicators = new();
    
    private bool isInitialized;
    private bool showIndicators;

    private void Awake()
    {
        for (int i = 0; i < GameManager.Instance.galaxyManager.planets.Count; i++)
        {
            Planet currentPlanet = GameManager.Instance.galaxyManager.planets[i];
            if (!currentPlanet.shouldShowIndicator) continue;

            GameObject indicatorGO = Instantiate(indicatorPrefab, transform);
            PlanetDirectionIndicator indicator = indicatorGO.GetComponent<PlanetDirectionIndicator>();

            indicator.Initialize(GameManager.Instance.galaxyManager.planets[i], data.radius);         
            
            activeIndicators.Add(indicator);
        }

        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        transform.position = player.transform.position;

        if (showIndicators)
        {
            UpdateIndicators();
        }
    }

    public void UpdateIndicators()
    {
        foreach(var indicator in activeIndicators)
        {
            indicator.UpdateIndicator();

            bool shouldShowIndicator = !indicator.planet.IsVisible;

            if (shouldShowIndicator)
            {
                indicator.ShowIndicator(data.showFade, data.maxAlpha);
            }

            else
            {
                indicator.HideIndicator(data.hideFade);
            }
        }
    }

    public void ShowIndicators()
    {
        showIndicators = true;
    }

    public void HideIndicators()
    {
        showIndicators = false;

        foreach(var indicator in activeIndicators)
        {
            indicator.HideIndicator(data.hideFade);
        }
    }
}