
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        player.velocity = Vector3.zero;

        CameraController.Instance.SetCameraState(CameraController.CameraState.Normal);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (player.Input.JumpInput)
        {
            ChangeState(player.FlyState);
        }

        if (player.Input.NormInputX != 0)
        {
            AccelerateToMaxSpeed();
            UpdateStateAccordingToSpeed();
        }

        else if (player.velocity.magnitude != 0)
        {
            Decelerate();
            UpdateStateAccordingToSpeed();
        }
    }
}
