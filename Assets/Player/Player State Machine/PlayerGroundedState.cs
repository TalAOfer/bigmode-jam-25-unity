using UnityEngine;

public abstract class PlayerGroundedState : PlayerBaseState
{
    protected PlayerGroundedState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        ApplyPlanetPhysics();
        player.HandleGroundCollision();
        player.UpdateFacingDirection();

        if (player.IsGrounded())
        {
            if (player.Input.NormInputY > 0)
            {
                ChangeState(player.JumpState);
            }
        }

        else
        {
            ChangeState(player.AirborneState);
        }


    }
    
    protected void UpdateStateAccordingToSpeed()
    {
        float speed = player.velocity.magnitude;

        if (speed > player.Data.SPRINT_THRESHOLD)
        {
            ChangeState(player.SprintState);
        }
        else if (speed > player.Data.RUN_THRESHOLD)
        {
            ChangeState(player.RunState);
        }
        else if (speed > player.Data.WALK_THRESHOLD)
        {
            ChangeState(player.WalkState);
        }
        else
        {
            ChangeState(player.IdleState);
        }
    }

    protected void AccelerateToMaxSpeed()
    {
        if (hasStartedTransition) return;

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

        float maxSpeed = GetMaxSpeed(sprintPressed);
        if (player.velocity.magnitude > maxSpeed)
        {
            player.velocity = player.velocity.normalized * maxSpeed;
        }
    }

    protected void Decelerate()
    {
        if (hasStartedTransition) return;

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
