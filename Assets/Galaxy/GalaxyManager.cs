using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public class GalaxyManager : MonoBehaviour
{
    [SerializeField] private Transform planetsContainer;
    public List<Planet> planets = new List<Planet>();
    [SerializeField] private Player player;

    #region Debugs
    [FoldoutGroup("Debug"), SerializeField] private Color planetInteractionColor = new Color(0f, 1f, 1f, 0.5f);
    [FoldoutGroup("Debug"), SerializeField] private bool showPlanetInteractions;
    [FoldoutGroup("Debug"), SerializeField] private Color planetRadiusColor = new Color(0f, 1f, 1f, 0.5f);
    [FoldoutGroup("Debug"), SerializeField] private bool showPlanetRadius;
    [FoldoutGroup("Debug"), SerializeField] private Color planetOrbitColor = new Color(0f, 1f, 1f, 0.5f);
    [FoldoutGroup("Debug"), SerializeField] private bool showPlanetOrbits;
    #endregion

    [Button]
    public void PopulatePlanetList()
    {
        planets = planetsContainer.GetComponentsInChildren<Planet>().ToList();
        foreach (var planet in planets)
        {
            planet.Initialize();
        }
    }

    public void Initialize()
    {
        if (planetsContainer == null)
        {
            planetsContainer = new GameObject("Planets Container").transform;
        }

        foreach (var planet in planets)
        {
            planet.Initialize();
        }
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

    private void OnDrawGizmos()
    {
        foreach (var planet in planets)
        {
            if (showPlanetRadius)
            {
                Gizmos.color = planetRadiusColor;
                Gizmos.DrawWireSphere(planet.transform.position, planet.PlanetRadius);
            }

            if (showPlanetInteractions)
            {
                float groundThreshold = planet.PlanetRadius + player.Data.PLAYER_ON_GROUND_THRESHOLD;
                Gizmos.color = planetInteractionColor;
                Gizmos.DrawWireSphere(planet.transform.position, groundThreshold);
            }

            if (showPlanetOrbits && planet.parentPlanet != null)
            {
                Gizmos.color = planetOrbitColor;
                Gizmos.DrawWireSphere(planet.parentPlanet.transform.position, planet.OrbitRadius);
            }
        }
    }

    //private void CreateSolarSystem()
    //{
    //    // Create sun and planets
    //    Planet sun = CreatePlanet(sunBlueprint);

    //    foreach (PlanetBlueprint planetBlueprint in sunBlueprint.planets)
    //    {
    //        Planet planet = CreatePlanet(planetBlueprint);
    //        sun.AddToOrbit(planet, planetBlueprint.distanceFromParent, planetBlueprint.rotationOffset);
    //        planet.shouldOrbit = planetBlueprint.shouldOrbit;

    //        foreach (MoonBlueprint moonBlueprint in planetBlueprint.moons)
    //        {
    //            Planet moon = CreatePlanet(moonBlueprint);
    //            planet.AddToOrbit(moon, moonBlueprint.distanceFromParent, moonBlueprint.rotationOffset);
    //            moon.shouldOrbit = moonBlueprint.shouldOrbit;
    //        }
    //    }

    //}

    //private Planet CreatePlanet(CelestialBodyBlueprint blueprint)
    //{
    //    GameObject prefab = (blueprint.useDefaultPrefab || blueprint.prefab == null)
    //        ? defaultPlanetPrefab :
    //        blueprint.prefab;

    //    GameObject planetObj = Instantiate(prefab, planetsContainer);
    //    planetObj.name = blueprint.name;

    //    Planet planet = planetObj.GetComponent<Planet>();
    //    if (planet == null)
    //    {
    //        Debug.LogError($"Planet prefab is missing Planet component: {blueprint.name}");
    //        return null;
    //    }

    //    ConfigurePlanet(planet, blueprint);
    //    planets.Add(planet);

    //    return planet;
    //}

    //private void ConfigurePlanet(Planet planet, CelestialBodyBlueprint blueprint)
    //{
    //    planet.planetName = blueprint.name;
    //    planet.radius = blueprint.radius;
    //    planet.gravity = blueprint.gravity;

    //    planet.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    //    planet.transform.localScale = Vector3.one * (blueprint.radius * 2);

    //    planet.RescalePremadeDecorations();
    //}
}