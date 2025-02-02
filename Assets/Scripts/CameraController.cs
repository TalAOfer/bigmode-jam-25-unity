using DG.Tweening;
using FMOD.Studio;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Camera Properties")]
    [SerializeField] private float baseZoom = 2f;
    [SerializeField] private float angle;
    [SerializeField] private float easing = 0.5f;

    [Header("State-Based Settings")]
    [SerializeField] private float normalZoom = 2f;
    [SerializeField] private float sprintZoom = 1f;
    [SerializeField] private float flyZoom = 0.5f;
    [SerializeField] private float planetZoom = 10f;

    [SerializeField] private float watchPlanetSpeed = 1f;
    [SerializeField] private Ease watchPlanetEase = Ease.InSine;
    [SerializeField] private float watchPlanetZoomDuration = 0.5f;
    
    [SerializeField] private float landRotationSpeed = 0.025f;
    [SerializeField] private float onPlanetRotationSpeed = 0.05f;

    [Header("Screen Shake")]
    [SerializeField] private Vector2 shakeOffset;
    [SerializeField] private float shakeAmount;
    [SerializeField] private float shakeTime;

    [SerializeField] private Camera cam;
    [SerializeField] private Player player;
    private float currentZoom;
    private float currentRotation;

    private Transform target;

    private bool isCutscene;
    
    private CameraState currentState = CameraState.Normal;
    private bool isLocked;

    public void SetCameraState(CameraState newState) 
    {
        if (isLocked) return;

        switch (newState)
        {
            case CameraState.Normal:
                target = player.transform;
                break;
            case CameraState.Sprint:
                target = player.transform;
                break;
            case CameraState.Land:
                //isLocked = true;
                //StartCoroutine(LandRoutine());
                break;
            case CameraState.Fly:
                target = player.transform;
                break;
        }

        currentState = newState;
    }

    public IEnumerator WatchPlanetRoutine(Planet planet)
    {
        isCutscene = true;
        cam.DOOrthoSize(planetZoom, watchPlanetZoomDuration);
        Vector3 pos = planet.transform.position;
        pos.z = -10f;
        yield return cam.transform.DOMove(pos, watchPlanetSpeed).SetSpeedBased().SetEase(watchPlanetEase).WaitForCompletion();

    }

    public IEnumerator GoBackToPlayerRoutine()
    {
        Vector3 pos = player.transform.position;
        pos.z = -10f;
        yield return cam.transform.DOMove(pos, watchPlanetSpeed).SetSpeedBased().SetEase(watchPlanetEase).WaitForCompletion();
        yield return cam.DOOrthoSize(normalZoom, watchPlanetZoomDuration).WaitForCompletion();
        isCutscene = false;
    }

    public IEnumerator LandRoutine()
    {
        yield return new WaitForSeconds(1f);
        isLocked = false;
        currentState = CameraState.Normal;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        currentZoom = baseZoom;
        cam.orthographic = true;
    }
    public enum CameraState
    {
        Normal,
        Sprint,
        Land,
        Fly,
    }
    
    private void LateUpdate()
    {
        if (player != null)
        {
            UpdateBasedOnState();
        }

        if (isCutscene) return;

        UpdateCamera();
        UpdateShake();
    }

    private void UpdateBasedOnState()
    {
        if (isCutscene) return;

        float targetZoom = 0;
        switch (currentState)
        {
            case CameraState.Normal:
                targetZoom = normalZoom;
                break;
            case CameraState.Sprint:
                targetZoom = sprintZoom;
                break;
            case CameraState.Land:
                targetZoom = normalZoom;
                break;
            case CameraState.Fly:
                targetZoom = flyZoom;
                break;
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * 10f);

        // Handle rotation based on state

        if (currentState != CameraState.Fly && player.planetIdx >= 0 &&
            player.planetIdx < GameManager.Instance.galaxyManager.planets.Count)
        {
            float rotationSpeed = currentState == CameraState.Land ? landRotationSpeed : onPlanetRotationSpeed;
            Planet currentPlanet = GameManager.Instance.galaxyManager.planets[player.planetIdx];
            Vector2 toPlanet = (Vector2)currentPlanet.transform.position - (Vector2)player.transform.position;

            float targetRotation = Mathf.Atan2(-toPlanet.x, toPlanet.y) * Mathf.Rad2Deg + 180f;
            float angleDiff = Mathf.DeltaAngle(currentRotation, targetRotation);
            currentRotation += angleDiff * rotationSpeed;
        }
    }

    private void UpdateCamera()
    {
        // Update position with lerp
        Vector3 currentPosition = cam.transform.position;
        Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, currentPosition.z);

        // Apply easing based on state
        float currentEasing = currentState == CameraState.Fly ?
            1f : easing;

        cam.transform.position = Vector3.Lerp(currentPosition, targetPosition, currentEasing);

        // Update zoom (orthographic size)
        cam.orthographicSize = currentZoom;

        // Update rotation
        cam.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    private void UpdateShake()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;
            shakeOffset = new Vector2(
                Random.Range(-1f, 1f) * shakeAmount,
                Random.Range(-1f, 1f) * shakeAmount
            );

            cam.transform.position += new Vector3(shakeOffset.x, shakeOffset.y, 0);
        }
        else
        {
            shakeOffset = Vector2.zero;
        }
    }

    // Public methods
    public void SetZoom(float newZoom) => currentZoom = Mathf.Max(0.1f, newZoom);
    public void SetAngle(float newAngle) => currentRotation = newAngle;

    public void StartShake(float amount, float duration)
    {
        shakeAmount = amount;
        shakeTime = duration;
    }
}