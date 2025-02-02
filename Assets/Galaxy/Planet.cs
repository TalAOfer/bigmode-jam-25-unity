using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using System.Collections;

public class Planet : MonoBehaviour
{
    [Header("Planet Properties")]
    public bool shouldOrbit;
    public float gravity = 12f;
    public Planet parentPlanet;
    public Color skyboxColor;
    public Sprite indicatorSprite;
    [ShowIf("shouldOrbit")]
    public float speed;
    public bool IsVisible {  get; private set; }

    public void OnBecameVisible() => IsVisible = true;
    public void OnBecameInvisible() => IsVisible = false;
    public float radius {  get; private set; }
    public float angle { get; private set; }
    public float orbitRadius {  get; private set; }
    [ShowInInspector, ReadOnly] public bool completed {  get; private set; }

    private readonly float DEG2RAD = Mathf.Deg2Rad;

    public void Initialize()
    {
        radius = transform.localScale.x / 2f;
        orbitRadius = parentPlanet != null ?
            Vector2.Distance((Vector2)transform.position, (Vector2)parentPlanet.transform.position) :
            0;
            
    }

    public IEnumerator OnPlanetComplete()
    {
        completed = true;
        GameManager.Instance.audioController.PlayOneShot("World interaction/Puzzle Complete");
        yield return CameraController.Instance.WatchPlanetRoutine(this);

        GameManager.Instance.screenFlash.TriggerFlash();
        GameManager.Instance.screenFlash.TriggerPuzzleComplete();

        yield return new WaitForSeconds(1f);

        yield return CameraController.Instance.GoBackToPlayerRoutine();
    }

    public void RescalePremadeDecorations()
    {
        List<DecorationManager> decorationManagers = GetComponentsInChildren<DecorationManager>().ToList();

        if (decorationManagers.Count > 0 )
        {
            foreach ( DecorationManager manager in decorationManagers)
            {
                manager.RescaleDecorations();
            }
        }
    }

    public void UpdatePlanet(float dt)
    {
        if (shouldOrbit && parentPlanet != null)
        {
            angle += speed * dt;
            transform.position = new Vector2(
                parentPlanet.transform.position.x + Mathf.Sin(angle * DEG2RAD) * orbitRadius,
                parentPlanet.transform.position.y + Mathf.Cos(angle * DEG2RAD) * orbitRadius
            );

            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }


        #region Oldnews
        //// Update clouds with Unity transformations
        //for (int i = 0; i < cloudsCount; i++)
        //{
        //    var cloud = clouds[i];
        //    cloud.angle += cloud.speed * dt;

        //    float x = transform.position.x + cloud.radius * Mathf.Cos(cloud.angle);
        //    float y = transform.position.y + cloud.radius * Mathf.Sin(cloud.angle);

        //    Vector2 toCenter = new Vector2(transform.position.x - x, transform.position.y - y);
        //    cloud.rotation = Mathf.Atan2(toCenter.y, toCenter.x) * RAD2DEG;

        //    cloud.position = new Vector2(x, y);

        //    // Update cloud transform
        //    if (cloud.cloudTransform != null)
        //    {
        //        cloud.cloudTransform.position = new Vector3(x, y, -2);
        //        cloud.cloudTransform.rotation = Quaternion.Euler(0, 0, cloud.rotation);
        //    }

        //    clouds[i] = cloud;
        //}


        #endregion
    }

    private void OnDrawGizmos()
    {
        // Draw planet radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        // Draw orbit if applicable
        if (parentPlanet != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(parentPlanet.transform.position, orbitRadius);
        }
    }

    #region Old
    //private readonly float RAD2DEG = Mathf.Rad2Deg;
    //private readonly float DEG2RAD = Mathf.Deg2Rad;
    //private readonly float PI = Mathf.PI;

    //[System.Serializable]
    //public struct PlanetDecoration
    //{
    //    public uint type;
    //    public float rotation;
    //    public Vector2 position;
    //    public Transform decorationTransform; // Reference to the actual GameObject
    //}

    //[System.Serializable]
    //public struct PlanetCloud
    //{
    //    public uint type;
    //    public float angle;
    //    public float speed;
    //    public float radius;
    //    public float rotation;
    //    public Vector2 position;
    //    public Transform cloudTransform; // Reference to the actual GameObject
    //}

    //[Header("Visual Elements")]
    //public PlanetCloud[] clouds = new PlanetCloud[24];
    //public PlanetDecoration[] decorationsFg = new PlanetDecoration[48];
    //public PlanetDecoration[] decorationsBg = new PlanetDecoration[64];

