using UnityEngine;
using System.Collections.Generic;

public class Planet : MonoBehaviour
{
    [System.Serializable]
    public struct PlanetDecoration
    {
        public uint type;
        public float rotation;
        public Vector2 position;
        public Transform decorationTransform; // Reference to the actual GameObject
    }

    [System.Serializable]
    public struct PlanetCloud
    {
        public uint type;
        public float angle;
        public float speed;
        public float radius;
        public float rotation;
        public Vector2 position;
        public Transform cloudTransform; // Reference to the actual GameObject
    }

    [Header("Planet Properties")]
    public string planetName;
    public Vector2 position;
    public float angle;
    public float speed;
    public float orbit;
    public float radius;
    public float gravity;
    public Color32[] palette = new Color32[5];

    [Header("Visual Elements")]
    public PlanetCloud[] clouds = new PlanetCloud[24];
    public PlanetDecoration[] decorationsFg = new PlanetDecoration[48];
    public PlanetDecoration[] decorationsBg = new PlanetDecoration[64];

    public int cloudsCount;
    public int decorationsFgCount;
    public int decorationsBgCount;

    public bool completed;
    public Planet orbits;

    private readonly float DEG2RAD = Mathf.Deg2Rad;
    private readonly float RAD2DEG = Mathf.Rad2Deg;
    private readonly float PI = Mathf.PI;

    private void Awake()
    {
        // Initialize position
        transform.position = new Vector3(position.x, position.y, 0);
    }

    private float RandomFloat(float min, float max)
    {
        return Random.Range(min, max);
    }

    public static Planet Add(List<Planet> planets, string name, float radius, float gravity, Color32[] palette)
    {
        GameObject planetObj = new GameObject(name);
        Planet planet = planetObj.AddComponent<Planet>();

        planet.planetName = name;
        planet.palette = new Color32[5];
        System.Array.Copy(palette, planet.palette, 5);

        planet.radius = radius;
        planet.gravity = gravity;

        planet.orbit = 0;
        planet.orbits = null;

        // Set initial position
        planet.position = planetObj.transform.position;

        planets.Add(planet);
        return planet;
    }

    public void AddToOrbit(Planet orbiter, float distance, float offset)
    {
        orbiter.orbit = distance;
        orbiter.orbits = this;
        orbiter.angle = offset;

        // Unity-adjusted orbital speed calculation
        orbiter.speed = Mathf.Sqrt(orbiter.radius / orbiter.orbit) * 10f;

        // Calculate initial position (Unity coordinate system)
        orbiter.position = new Vector2(
            position.x + Mathf.Sin(orbiter.angle * DEG2RAD) * orbiter.orbit,
            position.y + Mathf.Cos(orbiter.angle * DEG2RAD) * orbiter.orbit
        );

        orbiter.transform.position = new Vector3(orbiter.position.x, orbiter.position.y, 0);
    }

    public void Decorate(uint[] fgTypes, uint[] bgTypes)
    {
        const int MIN_DECORATIONS = 32;
        const int MAX_DECORATIONS = 48;
        const int MIN_CLOUDS = 12;
        const int MAX_CLOUDS = 24;

        cloudsCount = Random.Range(MIN_CLOUDS, MAX_CLOUDS + 1);
        decorationsFgCount = Random.Range(MIN_DECORATIONS, MAX_DECORATIONS + 1);
        decorationsBgCount = decorationsFgCount + 16;

        // Create foreground decorations
        for (int i = 0; i < decorationsFgCount; i++)
        {
            CreateDecoration(i, fgTypes, true);
        }

        // Create background decorations
        for (int i = 0; i < decorationsBgCount; i++)
        {
            CreateDecoration(i, bgTypes, false);
        }

        // Create clouds
        for (int i = 0; i < cloudsCount; i++)
        {
            CreateCloud(i);
        }
    }

