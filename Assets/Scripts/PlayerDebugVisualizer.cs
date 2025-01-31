using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerDebugVisualizer : MonoBehaviour
{
    [SerializeField] private PlayerData _data;

    [Header("Visualization Settings")]
    [SerializeField] private bool showCollision = true;
    [SerializeField] private bool showPlanetInteraction = true;
    [SerializeField] private bool showVelocity = true;
    [SerializeField] private bool showGroundNormal = true;

    [Header("Color Settings")]
    [SerializeField] private Color collisionColor = new Color(1f, 1f, 0f, 0.5f); // Semi-transparent yellow
    [SerializeField] private Color planetInteractionColor = new Color(0f, 1f, 1f, 0.5f); // Semi-transparent cyan
    [SerializeField] private Color velocityColor = Color.green;
    [SerializeField] private Color groundNormalColor = Color.blue;

    [Header("Line Settings")]
    [SerializeField] private float lineThickness = 2f;
    [SerializeField] private float velocityLineScale = 0.5f; // Scale factor for velocity vector

    private Player player;
    private Planet currentPlanet => GameManager.Instance?.planets[player.planetIdx];

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || player == null) return;

        if (showCollision)
        {
            DrawPlayerCollision();
        }

        if (showPlanetInteraction && currentPlanet != null)
        {
            DrawPlanetInteraction();
        }

        if (showVelocity)
        {
            DrawVelocityVector();
        }

        if (showGroundNormal && player.onGround && currentPlanet != null)
        {
            DrawGroundNormal();
        }
    }

    private void DrawPlayerCollision()
    {
        // Draw player's collision circle
        Gizmos.color = collisionColor;
        Gizmos.DrawWireSphere(transform.position, _data.PLAYER_RADIUS);

        // Draw player's center point
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }

    private void DrawPlanetInteraction()
    {
        Vector2 toPlanet = currentPlanet.position - player.position;
        float dist = toPlanet.magnitude;
        float interactionRadius = currentPlanet.radius + _data.PLAYER_RADIUS;
        float groundThreshold = interactionRadius + _data.PLAYER_ON_GROUND_THRESHOLD;

        // Draw line to current planet
        Gizmos.color = planetInteractionColor;
        Gizmos.DrawLine(transform.position, currentPlanet.transform.position);

        // Draw interaction range
        UnityEditor.Handles.color = new Color(planetInteractionColor.r, planetInteractionColor.g, planetInteractionColor.b, 0.2f);
        UnityEditor.Handles.DrawWireDisc(currentPlanet.transform.position, Vector3.forward, interactionRadius);

        // Draw ground detection threshold
        UnityEditor.Handles.color = new Color(planetInteractionColor.r, planetInteractionColor.g, planetInteractionColor.b, 0.1f);
        UnityEditor.Handles.DrawWireDisc(currentPlanet.transform.position, Vector3.forward, groundThreshold);

        // Draw distance information
        Vector3 textPosition = transform.position + Vector3.up * 0.5f;
        UnityEditor.Handles.Label(textPosition, $"Distance: {dist:F1}");
    }

    private void DrawVelocityVector()
    {
        if (player.velocity.magnitude > 0.01f)
        {
            Vector3 velocityEnd = transform.position + (Vector3)(player.velocity * velocityLineScale);
            Gizmos.color = velocityColor;
            Gizmos.DrawLine(transform.position, velocityEnd);

            // Draw arrow head
            Vector3 direction = velocityEnd - transform.position;
            Vector3 right = Quaternion.Euler(0, 0, 20) * -direction.normalized * 0.5f;
            Vector3 left = Quaternion.Euler(0, 0, -20) * -direction.normalized * 0.5f;
            Gizmos.DrawLine(velocityEnd, velocityEnd + right);
            Gizmos.DrawLine(velocityEnd, velocityEnd + left);
        }
    }

    private void DrawGroundNormal()
    {
        Vector2 toPlanet = player.position - currentPlanet.position;
        Vector2 normal = toPlanet.normalized;
        Vector3 normalEnd = transform.position + (Vector3)(normal * _data.PLAYER_RADIUS * 1.5f);

        Gizmos.color = groundNormalColor;
        Gizmos.DrawLine(transform.position, normalEnd);

        // Draw tangent vectors (movement plane)
        Vector2 tangent = new Vector2(-normal.y, normal.x);
        Vector3 tangentRight = transform.position + (Vector3)(tangent * _data.PLAYER_RADIUS);
        Vector3 tangentLeft = transform.position + (Vector3)(-tangent * _data.PLAYER_RADIUS);

        Gizmos.color = new Color(groundNormalColor.r, groundNormalColor.g, groundNormalColor.b, 0.5f);
        Gizmos.DrawLine(transform.position, tangentRight);
        Gizmos.DrawLine(transform.position, tangentLeft);
    }
}