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

    public Vector2 ToPlanet => transform.position - currentPlanet.transform.position;
    public Vector2 Normal => ToPlanet.normalized;

    [Header("State")]
    [ReadOnly] public Vector2 position;
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


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();

        position = transform.position;

        PlayerStateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, PlayerStateMachine, "Idle");
        WalkState = new PlayerWalkState(this, PlayerStateMachine, "Walk");
        RunState = new PlayerRunState(this, PlayerStateMachine, "Run");
        SprintState = new PlayerSprintState(this, PlayerStateMachine, "Sprint");
        AirborneState = new PlayerAirborneState(this, PlayerStateMachine, "Airborne");

        PlayerStateMachine.InitializeState(IdleState);
    }

    private void Update()
    {
        if (GameManager.Instance == null || planetIdx >= GameManager.Instance.planets.Count) return;

        currentPlanet = GameManager.Instance.planets[planetIdx];
        //ProcessInput(currentPlanet, _input.NormInputX, _input.NormInputY, _input.SprintInput, Time.deltaTime);

        PlayerStateMachine?.CurrentState?.UpdateState();
        UpdatePlayer();
        
        UpdateVisuals();
    }

    public void GetClosestPlanet(List<Planet> planets)
    {
        float closest = float.MaxValue;
        int closestIdx = planetIdx;

        for (int i = 0; i < planets.Count; i++)
        {
            Vector2 diff = planets[i].position - position;
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

    //private void ProcessGroundedStates(Planet planet, float x, float y, bool sprintInput, float dt)
    //{
    //    bool moved = x != 0;
    //    bool jumped = y > 0 && onGround;
    //    bool leaped = jumped && sprinting && charge >= 1;

    //    float speed = _data.PLAYER_WALK_SPEED;
    //    float height = _data.PLAYER_JUMP_HEIGHT;
    //    float mag = velocity.magnitude;

    //    // Handle movement direction and facing
    //    if (moved)
    //    {
    //        Vector2 toPlanet = position - planet.position;
    //        Vector2 normal = toPlanet.normalized;
    //        Vector2 tangent = new Vector2(-normal.y, normal.x);
    //        float tv = Vector2.Dot(velocity, tangent);
    //        facing = tv < 0 ? 1 : -1; // Flipped the facing logic
    //    }

    //    // Ground state checks
    //    if (!moved && onGround && mag < _data.PLAYER_IDLE_VEL_THRESHOLD)
    //    {
    //        velocity = Vector2.zero;
    //    }

    //    // Sprint handling
    //    if (onGround)
    //    {
    //        sprinting = sprintInput;
    //    }

    //    if (sprinting)
    //    {
    //        speed = charge >= 1 ? _data.PLAYER_CHARGED_SPEED : _data.PLAYER_RUN_SPEED;
    //    }

    //    if (leaped)
    //    {
    //        height = _data.PLAYER_LEAP_HEIGHT;
    //    }

    //    // State transitions
    //    if (jumped && leaped)
    //    {
    //        if (!moved)
    //        {
    //            EnterFlyState(planet);
    //        }
    //        else
    //        {
    //            EnterLeapState();
    //        }
    //        return;
    //    }

    //    // Movement and jump physics
    //    if (moved || jumped)
    //    {
    //        ApplyMovement(planet, x, jumped, speed, height, dt);
    //    }
    //}

    private void ProcessLeapState(Planet planet, float dt)
    {
        //float speed = _data.PLAYER_CHARGED_SPEED;
        //Vector2 toPlanet = position - planet.position;
        //Vector2 normal = toPlanet.normalized;

        //if (prevX != 0)
        //{
        //    Vector2 moveDirection = prevX > 0 ? new Vector2(-normal.y, normal.x) : new Vector2(normal.y, -normal.x); // Flipped movement direction
        //    velocity += dt * speed * moveDirection;
        //}
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

    private void UpdatePlayer()
    {
        transform.position = new Vector3(position.x, position.y, 0);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    private void UpdateFlyState(Planet planet)
    {
        position += velocity * Time.deltaTime;

        Vector2 toPlanet = position - planet.position;
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

        Vector2 toPlanet = position - currentPlanet.position;
        float dist = toPlanet.magnitude;
        float edge = currentPlanet.radius + Data.PLAYER_RADIUS;

        if (dist < edge + Data.PLAYER_ON_GROUND_THRESHOLD)
        {
            // Handle ground collision
            Vector2 normal = toPlanet / dist;
            position = currentPlanet.position + normal * edge;
            lastNormal = normal;

            float vel = Vector2.Dot(velocity, normal);
            if (vel < 0)
            {
                velocity -= vel * normal;
            }

            return true;
        }

        return false;
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

    private void UpdateVisuals()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facing < 0;
        }
    }

    private void ApplyMovement(Planet planet, float x, bool jumped, float speed, float height, float dt)
    {
        Vector2 toPlanet = position - planet.position;
        Vector2 normal = toPlanet.normalized;

        if (x != 0)
        {
            Vector2 moveDir = x > 0 ? new Vector2(normal.y, -normal.x) : new Vector2(-normal.y, normal.x);
            velocity += dt * speed * moveDir;
        }

        if (jumped)
        {
            velocity += normal * height;
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