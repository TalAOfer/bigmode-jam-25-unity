using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private CameraData data;
    [SerializeField] private Camera lens;
    [SerializeField] private Transform cam;
    [SerializeField] private Player player;

    private float currentZoom;
    private float currentRotation;
    private bool isCutscene;
    private CameraState currentState = CameraState.Normal;
    private bool isLocked;

    private Vector2 shakeOffset;
    private float shakeAmount;
    private float shakeTime;

    public enum CameraState
    {
        Normal,
        Sprint,
        Land,
        Fly,
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

        currentZoom = data.baseZoom;
        lens.orthographic = true;
    }

    private void LateUpdate()
    {
        if (isCutscene || player == null) return;

        UpdateZoom();
        UpdateRotation();
        UpdateCamera();
        UpdateShake();
    }

    private void UpdateZoom()
    {
        float targetZoom = currentState switch
        {
            CameraState.Normal => data.normalZoom,
            CameraState.Sprint => data.sprintZoom,
            CameraState.Fly => data.flyZoom,
            _ => data.normalZoom
        };

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * 10f);
    }

    private void UpdateRotation()
    {
        if (currentState == CameraState.Fly || player.planetIdx < 0 ||
            player.planetIdx >= GameManager.Instance.galaxyManager.planets.Count) return;

        float rotationSpeed = currentState == CameraState.Land ?
            data.landRotationSpeed : data.onPlanetRotationSpeed;

        Planet currentPlanet = GameManager.Instance.galaxyManager.planets[player.planetIdx];
        Vector2 toPlanet = (Vector2)currentPlanet.transform.position - (Vector2)player.transform.position;

        float targetRotation = Mathf.Atan2(-toPlanet.x, toPlanet.y) * Mathf.Rad2Deg + 180f;
        float angleDiff = Mathf.DeltaAngle(currentRotation, targetRotation);
        currentRotation += angleDiff * rotationSpeed;
    }

    private void UpdateCamera()
    {
        Vector3 currentPosition = cam.transform.position;
        Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, currentPosition.z);
        float currentEasing = currentState == CameraState.Fly ? 1f : data.easing;

        cam.transform.position = Vector3.Lerp(currentPosition, targetPosition, currentEasing);
        lens.orthographicSize = currentZoom;
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
            lens.transform.localPosition = new Vector3(shakeOffset.x, shakeOffset.y, 0);
        }
        else
        {
            lens.transform.localPosition = Vector3.Lerp(
                lens.transform.localPosition,
                Vector3.zero,
                data.easing * Time.deltaTime
            );
        }
    }

    public void SetCameraState(CameraState newState)
    {
        if (isLocked) return;

        currentState = newState;
    }

    public IEnumerator WatchPlanetRoutine(Planet planet)
    {
        isCutscene = true;
        lens.DOOrthoSize(data.planetZoom, data.watchPlanetZoomDuration);

        Vector3 pos = planet.transform.position;
        pos.z = -10f;

        yield return cam.transform
            .DOMove(pos, data.watchPlanetSpeed)
            .SetSpeedBased()
            .SetEase(data.watchPlanetEase)
            .WaitForCompletion();
    }

    public IEnumerator GoBackToPlayerRoutine()
    {
        Vector3 pos = player.transform.position;
        pos.z = -10f;

        yield return cam.transform
            .DOMove(pos, data.watchPlanetSpeed)
            .SetSpeedBased()
            .SetEase(data.watchPlanetEase)
            .WaitForCompletion();

        yield return lens
            .DOOrthoSize(data.normalZoom, data.watchPlanetZoomDuration)
            .WaitForCompletion();

        isCutscene = false;
    }

    public IEnumerator LandRoutine()
    {
        yield return new WaitForSeconds(1f);
        isLocked = false;
        currentState = CameraState.Normal;
    }

    public void SetZoom(float newZoom) => currentZoom = Mathf.Max(0.1f, newZoom);
    public void SetAngle(float newAngle) => currentRotation = newAngle;

    public void StartShake(float amount, float duration)
    {
        shakeAmount = amount;
        shakeTime = duration;
    }

    public void StartShake(ScreenShakeProfile profile)
    {
        shakeAmount = profile.amount;
        shakeTime = profile.duration;
    }
}