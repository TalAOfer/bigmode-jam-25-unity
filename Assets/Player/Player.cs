using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using JetBrains.Annotations;
using NUnit.Framework.Interfaces;

[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData _data;
    public PlayerData Data => _data;
    [SerializeField] private PlayerInputController _input;
    public PlayerInputController Input => _input;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Animator Anim {  get; private set; }
    [ShowInInspector]public bool View_IsGrounded => IsGrounded();
    [ShowInInspector] public bool View_IsTransitioningState => PlayerStateMachine != null
        && PlayerStateMachine.CurrentState != null
        && PlayerStateMachine.CurrentState.hasStartedTransition;
    public Vector2 ToPlanet => transform.position - currentPlanet.transform.position;
    public Vector2 Normal => ToPlanet.normalized;

    [Header("State")]
    [ReadOnly] public Vector2 velocity;
    [ReadOnly] public Vector2 direction;
    [ReadOnly] public int facing = 1;
    [ReadOnly] public float rotation;
    [ReadOnly] public bool sprinting;
    [ReadOnly] public float charge;
    [ReadOnly] public int planetIdx;
    [ReadOnly] public Planet currentPlanet;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private Vector2 lastNormal;
    public float verticalVelocity;

    public string PlayerStateName;
    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerSprintState SprintState { get; private set; }
    public PlayerAirborneState AirborneState { get; private set; }
    public PlayerFlyState FlyState { get; private set; }


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();

        PlayerStateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, PlayerStateMachine, "Idle");
        WalkState = new PlayerWalkState(this, PlayerStateMachine, "Walk");
        RunState = new PlayerRunState(this, PlayerStateMachine, "Run");
        SprintState = new PlayerSprintState(this, PlayerStateMachine, "Sprint");
        AirborneState = new PlayerAirborneState(this, PlayerStateMachine, "Airborne");
        FlyState = new PlayerFlyState(this, PlayerStateMachine, "Fly");

        GameManager.Instance.audioController.PlayOneShot("Player/Movement");

        PlayerStateMachine.InitializeState(IdleState);
    }

    public void UpdatePlayer()
    {
        if (GameManager.Instance == null || planetIdx >= GameManager.Instance.galaxyManager.planets.Count) return;

        currentPlanet = GameManager.Instance.galaxyManager.planets[planetIdx];

        PlayerStateMachine?.CurrentState?.UpdateState();

        transform.position += (Vector3)velocity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        
    }

    public void GetClosestPlanet(List<Planet> planets)
    {
        float closest = float.MaxValue;
        int closestIdx = planetIdx;

        for (int i = 0; i < planets.Count; i++)
        {
            Vector2 diff = planets[i].position - (Vector2)transform.position;
            float dist = diff.magnitude;
            float edge = dist - (planets[i].radius + _data.PLAYER_RADIUS);

            if (edge < closest)
            {
                closest = edge;
                closestIdx = i;
            }
        }

        if (planetIdx != closestIdx)
        {
            planetIdx = closestIdx;
            // Optional: Add planet transition effects here
        }
    }

    private void ProcessFlyState(float x, float y)
    {
        velocity = direction * _data.PLAYER_FLY_SPEED;

        if (x == 0 && y == 0) return;

        // Normalize diagonal movement
        if (x == 0) x = 1 - Mathf.Abs(y);
        if (y == 0) y = 1 - Mathf.Abs(x);

        // Calculate target direction in planet space
        float rads = rotation * Mathf.Deg2Rad;
        Vector2 target = new Vector2(
            x * Mathf.Cos(rads) - y * Mathf.Sin(rads),
            x * Mathf.Sin(rads) + y * Mathf.Cos(rads)
        ).normalized;

        direction = Vector2.Lerp(direction, target, _data.PLAYER_FLY_HANDLING);
        direction = direction.normalized;
    }

    private void UpdateFlyState(Planet planet)
    {
        Vector2 toPlanet = (Vector2)transform.position - planet.position;
        float dist = toPlanet.magnitude;
        float edge = planet.radius + _data.PLAYER_RADIUS;

        if (dist < edge + _data.PLAYER_ON_GROUND_THRESHOLD)
        {
            //state = PlayerState.Idle;
            velocity = Vector2.zero;
        }
    }

    public bool IsGrounded()
    {
        if (currentPlanet == null) return false;
        Vector2 toPlanet = (Vector2)transform.position - currentPlanet.position;
        float dist = toPlanet.magnitude;
        float edge = currentPlanet.radius + Data.PLAYER_RADIUS;
        return dist < edge + Data.PLAYER_ON_GROUND_THRESHOLD;
    }

    public void HandleGroundCollision()
    {
        Vector2 toPlanet = (Vector2)transform.position - currentPlanet.position;
        float dist = toPlanet.magnitude;
        float edge = currentPlanet.radius + Data.PLAYER_RADIUS;

        // Only handle collision if we're actually colliding
        if (dist < edge + Data.PLAYER_ON_GROUND_THRESHOLD)
        {
            Vector2 normal = toPlanet / dist;
            transform.position = currentPlanet.position + normal * edge;
            lastNormal = normal;
            float vel = Vector2.Dot(velocity, normal);
            if (vel < 0)
            {
                velocity -= vel * normal;
            }
        }
    }

    public void ApplyFriction(Planet planet, float dist, float dt)
    {
        float t = Mathf.Clamp01((dist - planet.radius) / planet.radius);
        float friction = Mathf.Lerp(
            IsGrounded() ? _data.PLAYER_FRICTION_GROUND : _data.PLAYER_FRICTION_AIR,
            1,
            t
        );

        velocity *= 1 / (1 + friction * dt);
    }

    public void UpdateFacingDirection()
    {
        if (spriteRenderer != null)
        {
            Vector2 normal = Normal;
            Vector2 tangent = new (-normal.y, normal.x);
            float tv = Vector2.Dot(velocity, tangent);
            if (velocity == Vector2.zero) return;
            facing = tv < 0 ? 1 : -1;
            spriteRenderer.flipX = facing < 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // Draw velocity vector
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)velocity);

        // Draw ground normal
        if (IsGrounded())
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)lastNormal);
        }

        // Draw player radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _data.PLAYER_RADIUS);
    }
}