using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData _data;
    [SerializeField] private PlayerInputController _input;

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
    public PlayerAnimation[] animations;
    public PlayerAnimation currentAnimation;

    private static float prevX = 0;
    private static float prevY = 0;

    void Awake()
    {
        animations = new PlayerAnimation[(int)PlayerState.Count];
        for (int i = 0; i < (int)PlayerState.Count; i++)
        {
            animations[i] = new PlayerAnimation();
        }
        currentAnimation = animations[0];
    }

    private void Update()
    {
        ProcessInput(GameManager.Instance.planets[planetIdx], _input.NormInputX, _input.NormInputY, _input.SprintInput, Time.deltaTime);
        UpdatePlayer(GameManager.Instance.planets[planetIdx], Time.deltaTime);
    }

    public void GetClosestPlanet(List<Planet> planets)
    {
        float closest = float.MaxValue;

        for (int i = 0; i < planets.Count; i++)
        {
            Vector2 diff = planets[i].position - position;
            float dist = diff.magnitude;
            float edge = dist - (planets[i].radius + _data.PLAYER_RADIUS);

            if (edge < closest)
            {
                closest = edge;
                planetIdx = i;
            }
        }
    }

    public void ProcessInput(Planet planet, float x, float y, bool sprinting, float dt)
    {
        switch (state)
        {
            case PlayerState.Idle:
            case PlayerState.Walk:
            case PlayerState.Run:
            case PlayerState.Sprint:
            case PlayerState.Jump:
            case PlayerState.Fall:
                {
                    bool moved = x != 0;
                    bool jumped = y > 0 && onGround;
                    bool leaped = jumped && sprinting && charge >= 1;

                    float speed = _data.PLAYER_WALK_SPEED;
                    float height = _data.PLAYER_JUMP_HEIGHT;

                    float mag = velocity.magnitude;

                    if (moved)
                    {
                        Vector2 toPlanet = position - planet.position;
                        float dist = toPlanet.magnitude;
                        Vector2 normal = toPlanet.normalized;
                        Vector2 tangent = new Vector2(-normal.y, normal.x);
                        float tv = Vector2.Dot(velocity, tangent);
                        facing = tv > 0 ? 1 : -1;
                    }

                    if (!moved && onGround && mag < _data.PLAYER_IDLE_VEL_THRESHOLD)
                    {
                        velocity = Vector2.zero;
                    }

                    if (onGround)
                    {
                        this.sprinting = sprinting;
                    }

                    if (this.sprinting)
                    {
                        speed = _data.PLAYER_RUN_SPEED;
                        if (charge >= 1)
                        {
                            speed = _data.PLAYER_CHARGED_SPEED;
                        }
                    }

                    if (leaped)
                    {
                        height = _data.PLAYER_LEAP_HEIGHT;
                    }

                    if (jumped && leaped)
                    {
                        state = PlayerState.Leap;
                        charge -= 1;
                    }

                    if (moved || jumped)
                    {
                        Vector2 toPlanet = position - planet.position;
                        float dist = toPlanet.magnitude;
                        Vector2 normal = toPlanet.normalized;

                        if (x > 0)
                        {
                            velocity += new Vector2(-normal.y, normal.x) * speed * dt;
                        }
                        if (x < 0)
                        {
                            velocity += new Vector2(normal.y, -normal.x) * speed * dt;
                        }

                        if (jumped)
                        {
                            velocity += normal * height;
                        }
                    }

                    if (jumped && leaped && !moved)
                    {
                        state = PlayerState.Fly;
                        onGround = false;

                        Vector2 toPlanet = position - planet.position;
                        direction = toPlanet.normalized;
                    }
                }
                break;

            case PlayerState.Leap:
                {
                    float speed = _data.PLAYER_CHARGED_SPEED;
                    Vector2 toPlanet = position - planet.position;
                    float dist = toPlanet.magnitude;
                    Vector2 normal = toPlanet.normalized;

                    if (prevX > 0)
                    {
                        velocity += new Vector2(-normal.y, normal.x) * speed * dt;
                    }
                    if (prevX < 0)
                    {
                        velocity += new Vector2(normal.y, -normal.x) * speed * dt;
                    }

                    x = prevX;
                    y = prevY;
                }
                break;

            case PlayerState.Fly:
                {
                    velocity = direction * _data.PLAYER_FLY_SPEED;
                    bool moved = x != 0 || y != 0;

                    if (!moved) return;

                    if (x == 0) x = 1 - Mathf.Abs(y);
                    if (y == 0) y = 1 - Mathf.Abs(x);

                    float rads = rotation * Mathf.Deg2Rad;
                    float c = Mathf.Cos(rads);
                    float s = Mathf.Sin(rads);

                    Vector2 target = new Vector2(
                        x * c - y * s,
                        x * s + y * c
                    ).normalized;

                    direction = Vector2.Lerp(direction, target, _data.PLAYER_FLY_HANDLING);

                    if (direction.magnitude != 0)
                    {
                        direction = direction.normalized;
                    }
                    else
                    {
                        direction = Vector2.zero;
                    }
                }
                break;
        }

        prevX = x;
        prevY = y;
    }

    public void UpdatePlayer(Planet planet, float dt)
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
                {
                    Vector2 toPlanet = planet.position - position;
                    rotation = Mathf.Atan2(-toPlanet.x, toPlanet.y) * Mathf.Rad2Deg;

                    float dist = toPlanet.magnitude;
                    float edge = planet.radius + _data.PLAYER_RADIUS;

                    if (dist > 0)
                    {
                        if (dist >= edge)
                        {
                            Vector2 normal = toPlanet / dist;
                            velocity += dt * planet.gravity * normal;
                        }
                    }

                    position += velocity;
                    transform.position = new Vector3(position.x, position.y, 0);

                    toPlanet = position - planet.position;
                    dist = toPlanet.magnitude;

                    if (dist < edge + _data.PLAYER_ON_GROUND_THRESHOLD)
                    {
                        Vector2 normal = toPlanet / dist;
                        position = planet.position + normal * edge;

                        float vel = Vector2.Dot(velocity, normal);
                        if (vel < 0)
                        {
                            velocity -= vel * normal;
                        }

                        onGround = true;
                    }
                    else
                    {
                        onGround = false;
                    }

                    float t = Mathf.Clamp01((dist - planet.radius) / planet.radius);
                    float friction = Mathf.Lerp(onGround ? _data.PLAYER_FRICTION_GROUND : _data.PLAYER_FRICTION_AIR, 1, t);

                    velocity *= 1 / (1 + friction * dt);

                    if (onGround)
                    {
                        float mag = velocity.magnitude;

                        if (mag < _data.PLAYER_IDLE_VEL_THRESHOLD)
                        {
                            state = PlayerState.Idle;
                        }
                        else if (mag > _data.PLAYER_RUN_VEL_THRESHOLD)
                        {
                            state = sprinting && charge >= 1 ? PlayerState.Sprint : PlayerState.Run;
                        }
                        else
                        {
                            state = PlayerState.Walk;
                        }

                        float chargeChange = 0;
                        if (mag > _data.PLAYER_CHARGE_VEL_THRESHOLD)
                        {
                            chargeChange = _data.PLAYER_CHARGE_RATE * dt;
                        }
                        else if (charge < 1)
                        {
                            chargeChange = -_data.PLAYER_DISCHARGE_RATE * dt;
                        }

                        charge = Mathf.Clamp(charge + chargeChange, 0, 1);

                        if (dist > 0)
                        {
                            Vector2 normal = toPlanet.normalized;
                            position = planet.position + normal * edge;

                            float vel = Vector2.Dot(velocity, normal);
                            if (vel > 0)
                            {
                                velocity -= vel * normal;
                            }
                        }
                    }
                    else
                    {
                        if (state == PlayerState.Leap)
                        {
                            state = PlayerState.Leap;
                        }
                        else
                        {
                            if (dist > 0)
                            {
                                Vector2 normal = toPlanet.normalized;
                                float vel = Vector2.Dot(velocity, normal);
                                state = vel > 0 ? PlayerState.Jump : PlayerState.Fall;
                            }
                        }
                    }
                }
                break;

            case PlayerState.Fly:
                {
                    position += velocity;
                    transform.position = new Vector3(position.x, position.y, 0);

                    Vector2 toPlanet = planet.position - position;
                    float dist = toPlanet.magnitude;
                    float edge = planet.radius + _data.PLAYER_RADIUS;

                    toPlanet = position - planet.position;
                    dist = toPlanet.magnitude;

                    if (dist < edge + _data.PLAYER_ON_GROUND_THRESHOLD)
                    {
                        state = PlayerState.Idle;
                        onGround = true;
                        velocity = Vector2.zero;
                    }
                }
                break;
        }

        // Update the GameObject's rotation to match the calculated rotation
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public void SetAnimation(PlayerState newState)
    {
        animations[(int)newState].elapsed = 0;
        state = newState;
        currentAnimation = animations[(int)newState];
    }
}