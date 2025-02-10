using UnityEngine;

public class PlayerChargeFlyState : PlayerBaseState
{
    public PlayerChargeFlyState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (timeSinceEntered > player.Data.PLAYER_FLY_CHARGE_DURATION)
        {
            ChangeState(player.FlyState);
        }

        else if (!player.Input.FlyInput)
        {
            ChangeState(player.IdleState);
        }
    }
}
