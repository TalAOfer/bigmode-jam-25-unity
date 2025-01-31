using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
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

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private Camera cam;
    [SerializeField] private Player player;
    private float currentZoom;
    private float currentRotation;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        currentZoom = baseZoom;
        cam.orthographic = true;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            UpdateStateBasedCamera();
        }
        UpdateCamera();
        UpdateShake();
    }

    private void UpdateStateBasedCamera()
    {
        // Update target to follow player
        target = player.position;

        // Determine zoom based on player state
        bool shouldZoomPlayer = !(player.state == Player.PlayerState.Fly ||
                                player.state == Player.PlayerState.Leap);

        float targetZoom = shouldZoomPlayer ?
            ((player.charge >= 1 && player.sprinting) ? sprintZoom : normalZoom) :
            flyZoom;

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * 10f);

        // Update rotation
        bool shouldAnglePlayer = player.state != Player.PlayerState.Leap;
        if (shouldAnglePlayer && player.planetIdx >= 0 && player.planetIdx < GameManager.Instance.planets.Count)
        {
            Planet currentPlanet = GameManager.Instance.planets[player.planetIdx];
            Vector2 toPlanet = currentPlanet.position - player.position;

            // Calculate angle in degrees (matching original C implementation)
            float targetRotation = Mathf.Atan2(-toPlanet.x, toPlanet.y) * Mathf.Rad2Deg + 180f;

            // Calculate shortest rotation path
            float angleDiff = Mathf.DeltaAngle(currentRotation, targetRotation);
            currentRotation += angleDiff * rotationSpeed;
        }
    }

    private void UpdateCamera()
    {
        // Update position with lerp
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = new Vector3(target.x, target.y, currentPosition.z);

        // Apply easing based on state
        float currentEasing = player != null && player.state == Player.PlayerState.Fly ?
            1f : easing;

        transform.position = Vector3.Lerp(currentPosition, targetPosition, currentEasing);

        // Update zoom (orthographic size)
        cam.orthographicSize = currentZoom;

        // Update rotation
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
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

            transform.position += new Vector3(shakeOffset.x, shakeOffset.y, 0);
        }
        else
        {
            shakeOffset = Vector2.zero;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || !Application.isPlaying || player == null ||
            player.planetIdx < 0 || player.planetIdx >= GameManager.Instance.planets.Count)
            return;

        Planet currentPlanet = GameManager.Instance.planets[player.planetIdx];
        Vector2 toPlanet = currentPlanet.position - player.position;

        // Draw line to current planet
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, currentPlanet.transform.position);

        // Draw camera direction
        Gizmos.color = Color.blue;
        Vector3 rotationDir = Quaternion.Euler(0, 0, currentRotation) * Vector3.up;
        Gizmos.DrawRay(transform.position, rotationDir * 2);
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

    public Rect GetFrustum()
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        return new Rect(
            position.x - halfWidth,
            position.y - halfHeight,
            halfWidth * 2,
            halfHeight * 2
        );
    }

    public bool ShouldCull(Rect bounds)
    {
        Rect frustum = GetFrustum();
        return !frustum.Overlaps(bounds);
    }
}