using UnityEngine;

public class PlayerSprintState : PlayerGroundedState
{
    public PlayerSprintState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        CameraController.Instance.SetCameraState(CameraController.CameraState.Sprint);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (player.Input.NormInputX != 0 && player.Input.SprintInput)
        {
            AccelerateToMaxSpeed();
        } 
        
        else
        {
            Decelerate();
            UpdateStateAccordingToSpeed();
        }
    }
}
