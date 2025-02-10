using UnityEngine;

public class PlayerAirborneState : PlayerBaseState
{
    public PlayerAirborneState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        
        UpdateAnimatorAccordingToVerticalVelocity();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        ApplyPlanetPhysics();
        UpdateHorizontalMovement();
        player.UpdateFacingDirection();
        UpdateAnimatorAccordingToVerticalVelocity();

        if (timeSinceEntered > 0.15f && player.IsGrounded())
        {
            ChangeState(player.IdleState);
        }
    }

    private void UpdateHorizontalMovement()
    {
        float xInput = player.Input.NormInputX;

        if (xInput != 0)
        {
            Vector2 moveDir = xInput > 0 ? player.transform.right : -player.transform.right;
            player.velocity += Time.deltaTime * player.Data.MAX_WALK_SPEED * moveDir;
        }
    }

    public override void OnExitState()
    {
        base.OnExitState();
        GameManager.Instance.audioController.PlayOneShot("Player/Land");
    }

    private void UpdateAnimatorAccordingToVerticalVelocity()
    {
        float verticalVelocity = Vector2.Dot(player.velocity, player.Normal);
        player.Anim.SetFloat("VerticalVelocity", verticalVelocity);
    }
}
