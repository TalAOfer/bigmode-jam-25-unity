using System.Collections.Generic;
using UnityEngine;

public class PlanetDirectionIndicatorManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float padding = 20f;
    private Camera mainCamera;

    [SerializeField] private List<PlanetDirectionIndicator> directionIndicators;
    private bool isInitialized;

    private void Awake()
    {
        mainCamera = Camera.main;

        for (int i = 0; i < GameManager.Instance.galaxyManager.planets.Count; i++)
        {
            directionIndicators[i].Initialize(GameManager.Instance.galaxyManager.planets[i]);
        }

        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        SharedIndicatorData indicatorData = GetSharedData();

        for (int i = 0; i < GameManager.Instance.galaxyManager.planets.Count; i++)
        {
            bool shouldShowIndicator = !directionIndicators[i].planet.IsVisible;
            directionIndicators[i].gameObject.SetActive(shouldShowIndicator);
            if (shouldShowIndicator)
            {
                directionIndicators[i].UpdateIndicator(indicatorData);
            }
        }
    }

    public SharedIndicatorData GetSharedData()
    {
        return new SharedIndicatorData
        {
            playerScreenPos = mainCamera.WorldToScreenPoint(player.transform.position),
            screenSize = new Vector2(Screen.width, Screen.height),
            padding = padding,
            camera = mainCamera
        };
    }
}
public struct SharedIndicatorData
{
    public Vector2 playerScreenPos;
    public Vector2 screenSize;
    public float padding;
    public Camera camera;
}