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
    }

    public string planetName;
    public Vector2 position;
    public float angle;
    public float speed;
    public float orbit;
    public float radius;
    public float gravity;
    public Color32[] palette = new Color32[5];

    public PlanetCloud[] clouds = new PlanetCloud[24];
    public PlanetDecoration[] decorationsFg = new PlanetDecoration[48];
    public PlanetDecoration[] decorationsBg = new PlanetDecoration[64];

    public int cloudsCount;
    public int decorationsFgCount;
    public int decorationsBgCount;

    public bool completed;
    public Planet orbits;

    private const float DEG2RAD = Mathf.Deg2Rad;
    private const float RAD2DEG = Mathf.Rad2Deg;
    private const float PI = Mathf.PI;

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

        planets.Add(planet);
        return planet;
    }

    public void AddToOrbit(Planet orbiter, float distance, float offset)
    {
        orbiter.orbit = distance;
        orbiter.orbits = this;

        orbiter.angle = offset;

        orbiter.speed = Mathf.Sqrt(orbiter.radius / orbiter.orbit) * 10f;

        orbiter.position = new Vector2(
            position.x + Mathf.Sin(orbiter.angle * DEG2RAD) * orbiter.orbit,
            position.y + Mathf.Cos(orbiter.angle * DEG2RAD) * orbiter.orbit
        );
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

        for (int i = 0; i < decorationsFgCount; i++)
        {
            int idx = Random.Range(0, fgTypes.Length);
            uint type = fgTypes[idx];
            float angle = RandomFloat(0, 2 * PI);
            float offset = RandomFloat(16, 80);

            float x = (radius - offset) * Mathf.Cos(angle);
            float y = (radius - offset) * Mathf.Sin(angle);

            float dx = -x;
            float dy = -y;

            float rotation = Mathf.Atan2(-dx, dy) * RAD2DEG;

            decorationsFg[i] = new PlanetDecoration
            {
                type = type,
                rotation = rotation,
                position = new Vector2(x, y)
            };
        }

        for (int i = 0; i < decorationsBgCount; i++)
        {
            int idx = Random.Range(0, bgTypes.Length);
            uint type = bgTypes[idx];
            float angle = RandomFloat(0, 2 * PI);
            float offset = RandomFloat(4, 16);

            float x = (radius - offset) * Mathf.Cos(angle);
            float y = (radius - offset) * Mathf.Sin(angle);

            float dx = -x;
            float dy = -y;

            float rotation = Mathf.Atan2(-dx, dy) * RAD2DEG;

            decorationsBg[i] = new PlanetDecoration
            {
                type = type,
                rotation = rotation,
                position = new Vector2(x, y)
            };
        }

        for (int i = 0; i < cloudsCount; i++)
        {
            uint type = (uint)Random.Range(0, 3);
            float speed = RandomFloat(0.01f, 0.06f);
            float angle = RandomFloat(0, 2 * PI);
            float cloudRadius = radius + RandomFloat(96, 256);

            float x = position.x + cloudRadius * Mathf.Cos(angle);
            float y = position.y + cloudRadius * Mathf.Sin(angle);

            float dx = position.x - x;
            float dy = position.y - y;

            float rotation = Mathf.Atan2(-dx, dy) * RAD2DEG;

            clouds[i] = new PlanetCloud
            {
                type = type,
                angle = angle,
                speed = speed,
                radius = cloudRadius,
                rotation = rotation,
                position = new Vector2(x, y)
            };
        }
    }

    public void UpdatePlanet(float dt)
    {
        if (orbits != null)
        {
            angle += speed * dt;

            position = new Vector2(
                orbits.position.x + Mathf.Sin(angle * DEG2RAD) * orbit,
                orbits.position.y + Mathf.Cos(angle * DEG2RAD) * orbit
            );

            transform.position = new Vector3(position.x, position.y, 0);
        }

        for (int i = 0; i < cloudsCount; i++)
        {
            var cloud = clouds[i];
            float speed = cloud.speed;
            float angle = cloud.angle + speed * dt;

            float x = position.x + cloud.radius * Mathf.Cos(angle);
            float y = position.y + cloud.radius * Mathf.Sin(angle);

            float dx = position.x - x;
            float dy = position.y - y;

            clouds[i].position = new Vector2(x, y);
            clouds[i].angle = angle;
            clouds[i].rotation = Mathf.Atan2(-dx, dy) * RAD2DEG;
        }
    }
}