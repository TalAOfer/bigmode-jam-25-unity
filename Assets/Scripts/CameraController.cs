using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Camera Properties")]
    [SerializeField] private Vector2 target;
    [SerializeField] private float zoom = 5f;
    [SerializeField] private float angle;
    [SerializeField] private float easing = 0.1f;

    [Header("Camera Bounds")]
    [SerializeField] private float maxDistance = 128f;

    [Header("Screen Shake")]
    [SerializeField] private Vector2 shakeOffset;
    [SerializeField] private float shakeAmount;
    [SerializeField] private float shakeTime;

    private Camera cam;
    private Transform camTransform;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        camTransform = cam.transform;

        // Ensure the camera is orthographic since we're working in 2D
        cam.orthographic = true;
    }

    private void LateUpdate()
    {
        UpdateCamera();
        UpdateShake();
    }

    private void UpdateCamera()
    {
        // Update position with lerp
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = new Vector3(target.x, target.y, currentPosition.z);

        // Apply easing
        transform.position = Vector3.Lerp(currentPosition, targetPosition, easing);

        // Apply max distance constraint
        if (maxDistance > 0)
        {
            float distance = Vector2.Distance(
                new Vector2(transform.position.x, transform.position.y),
                target
            );

            if (distance > maxDistance)
            {
                Vector2 direction = (target - new Vector2(transform.position.x, transform.position.y)).normalized;
                Vector2 constrainedPosition = target - (direction * maxDistance);
                transform.position = new Vector3(constrainedPosition.x, constrainedPosition.y, transform.position.z);
            }
        }

        // Update zoom (orthographic size)
        cam.orthographicSize = zoom;

        // Update rotation
        transform.rotation = Quaternion.Euler(0, 0, angle);
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

    // Public methods to control the camera
    public void SetTarget(Vector2 newTarget)
    {
        target = newTarget;
    }

    public void SetZoom(float newZoom)
    {
        zoom = Mathf.Max(0.1f, newZoom);
    }

    public void SetAngle(float newAngle)
    {
        angle = newAngle;
    }

    public void StartShake(float amount, float duration)
    {
        shakeAmount = amount;
        shakeTime = duration;
    }

    // Get camera frustum for culling
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

    // Check if bounds should be culled
    public bool ShouldCull(Rect bounds)
    {
        Rect frustum = GetFrustum();
        return !frustum.Overlaps(bounds);
    }
}