using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public abstract class CelestialBodyBlueprint
{
    public string name;
    public bool useDefaultPrefab = true;
    [HideIf("useDefaultPrefab")]
    public GameObject prefab;
    [FoldoutGroup("Data")]
    public float radius = 16f;
    [FoldoutGroup("Data")]
    public float gravity = 32f;
    [FoldoutGroup("Data")]
    public Color32[] palette;
}

