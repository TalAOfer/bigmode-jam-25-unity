using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene References")]
    [SerializeField] private Player player;
    [SerializeField] private Camera mainCamera;
    //[SerializeField] private AudioManager audioManager;
    //[SerializeField] private PauseMenuUI pauseMenu;

    [Header("Prefabs")]
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private PlanetBlueprint sunBlueprint;
    [SerializeField] private float planet1DistanceFromSun;
    [SerializeField] private float planet2DistanceFromSun;

    [SerializeField] private PlanetBlueprint planet1Blueprint;
    [SerializeField] private PlanetBlueprint planet2Blueprint;


    [Header("Planet Decorations")]
    [SerializeField] private List<Sprite> cloudSprites;
    [SerializeField] private List<Sprite> foregroundDecorations;
    [SerializeField] private List<Sprite> backgroundDecorations;

    public List<Planet> planets = new List<Planet>();
    private bool[] renderedPlanets;

    private Color32[] currentPalette = new Color32[5];
    private float skyColor = 0.77f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        renderedPlanets = new bool[16]; // Max planets
        SetupGame();
    }

    private void SetupGame()
    {
        // Create the solar system
        Planet sun = CreatePlanet(sunBlueprint);

        Planet planet1 = CreatePlanet(planet1Blueprint);

        Planet planet2 = CreatePlanet(planet2Blueprint);

        // Setup orbits
        sun.AddToOrbit(planet1, planet1DistanceFromSun, 0f);
        sun.AddToOrbit(planet2, planet2DistanceFromSun, 90f);

        // Add decorations
        planet1.Decorate(
            new uint[] { 1, 2, 3, 4, 5, 6 }, // FG decoration indices
            new uint[] { 7, 8, 9, 10, 11 }   // BG decoration indices
        );

        // Initialize player
        if (player != null)
        {
            player.position = new Vector2(320f, 320f);
            // Default planet is the first one
            player.planetIdx = 0;
        }
    }

    private Planet CreatePlanet(PlanetBlueprint blueprint)
    {
        GameObject planetObj = Instantiate(planetPrefab);
        planetObj.name = blueprint.name;
        Planet planet = planetObj.GetComponent<Planet>();

        planets.Add(planet);

        planet.name = blueprint.name;
        planet.radius = blueprint.radius;
        planet.gravity = blueprint.gravity;
        planet.palette = blueprint.palette;

        return planet;
    }

    private void Update()
    {
        // Handle pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //pauseMenu.TogglePause();
        }

        //if (pauseMenu.IsPaused)
          //  return;

        UpdateGameState();
    }

    private void UpdateGameState()
    {
        // Update all planets
        foreach (Planet planet in planets)
        {
            planet.UpdatePlanet(Time.deltaTime);
        }

        // Update which planets should be rendered based on camera view
        UpdateVisiblePlanets();

        // Update player's closest planet
        player.GetClosestPlanet(planets);

        // Handle world wrapping
        WrapPlayerPosition();

        // Update palette lerping
        UpdatePaletteLerp();
    }

    private void UpdateVisiblePlanets()
    {
        if (mainCamera == null) return;

        float viewportHeight = 2f * mainCamera.orthographicSize;
        float viewportWidth = viewportHeight * mainCamera.aspect;
        Vector2 cameraPos = mainCamera.transform.position;

        for (int i = 0; i < planets.Count; i++)
        {
            float viewRadius = Mathf.Max(viewportWidth, viewportHeight) / 2f * Mathf.Sqrt(2);
            Vector2 planetPos = planets[i].position;
            float dist = Vector2.Distance(planetPos, cameraPos);
            float threshold = viewRadius + planets[i].radius;

            renderedPlanets[i] = dist <= threshold;
        }
    }

    private void WrapPlayerPosition()
    {
        const float WORLD_BOUND = 16000f;

        if (player.position.x < -WORLD_BOUND) player.position = new Vector2(15000f, player.position.y);
        if (player.position.x > WORLD_BOUND) player.position = new Vector2(-15000f, player.position.y);
        if (player.position.y < -WORLD_BOUND) player.position = new Vector2(player.position.x, 15000f);
        if (player.position.y > WORLD_BOUND) player.position = new Vector2(player.position.x, -15000f);
    }

    private void UpdatePaletteLerp()
    {
        Planet currentPlanet = planets[player.planetIdx];
        for (int i = 0; i < 5; i++)
        {
            currentPalette[i] = Color32.Lerp(currentPalette[i], currentPlanet.palette[i], 0.05f);
        }

        // Update sky color based on distance from planet
        float dist = Vector2.Distance(player.position, currentPlanet.position);
        float t = Mathf.Clamp01((dist - currentPlanet.radius * 1.5f) / (currentPlanet.radius * 1.5f));
        skyColor = Mathf.Lerp(0.77f, 0f, t);
    }

    private Color32 HexToColor(string hex)
    {
        hex = hex.Replace("0x", "").Replace("#", "");
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }

    public bool IsPlanetVisible(int index)
    {
        return renderedPlanets[index];
    }

    public Color32[] GetCurrentPalette()
    {
        return currentPalette;
    }

    public float GetSkyColor()
    {
        return skyColor;
    }
}