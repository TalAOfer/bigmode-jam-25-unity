using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Camera Properties")]
    [SerializeField] private Vector2 target;
    [SerializeField] private float baseZoom = 2f;
    [SerializeField] private float angle;
    [SerializeField] private float easing = 0.5f;

    [Header("State-Based Settings")]
    [SerializeField] private float normalZoom = 2f;
    [SerializeField] private float sprintZoom = 1f;
    [SerializeField] private float flyZoom = 0.5f;
    [SerializeField] private float rotationSpeed = 0.05f;

    [Header("Screen Shake")]
    [SerializeField] private Vector2 shakeOffset;
    [SerializeField] private float shakeAmount;
    [SerializeField] private float shakeTime;

    [SerializeField] private Camera cam;
    [SerializeField] private Player player;
    private float currentZoom;
    private float currentRotation;

    private CameraState currentState = CameraState.Normal;
    public void SetCameraState(CameraState newState) => currentState = newState;

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
        Fly,
    }
    
    private void LateUpdate()
    {
        if (player != null)
        {
            UpdateBasedOnState();
        }
        UpdateCamera();
        UpdateShake();
    }

    private void UpdateBasedOnState()
    {
        // Update target to follow player
        target = player.transform.position;

        // Set zoom based on state
        float targetZoom = currentState switch
        {
            CameraState.Sprint => sprintZoom,
            CameraState.Fly => flyZoom,
            _ => normalZoom
        };

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * 10f);

        // Handle rotation based on state
        if (currentState != CameraState.Fly && player.planetIdx >= 0 &&
            player.planetIdx < GameManager.Instance.galaxyManager.planets.Count)
        {
            Planet currentPlanet = GameManager.Instance.galaxyManager.planets[player.planetIdx];
            Vector2 toPlanet = currentPlanet.position - (Vector2)player.transform.position;

            float targetRotation = Mathf.Atan2(-toPlanet.x, toPlanet.y) * Mathf.Rad2Deg + 180f;
            float angleDiff = Mathf.DeltaAngle(currentRotation, targetRotation);
            currentRotation += angleDiff * rotationSpeed;
        }
    }

    private void UpdateCamera()
    {
        // Update position with lerp
        Vector3 currentPosition = cam.transform.position;
        Vector3 targetPosition = new Vector3(target.x, target.y, currentPosition.z);

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
    public void SetTarget(Vector2 newTarget) => target = newTarget;
    public void SetZoom(float newZoom) => currentZoom = Mathf.Max(0.1f, newZoom);
    public void SetAngle(float newAngle) => currentRotation = newAngle;

    public void StartShake(float amount, float duration)
    {
        shakeAmount = amount;
        shakeTime = duration;
    }
}