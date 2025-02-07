using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera Data")]
public class CameraData : ScriptableObject
{
    [Header("Camera Properties")]
    public float baseZoom = 2f;
    public float easing = 0.5f;

    [Header("State-Based Settings")]
    public float normalZoom = 2f;
    public float sprintZoom = 1f;
    public float flyZoom = 0.5f;
    public float planetZoom = 10f;

    [Header("Watch Planet")]
    public float watchPlanetSpeed = 1f;
    public Ease watchPlanetEase = Ease.InSine;
    public float watchPlanetZoomDuration = 0.5f;

    public float landRotationSpeed = 0.025f;
    public float onPlanetRotationSpeed = 0.05f;
}
