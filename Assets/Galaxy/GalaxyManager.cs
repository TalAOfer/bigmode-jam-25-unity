using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public class GalaxyManager : MonoBehaviour
{
    [SerializeField] private Transform planetsContainer;
    public List<Planet> planets = new List<Planet>();

    [Button]
    public void PopulatePlanetList()
    {
        planets = planetsContainer.GetComponentsInChildren<Planet>().ToList();
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

    //    //// Add decorations
    //    //DecoratePlanet(planet1);
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

    //private void DecoratePlanet(Planet planet)
    //{
    //    uint[] fgTypes = new uint[] { 1, 2, 3, 4, 5, 6 };
    //    uint[] bgTypes = new uint[] { 7, 8, 9, 10, 11 };
    //    planet.Decorate(fgTypes, bgTypes);
    //}

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
}