    //public int cloudsCount;
    //public int decorationsFgCount;
    //public int decorationsBgCount;
    //public void Decorate(uint[] fgTypes, uint[] bgTypes)
    //{
    //    const int MIN_DECORATIONS = 32;
    //    const int MAX_DECORATIONS = 48;
    //    const int MIN_CLOUDS = 12;
    //    const int MAX_CLOUDS = 24;

    //    cloudsCount = Random.Range(MIN_CLOUDS, MAX_CLOUDS + 1);
    //    decorationsFgCount = Random.Range(MIN_DECORATIONS, MAX_DECORATIONS + 1);
    //    decorationsBgCount = decorationsFgCount + 16;

    //    // Create foreground decorations
    //    for (int i = 0; i < decorationsFgCount; i++)
    //    {
    //        CreateDecoration(i, fgTypes, true);
    //    }

    //    // Create background decorations
    //    for (int i = 0; i < decorationsBgCount; i++)
    //    {
    //        CreateDecoration(i, bgTypes, false);
    //    }

    //    // Create clouds
    //    for (int i = 0; i < cloudsCount; i++)
    //    {
    //        CreateCloud(i);
    //    }
    //}

    //private void CreateDecoration(int index, uint[] types, bool isForeground)
    //{
    //    int idx = Random.Range(0, types.Length);
    //    uint type = types[idx];
    //    float angle = RandomFloat(0, 2 * PI);
    //    float offset = isForeground ? RandomFloat(16, 80) : RandomFloat(4, 16);

    //    // Calculate position in Unity's coordinate system
    //    float x = (radius - offset) * Mathf.Cos(angle);
    //    float y = (radius - offset) * Mathf.Sin(angle);

    //    Vector2 toCenter = new Vector2(-x, -y);
    //    float rotation = Mathf.Atan2(toCenter.y, toCenter.x) * RAD2DEG;

    //    // Create decoration GameObject
    //    GameObject decorObj = new GameObject($"{(isForeground ? "FG" : "BG")}_Decoration_{index}");
    //    decorObj.transform.SetParent(transform);
    //    decorObj.transform.localPosition = new Vector3(x, y, isForeground ? -1 : 1); // Z-ordering
    //    decorObj.transform.localRotation = Quaternion.Euler(0, 0, rotation);

    //    var decoration = new PlanetDecoration
    //    {
    //        type = type,
    //        rotation = rotation,
    //        position = new Vector2(x, y),
    //        decorationTransform = decorObj.transform
    //    };

    //    if (isForeground)
    //        decorationsFg[index] = decoration;
    //    else
    //        decorationsBg[index] = decoration;
    //}


    //private float RandomFloat(float min, float max)
    //{
    //    return Random.Range(min, max);
    //}
    //private void CreateCloud(int index)
    //{
    //    uint type = (uint)Random.Range(0, 3);
    //    float speed = RandomFloat(0.01f, 0.06f);
    //    float angle = RandomFloat(0, 2 * PI);
    //    float cloudRadius = radius + RandomFloat(96, 256);

    //    // Calculate position in Unity's coordinate system
    //    float x = transform.position.x + cloudRadius * Mathf.Cos(angle);
    //    float y = transform.position.y + cloudRadius * Mathf.Sin(angle);

    //    Vector2 toCenter = new Vector2(transform.position.x - x, transform.position.y - y);
    //    float rotation = Mathf.Atan2(toCenter.y, toCenter.x) * RAD2DEG;

    //    // Create cloud GameObject
    //    GameObject cloudObj = new GameObject($"Cloud_{index}");
    //    cloudObj.transform.SetParent(transform);
    //    cloudObj.transform.position = new Vector3(x, y, -2); // Clouds in front
    //    cloudObj.transform.rotation = Quaternion.Euler(0, 0, rotation);

    //    clouds[index] = new PlanetCloud
    //    {
    //        type = type,
    //        angle = angle,
    //        speed = speed,
    //        radius = cloudRadius,
    //        rotation = rotation,
    //        position = new Vector2(x, y),
    //        cloudTransform = cloudObj.transform
    //    };
    //}

    //public void AddToOrbit(Planet orbiter, float distance, float offset)
    //{
    //    orbiter.orbit = distance;
    //    orbiter.orbits = this;
    //    orbiter.angle = offset;

    //    // Unity-adjusted orbital speed calculation
    //    orbiter.speed = Mathf.Sqrt(orbiter.radius / orbiter.orbit) * 10f;

    //    // Calculate initial position (Unity coordinate system)
    //    orbiter.transform.position = new Vector2(
    //        transform.position.x + Mathf.Sin(orbiter.angle * DEG2RAD) * orbiter.orbit,
    //        transform.position.y + Mathf.Cos(orbiter.angle * DEG2RAD) * orbiter.orbit
    //    );

    //    orbiter.transform.position = new Vector3(orbiter.transform.position.x, orbiter.transform.position.y, 0);
    //}

    #endregion
}