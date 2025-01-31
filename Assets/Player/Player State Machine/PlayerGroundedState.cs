using UnityEngine;

public abstract class PlayerGroundedState : PlayerBaseState
{
    protected PlayerGroundedState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (player.Input.NormInputY > 0)
        {
            stateMachine.ChangeState(player.AirborneState);
        }

        ApplyPlanetPhysics();
    }
    
    protected void UpdateStateAccordingToSpeed()
    {
        float speed = player.velocity.magnitude;

        if (speed > player.Data.SPRINT_THRESHOLD)
        {
            stateMachine.ChangeState(player.SprintState);
        }
        else if (speed > player.Data.RUN_THRESHOLD)
        {
            stateMachine.ChangeState(player.RunState);
        }
        else if (speed > player.Data.WALK_THRESHOLD)
        {
            stateMachine.ChangeState(player.WalkState);
        }
        else
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    protected void AccelerateToMaxSpeed()
    {
        float xInput = player.Input.NormInputX;
        bool sprintPressed = player.Input.SprintInput;

        float dt = Time.deltaTime;

        Vector2 normal = player.Normal;

        // Calculate move direction
        Vector2 moveDir = xInput > 0 ?
            new Vector2(normal.y, -normal.x) :
            new Vector2(-normal.y, normal.x);

        // Calculate acceleration based on sprint
        float acceleration = sprintPressed ?
            player.Data.PLAYER_SPRINT_ACCELERATION :
            player.Data.PLAYER_NORMAL_ACCELERATION;

        // Apply acceleration
        player.velocity += acceleration * dt * moveDir;

        // Update facing
        Vector2 tangent = new Vector2(-normal.y, normal.x);
        float tv = Vector2.Dot(player.velocity, tangent);
        player.facing = tv < 0 ? 1 : -1;

        float maxSpeed = GetMaxSpeed(sprintPressed);
        if (player.velocity.magnitude > maxSpeed)
        {
            player.velocity = player.velocity.normalized * maxSpeed;
        }
    }

    protected void Decelerate()
    {
        player.velocity = Vector2.Lerp(player.velocity, Vector2.zero, Time.deltaTime * player.Data.PLAYER_DECELERATION);
    }

    protected float GetMaxSpeed(bool isSprinting)
    {
        if (isSprinting)
        {
            return player.Data.MAX_SPRINT_SPEED;
        }

        else
        {
            return player.Data.MAX_WALK_SPEED;
        }
    }

    
}
