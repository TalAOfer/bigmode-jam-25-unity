using UnityEngine;
using System.Collections.Generic;

public class GalaxyManager : MonoBehaviour
{
    [Header("Solar System Configuration")]
    [SerializeField] private GameObject defaultPlanetPrefab;

    [Header("Planet Visuals")]
    [SerializeField] private List<Sprite> cloudSprites;
    [SerializeField] private List<Sprite> foregroundDecorations;
    [SerializeField] private List<Sprite> backgroundDecorations;

    [SerializeField] private SunBlueprint sunBlueprint;

    private Transform planetsContainer;
    public List<Planet> planets { get; private set; } = new List<Planet>();
    private bool[] renderedPlanets = new bool[16]; // Max planets

    public void Initialize()
    {
        if (planetsContainer == null)
        {
            planetsContainer = new GameObject("Planets Container").transform;
        }

        CreateSolarSystem();
    }


    private void CreateSolarSystem()
    {
        // Create sun and planets
        Planet sun = CreatePlanet(sunBlueprint);
       
        foreach (PlanetBlueprint planetBlueprint in sunBlueprint.planets)
        {
            Planet planet = CreatePlanet(planetBlueprint);
            sun.AddToOrbit(planet, planetBlueprint.distanceFromParent, planetBlueprint.rotationOffset);

            foreach(MoonBlueprint moonBlueprint in planetBlueprint.moons)
            {
                Planet moon = CreatePlanet(moonBlueprint);
                planet.AddToOrbit(moon, moonBlueprint.distanceFromParent, moonBlueprint.rotationOffset);
            }
        }

        //// Add decorations
        //DecoratePlanet(planet1);
    }

    private Planet CreatePlanet(CelestialBodyBlueprint blueprint)
    {
        GameObject prefab = (blueprint.useDefaultPrefab || blueprint.prefab == null)
            ? defaultPlanetPrefab :
            blueprint.prefab;

        GameObject planetObj = Instantiate(prefab, planetsContainer);
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

    private void ConfigurePlanet(Planet planet, CelestialBodyBlueprint blueprint)
    {
        planet.planetName = blueprint.name;
        planet.radius = blueprint.radius;
        planet.gravity = blueprint.gravity;
        planet.palette = new Color32[5];
        //System.Array.Copy(blueprint.palette, planet.palette, 5);

        // Set initial transform values
        planet.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        planet.transform.localScale = Vector3.one * (blueprint.radius * 2);

        planet.RescalePremadeDecorations();
    }

    private void DecoratePlanet(Planet planet)
    {
        uint[] fgTypes = new uint[] { 1, 2, 3, 4, 5, 6 };
        uint[] bgTypes = new uint[] { 7, 8, 9, 10, 11 };
        planet.Decorate(fgTypes, bgTypes);
    }

    public void UpdatePlanets(float deltaTime)
    {
        foreach (Planet planet in planets)
        {
            if (planet != null)
            {
                planet.UpdatePlanet(deltaTime);
            }
        }
    }

    public void UpdateVisiblePlanets(Camera mainCamera, float planetVisibilityMultiplier)
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
        }
    }

    public bool IsPlanetVisible(int index)
    {
        return index < renderedPlanets.Length && renderedPlanets[index];
    }
}