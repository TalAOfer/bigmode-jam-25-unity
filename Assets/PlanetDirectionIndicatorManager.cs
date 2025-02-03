using System.Collections.Generic;
using UnityEngine;

public class PlanetDirectionIndicatorManager : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private float maxAlpha = 0.5f;
    [SerializeField] private float showHideDuration = 0.5f;
    [SerializeField] private float distance = 5f;

    [SerializeField] private List<PlanetDirectionIndicator> directionIndicators;
    private bool isInitialized;

    private void Awake()
    {
        for (int i = 0; i < GameManager.Instance.galaxyManager.planets.Count; i++)
        {
            directionIndicators[i].Initialize(GameManager.Instance.galaxyManager.planets[i]);
            Vector3 pos = directionIndicators[i].sr.transform.position;
            pos.y = distance;
            directionIndicators[i].sr.transform.position = pos;
        }

        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        transform.position = player.transform.position;

        for (int i = 0; i < GameManager.Instance.galaxyManager.planets.Count; i++)
        {
            bool shouldShowIndicator = !directionIndicators[i].planet.IsVisible;
            directionIndicators[i].gameObject.SetActive(shouldShowIndicator);
            if (shouldShowIndicator)
            {
                directionIndicators[i].UpdateIndicator();
            }
        }
    }

    public void ShowIndicators()
    {
        for (int i = 0; i < GameManager.Instance.galaxyManager.planets.Count; i++)
        {
            bool shouldShowIndicator = !directionIndicators[i].planet.IsVisible;
            if (shouldShowIndicator)
            {
                directionIndicators[i].ShowIndicator(showHideDuration, maxAlpha);
            }
        }
    }

    public void HideIndicators()
    {
        for (int i = 0; i < GameManager.Instance.galaxyManager.planets.Count; i++)
        {
            directionIndicators[i].HideIndicator(showHideDuration);
        }
    }
}