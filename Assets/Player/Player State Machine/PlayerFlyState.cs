using UnityEngine;

public class PlayerFlyState : PlayerBaseState
{
    public PlayerFlyState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        player.socketCollider.enabled = false;
        GameManager.Instance.backgroundManager.GoToSpaceBG();
        GameManager.Instance.audioController.PlayOneShot("Player/Fly Jump");
        CameraController.Instance.SetCameraState(CameraController.CameraState.Fly);
        GameManager.Instance.planetDirectionIndicatorManager.ShowIndicators();
    }

    public override void OnExitState()
    {
        base.OnExitState();
        player.socketCollider.enabled = true;
        GameManager.Instance.backgroundManager.GoToPlanetSkybox();
        GameManager.Instance.audioController.PlayOneShot("Player/Fly Land");
        CameraController.Instance.SetCameraState(CameraController.CameraState.Land);
        GameManager.Instance.planetDirectionIndicatorManager.HideIndicators();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (timeSinceEntered > 0.5f && player.IsGrounded())
        {
            ChangeState(player.IdleState);
        }

        player.velocity = (Vector2)player.transform.up * player.Data.PLAYER_FLY_SPEED;
        HandleHorizontalInput();
    }

    private void HandleHorizontalInput()
    {
        float xInput = player.Input.NormInputX;
        if (xInput != 0)
        {
            player.rotation -= xInput * player.Data.PLAYER_FLY_HANDLING * Time.deltaTime;
            player.facing = xInput > 0 ? -1 : 1;
        }
    }
}
