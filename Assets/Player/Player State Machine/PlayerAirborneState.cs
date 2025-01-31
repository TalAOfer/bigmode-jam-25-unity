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

        player.velocity += player.Normal * player.Data.PLAYER_JUMP_HEIGHT;

        ApplyPlanetPhysics();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        //if (player.IsGrounded())
        //{
        //    stateMachine.ChangeState(player.IdleState);
        //}

        UpdateAnimatorAccordingToVerticalVelocity();
    }

    private void UpdateAnimatorAccordingToVerticalVelocity()
    {
        float verticalVelocity = Vector2.Dot(player.velocity, player.Normal);
        player.Anim.SetFloat("VerticalVelocity", verticalVelocity);
    }
}
