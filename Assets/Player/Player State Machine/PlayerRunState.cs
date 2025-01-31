using UnityEngine;

public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        CameraController.Instance.SetCameraState(CameraController.CameraState.Normal);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (player.Input.NormInputX != 0)
        {
            AccelerateToMaxSpeed();
            UpdateStateAccordingToSpeed();
        }

        else
        {
            Decelerate();
            UpdateStateAccordingToSpeed();
        }
    }
}
