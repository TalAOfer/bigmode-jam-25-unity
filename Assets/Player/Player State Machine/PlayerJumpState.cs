using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        GameManager.Instance.audioController.PlayOneShot("Player/Jump");

        // Apply jump force in the correct "up" direction
        player.velocity += (Vector2)player.transform.up * player.Data.PLAYER_JUMP_HEIGHT;

        ChangeState(player.AirborneState);
    }
}
