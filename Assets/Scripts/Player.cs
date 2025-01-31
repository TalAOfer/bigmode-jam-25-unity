using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData _data;
    [SerializeField] private PlayerInputController _input;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public enum PlayerState
    {
        None,
        Idle,
        Walk,
        Run,
        Sprint,
        Jump,
        Fall,
        Leap,
        Fly,
        Count
    }

    [System.Serializable]
    public class PlayerAnimation
    {
        public float elapsed;
        // Add other animation properties as needed
    }

    [Header("State")]
    public PlayerState state;
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 direction;
    public int facing = 1;
    public float rotation;
    public bool sprinting;
    public bool onGround;
    public float charge;
    public int planetIdx;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private PlayerAnimation[] animations;
    private PlayerAnimation currentAnimation;
    private float prevX = 0;
    private float prevY = 0;
    private Vector2 lastNormal;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        position = transform.position;

        // Initialize animations
        animations = new PlayerAnimation[(int)PlayerState.Count];
        for (int i = 0; i < (int)PlayerState.Count; i++)
        {
            animations[i] = new PlayerAnimation();
        }
        currentAnimation = animations[0];
    }

    private void Update()
    {
        if (GameManager.Instance == null || planetIdx >= GameManager.Instance.planets.Count) return;

        Planet currentPlanet = GameManager.Instance.planets[planetIdx];
        ProcessInput(currentPlanet, _input.NormInputX, _input.NormInputY, _input.SprintInput, Time.deltaTime);
        UpdatePlayer(currentPlanet, Time.deltaTime);
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

    private void ProcessInput(Planet planet, float x, float y, bool sprintInput, float dt)
    {
        switch (state)
        {
            case PlayerState.Idle:
            case PlayerState.Walk:
            case PlayerState.Run:
            case PlayerState.Sprint:
            case PlayerState.Jump:
            case PlayerState.Fall:
                ProcessGroundedStates(planet, x, y, sprintInput, dt);
                break;

            case PlayerState.Leap:
                ProcessLeapState(planet, dt);
                break;

            case PlayerState.Fly:
                ProcessFlyState(x, y);
                break;
        }

        prevX = x;
        prevY = y;
    }

    private void ProcessGroundedStates(Planet planet, float x, float y, bool sprintInput, float dt)
    {
        bool moved = x != 0;
        bool jumped = y > 0 && onGround;
        bool leaped = jumped && sprinting && charge >= 1;

        float speed = _data.PLAYER_WALK_SPEED;
        float height = _data.PLAYER_JUMP_HEIGHT;
        float mag = velocity.magnitude;

        // Handle movement direction and facing
        if (moved)
        {
            Vector2 toPlanet = position - planet.position;
            Vector2 normal = toPlanet.normalized;
            Vector2 tangent = new Vector2(-normal.y, normal.x);
            float tv = Vector2.Dot(velocity, tangent);
            facing = tv > 0 ? 1 : -1;
        }

        // Ground state checks
        if (!moved && onGround && mag < _data.PLAYER_IDLE_VEL_THRESHOLD)
        {
            velocity = Vector2.zero;
        }

        // Sprint handling
        if (onGround)
        {
            sprinting = sprintInput;
        }

        if (sprinting)
        {
            speed = charge >= 1 ? _data.PLAYER_CHARGED_SPEED : _data.PLAYER_RUN_SPEED;
        }

        if (leaped)
        {
            height = _data.PLAYER_LEAP_HEIGHT;
        }

        // State transitions
        if (jumped && leaped)
        {
            if (!moved)
            {
                EnterFlyState(planet);
            }
            else
            {
                EnterLeapState();
            }
            return;
        }

        // Movement and jump physics
        if (moved || jumped)
        {
            ApplyMovement(planet, x, jumped, speed, height, dt);
        }
    }

    private void ProcessLeapState(Planet planet, float dt)
    {
        float speed = _data.PLAYER_CHARGED_SPEED;
        Vector2 toPlanet = position - planet.position;
        Vector2 normal = toPlanet.normalized;

        if (prevX != 0)
        {
            Vector2 moveDirection = prevX > 0 ? new Vector2(normal.y, -normal.x) : new Vector2(-normal.y, normal.x);
            velocity += moveDirection * speed * dt;
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

    private void UpdatePlayer(Planet planet, float dt)
    {
        switch (state)
        {
            case PlayerState.Idle:
            case PlayerState.Walk:
            case PlayerState.Run:
            case PlayerState.Sprint:
            case PlayerState.Jump:
            case PlayerState.Fall:
            case PlayerState.Leap:
                UpdateGroundedStates(planet, dt);
                break;

            case PlayerState.Fly:
                UpdateFlyState(planet);
                break;
        }

        // Update transform
        transform.position = new Vector3(position.x, position.y, 0);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    private void UpdateGroundedStates(Planet planet, float dt)
    {
        // Calculate orientation relative to planet
        Vector2 toPlanet = planet.position - position;
        rotation = Mathf.Atan2(-toPlanet.x, toPlanet.y) * Mathf.Rad2Deg;

        float dist = toPlanet.magnitude;
        float edge = planet.radius + _data.PLAYER_RADIUS;

        // Apply gravity
        if (dist > edge)
        {
            Vector2 normal = toPlanet.normalized;
            velocity += dt * planet.gravity * normal;
        }

        // Update position
        position += velocity * dt;

        // Ground collision check
        toPlanet = position - planet.position;
        dist = toPlanet.magnitude;

        if (dist < edge + _data.PLAYER_ON_GROUND_THRESHOLD)
        {
            HandleGroundCollision(planet, edge, toPlanet, dist);
        }
        else
        {
            onGround = false;
        }

        // Apply friction
        ApplyFriction(planet, dist, dt);

        // Update state based on velocity
        if (onGround)
        {
            UpdateGroundedState();
        }
        else if (state != PlayerState.Leap)
        {
            UpdateAirborneState(toPlanet);
        }
    }

    private void UpdateFlyState(Planet planet)
    {
        position += velocity * Time.deltaTime;

        Vector2 toPlanet = position - planet.position;
        float dist = toPlanet.magnitude;
        float edge = planet.radius + _data.PLAYER_RADIUS;

        if (dist < edge + _data.PLAYER_ON_GROUND_THRESHOLD)
        {
            state = PlayerState.Idle;
            onGround = true;
            velocity = Vector2.zero;
        }
    }

    private void HandleGroundCollision(Planet planet, float edge, Vector2 toPlanet, float dist)
    {
        Vector2 normal = toPlanet / dist;
        position = planet.position + normal * edge;
        lastNormal = normal;

        float vel = Vector2.Dot(velocity, normal);
        if (vel < 0)
        {
            velocity -= vel * normal;
        }

        onGround = true;
    }

    private void ApplyFriction(Planet planet, float dist, float dt)
    {
        float t = Mathf.Clamp01((dist - planet.radius) / planet.radius);
        float friction = Mathf.Lerp(
            onGround ? _data.PLAYER_FRICTION_GROUND : _data.PLAYER_FRICTION_AIR,
            1,
            t
        );

        velocity *= 1 / (1 + friction * dt);
    }

    private void UpdateGroundedState()
    {
        float mag = velocity.magnitude;

        if (mag < _data.PLAYER_IDLE_VEL_THRESHOLD)
        {
            SetState(PlayerState.Idle);
        }
        else if (mag > _data.PLAYER_RUN_VEL_THRESHOLD)
        {
            SetState(sprinting && charge >= 1 ? PlayerState.Sprint : PlayerState.Run);
        }
        else
        {
            SetState(PlayerState.Walk);
        }

        UpdateCharge(mag);
    }

    private void UpdateCharge(float velocityMagnitude)
    {
        float chargeChange = 0;
        if (velocityMagnitude > _data.PLAYER_CHARGE_VEL_THRESHOLD)
        {
            chargeChange = _data.PLAYER_CHARGE_RATE * Time.deltaTime;
        }
        else if (charge < 1)
        {
            chargeChange = -_data.PLAYER_DISCHARGE_RATE * Time.deltaTime;
        }

        charge = Mathf.Clamp(charge + chargeChange, 0, 1);
    }

    private void UpdateAirborneState(Vector2 toPlanet)
    {
        if (toPlanet.magnitude > 0)
        {
            Vector2 normal = toPlanet.normalized;
            float vel = Vector2.Dot(velocity, normal);
            SetState(vel > 0 ? PlayerState.Jump : PlayerState.Fall);
        }
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facing < 0;
        }
    }

    private void EnterLeapState()
    {
        SetState(PlayerState.Leap);
        charge -= 1;
    }

    private void EnterFlyState(Planet planet)
    {
        SetState(PlayerState.Fly);
        onGround = false;
        Vector2 toPlanet = position - planet.position;
        direction = toPlanet.normalized;
        charge -= 1;
    }

    private void ApplyMovement(Planet planet, float x, bool jumped, float speed, float height, float dt)
    {
        Vector2 toPlanet = position - planet.position;
        Vector2 normal = toPlanet.normalized;

        if (x != 0)
        {
            Vector2 moveDir = x > 0 ? new Vector2(normal.y, -normal.x) : new Vector2(-normal.y, normal.x);
            velocity += moveDir * speed * dt;
        }

        if (jumped)
        {
            velocity += normal * height;
        }
    }

    public void SetState(PlayerState newState)
    {
        if (state == newState) return;

        state = newState;
        animations[(int)newState].elapsed = 0;
        currentAnimation = animations[(int)newState];
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // Draw velocity vector
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)velocity);

        // Draw ground normal
        if (onGround)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)lastNormal);
        }

        // Draw player radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _data.PLAYER_RADIUS);
    }
}