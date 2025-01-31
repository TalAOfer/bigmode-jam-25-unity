using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Core References")]
    [SerializeField] private Player player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform planetsContainer; // Parent object for planets

    [Header("Solar System Configuration")]
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private PlanetBlueprint sunBlueprint;
    [SerializeField] private PlanetBlueprint planet1Blueprint;
    [SerializeField] private PlanetBlueprint planet2Blueprint;
    [SerializeField] private float planet1DistanceFromSun = 1000f;
    [SerializeField] private float planet2DistanceFromSun = 2000f;

    [Header("Planet Visuals")]
    [SerializeField] private List<Sprite> cloudSprites;
    [SerializeField] private List<Sprite> foregroundDecorations;
    [SerializeField] private List<Sprite> backgroundDecorations;

    [Header("Game Settings")]
    [SerializeField] private float worldBoundary = 16000f;
    [SerializeField] private float colorLerpSpeed = 0.05f;
    [SerializeField] private float skyColorMin = 0f;
    [SerializeField] private float skyColorMax = 0.77f;
    [SerializeField] private float planetVisibilityMultiplier = 1.5f;

    public List<Planet> planets = new List<Planet>();
    private bool[] renderedPlanets;
    private Color32[] currentPalette = new Color32[5];
    private float skyColor;
    private bool isGamePaused;

    #region Unity Lifecycle

    private void Awake()
    {
        SetupSingleton();
        InitializeGameState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (!isGamePaused)
        {
            UpdateGameState();
        }
    }

    private void OnDrawGizmos()
    {
        // Draw world boundaries
        Gizmos.color = Color.red;
        Vector3 center = Vector3.zero;
        Vector3 size = new Vector3(worldBoundary * 2, worldBoundary * 2, 0);
        Gizmos.DrawWireCube(center, size);
    }

    #endregion

    #region Initialization

    private void SetupSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGameState()
    {
        renderedPlanets = new bool[16]; // Max planets
        skyColor = skyColorMax;

        if (planetsContainer == null)
        {
            planetsContainer = new GameObject("Planets Container").transform;
        }

        SetupGame();
    }

    private void SetupGame()
    {
        CreateSolarSystem();
        InitializePlayer();
    }

    private void CreateSolarSystem()
    {
        // Create sun and planets
        Planet sun = CreatePlanet(sunBlueprint);
        Planet planet1 = CreatePlanet(planet1Blueprint);
        Planet planet2 = CreatePlanet(planet2Blueprint);

        // Setup orbits
        sun.AddToOrbit(planet1, planet1DistanceFromSun, 0f);
        sun.AddToOrbit(planet2, planet2DistanceFromSun, 90f);

        // Add decorations
        DecoratePlanet(planet1);
    }

    private Planet CreatePlanet(PlanetBlueprint blueprint)
    {
        GameObject planetObj = Instantiate(planetPrefab, planetsContainer);
        planetObj.name = blueprint.name;

        Planet planet = planetObj.GetComponent<Planet>();
        if (planet == null)
        {
            Debug.LogError($"Planet prefab is missing Planet component: {blueprint.name}");
            return null;
        }

        ConfigurePlanet(planet, blueprint);
        planets.Add(planet);

        return planet;
    }

    private void ConfigurePlanet(Planet planet, PlanetBlueprint blueprint)
    {
        planet.planetName = blueprint.name;
        planet.radius = blueprint.radius;
        planet.gravity = blueprint.gravity;
        planet.palette = new Color32[5];
        System.Array.Copy(blueprint.palette, planet.palette, 5);

        // Set initial transform values
        planet.transform.localPosition = Vector3.zero;
        planet.transform.localRotation = Quaternion.identity;
        planet.transform.localScale = Vector3.one * (blueprint.radius * 2);
    }

    private void DecoratePlanet(Planet planet)
    {
        uint[] fgTypes = new uint[] { 1, 2, 3, 4, 5, 6 };
        uint[] bgTypes = new uint[] { 7, 8, 9, 10, 11 };
        planet.Decorate(fgTypes, bgTypes);
    }

    private void InitializePlayer()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set in GameManager!");
            return;
        }

        player.planetIdx = 0; // Default to first planet
    }

    #endregion

    #region Game State Updates

    private void UpdateGameState()
    {
        UpdatePlanets();
        UpdateVisiblePlanets();
        UpdatePlayerState();
        UpdatePaletteLerp();
    }

    private void UpdatePlanets()
    {
        foreach (Planet planet in planets)
        {
            if (planet != null)
            {
                planet.UpdatePlanet(Time.deltaTime);
            }
        }
    }

    private void UpdateVisiblePlanets()
    {
        if (mainCamera == null) return;

        Vector2 cameraPos = mainCamera.transform.position;
        float viewportHeight = 2f * mainCamera.orthographicSize;
        float viewportWidth = viewportHeight * mainCamera.aspect;
        float viewRadius = Mathf.Max(viewportWidth, viewportHeight) / 2f * Mathf.Sqrt(2);

        for (int i = 0; i < planets.Count; i++)
        {
            if (planets[i] == null) continue;

            Vector2 planetPos = planets[i].position;
            float dist = Vector2.Distance(planetPos, cameraPos);
            float threshold = viewRadius + planets[i].radius;

            renderedPlanets[i] = dist <= threshold;

            // Optionally disable rendering for far planets
            //if (planets[i].gameObject.activeSelf != renderedPlanets[i])
            //{
            //    planets[i].gameObject.SetActive(renderedPlanets[i]);
            //}
        }
    }

    private void UpdatePlayerState()
    {
        if (player == null) return;

        player.GetClosestPlanet(planets);
        WrapPlayerPosition();
    }

    private void WrapPlayerPosition()
    {
        Vector2 newPosition = (Vector2)player.transform.position;
        bool positionChanged = false;

        if (player.transform.position.x < -worldBoundary)
        {
            newPosition.x = worldBoundary - 1000f;
            positionChanged = true;
        }
        else if (player.transform.position.x > worldBoundary)
        {
            newPosition.x = -worldBoundary + 1000f;
            positionChanged = true;
        }

        if (player.transform.position.y < -worldBoundary)
        {
            newPosition.y = worldBoundary - 1000f;
            positionChanged = true;
        }
        else if (player.transform.position.y > worldBoundary)
        {
            newPosition.y = -worldBoundary + 1000f;
            positionChanged = true;
        }

        if (positionChanged)
        {
            player.transform.position = newPosition;
        }
    }

    private void UpdatePaletteLerp()
    {
        if (player.planetIdx >= planets.Count || planets[player.planetIdx] == null) return;

        Planet currentPlanet = planets[player.planetIdx];

        // Update color palette
        for (int i = 0; i < 5; i++)
        {
            currentPalette[i] = Color32.Lerp(currentPalette[i], currentPlanet.palette[i], colorLerpSpeed);
        }

        // Update sky color
        float dist = Vector2.Distance(player.transform.position, currentPlanet.position);
        float t = Mathf.Clamp01((dist - currentPlanet.radius * planetVisibilityMultiplier) /
                               (currentPlanet.radius * planetVisibilityMultiplier));
        skyColor = Mathf.Lerp(skyColorMax, skyColorMin, t);
    }

    #endregion

    #region Public Methods

    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0 : 1;
        // Add pause menu handling here
    }

    public bool IsPlanetVisible(int index)
    {
        return index < renderedPlanets.Length && renderedPlanets[index];
    }

    public Color32[] GetCurrentPalette()
    {
        return currentPalette;
    }

    public float GetSkyColor()
    {
        return skyColor;
    }

    public Color32 HexToColor(string hex)
    {
        hex = hex.Replace("0x", "").Replace("#", "");
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }

    #endregion
}