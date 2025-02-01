
using Sirenix.OdinInspector;
using UnityEngine;

public class PlanetDirectionIndicator : MonoBehaviour
{
    [ReadOnly]
    public Planet planet;
    [SerializeField] private float addedPointingRotation = 0f;

    public void Initialize(Planet planet)
    {
        this.planet = planet;
    }

    public void UpdateIndicator(SharedIndicatorData sharedData)
    {
        if (planet == null) return;

        Vector2 planetScreenPos = sharedData.camera.WorldToScreenPoint(planet.transform.position);
        Vector2 screenDirection = (planetScreenPos - sharedData.playerScreenPos).normalized;

        // Calculate distances to each edge
        float distanceToRightEdge = (sharedData.screenSize.x - sharedData.padding - sharedData.playerScreenPos.x) / screenDirection.x;
        float distanceToLeftEdge = (sharedData.padding - sharedData.playerScreenPos.x) / screenDirection.x;
        float distanceToTopEdge = (sharedData.screenSize.y - sharedData.padding - sharedData.playerScreenPos.y) / screenDirection.y;
        float distanceToBottomEdge = (sharedData.padding - sharedData.playerScreenPos.y) / screenDirection.y;

        // Find the closest positive intersection
        float distance = float.MaxValue;
        if (distanceToRightEdge > 0) distance = Mathf.Min(distance, distanceToRightEdge);
        if (distanceToLeftEdge > 0) distance = Mathf.Min(distance, distanceToLeftEdge);
        if (distanceToTopEdge > 0) distance = Mathf.Min(distance, distanceToTopEdge);
        if (distanceToBottomEdge > 0) distance = Mathf.Min(distance, distanceToBottomEdge);

        // Calculate edge position
        Vector2 edgePos = sharedData.playerScreenPos + screenDirection * distance;

        // Convert to world position
        Vector3 worldPos = sharedData.camera.ScreenToWorldPoint(new Vector3(edgePos.x, edgePos.y, 0));
        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);

        // Calculate rotation to point at planet
        Vector2 worldDirection = (planet.transform.position - transform.position);
        float angle = Mathf.Atan2(worldDirection.y, worldDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + addedPointingRotation);
    }
}