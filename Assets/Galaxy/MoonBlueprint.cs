using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class MoonBlueprint : CelestialBodyBlueprint
{
    [FoldoutGroup("Data")]
    public float distanceFromParent = 100f;
    [FoldoutGroup("Data"), Range(0f, 360f)]
    public float rotationOffset;
}