    private void CreateDecoration(int index, uint[] types, bool isForeground)
    {
        int idx = Random.Range(0, types.Length);
        uint type = types[idx];
        float angle = RandomFloat(0, 2 * PI);
        float offset = isForeground ? RandomFloat(16, 80) : RandomFloat(4, 16);

        // Calculate position in Unity's coordinate system
        float x = (radius - offset) * Mathf.Cos(angle);
        float y = (radius - offset) * Mathf.Sin(angle);

        Vector2 toCenter = new Vector2(-x, -y);
        float rotation = Mathf.Atan2(toCenter.y, toCenter.x) * RAD2DEG;

        // Create decoration GameObject
        GameObject decorObj = new GameObject($"{(isForeground ? "FG" : "BG")}_Decoration_{index}");
        decorObj.transform.SetParent(transform);
        decorObj.transform.localPosition = new Vector3(x, y, isForeground ? -1 : 1); // Z-ordering
        decorObj.transform.localRotation = Quaternion.Euler(0, 0, rotation);

        var decoration = new PlanetDecoration
        {
            type = type,
            rotation = rotation,
            position = new Vector2(x, y),
            decorationTransform = decorObj.transform
        };

        if (isForeground)
            decorationsFg[index] = decoration;
        else
            decorationsBg[index] = decoration;
    }

    private void CreateCloud(int index)
    {
        uint type = (uint)Random.Range(0, 3);
        float speed = RandomFloat(0.01f, 0.06f);
        float angle = RandomFloat(0, 2 * PI);
        float cloudRadius = radius + RandomFloat(96, 256);

        // Calculate position in Unity's coordinate system
        float x = position.x + cloudRadius * Mathf.Cos(angle);
        float y = position.y + cloudRadius * Mathf.Sin(angle);

        Vector2 toCenter = new Vector2(position.x - x, position.y - y);
        float rotation = Mathf.Atan2(toCenter.y, toCenter.x) * RAD2DEG;

        // Create cloud GameObject
        GameObject cloudObj = new GameObject($"Cloud_{index}");
        cloudObj.transform.SetParent(transform);
        cloudObj.transform.position = new Vector3(x, y, -2); // Clouds in front
        cloudObj.transform.rotation = Quaternion.Euler(0, 0, rotation);

        clouds[index] = new PlanetCloud
        {
            type = type,
            angle = angle,
            speed = speed,
            radius = cloudRadius,
            rotation = rotation,
            position = new Vector2(x, y),
            cloudTransform = cloudObj.transform
        };
    }

    public void UpdatePlanet(float dt)
    {
        if (orbits != null)
        {
            angle += speed * dt;

            // Update position in Unity's coordinate system
            position = new Vector2(
                orbits.position.x + Mathf.Sin(angle * DEG2RAD) * orbit,
                orbits.position.y + Mathf.Cos(angle * DEG2RAD) * orbit
            );

            transform.position = new Vector3(position.x, position.y, 0);
        }

        // Update clouds with Unity transformations
        for (int i = 0; i < cloudsCount; i++)
        {
            var cloud = clouds[i];
            cloud.angle += cloud.speed * dt;

            float x = position.x + cloud.radius * Mathf.Cos(cloud.angle);
            float y = position.y + cloud.radius * Mathf.Sin(cloud.angle);

            Vector2 toCenter = new Vector2(position.x - x, position.y - y);
            cloud.rotation = Mathf.Atan2(toCenter.y, toCenter.x) * RAD2DEG;

            cloud.position = new Vector2(x, y);

            // Update cloud transform
            if (cloud.cloudTransform != null)
            {
                cloud.cloudTransform.position = new Vector3(x, y, -2);
                cloud.cloudTransform.rotation = Quaternion.Euler(0, 0, cloud.rotation);
            }

            clouds[i] = cloud;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw planet radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        // Draw orbit if applicable
        if (orbits != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(orbits.transform.position, orbit);
        }
    }
}