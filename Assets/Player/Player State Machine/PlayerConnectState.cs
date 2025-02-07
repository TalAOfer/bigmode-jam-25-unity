using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerConnectState : PlayerBaseState
{
    public PlayerConnectState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        player.velocity = Vector3.zero;
        FlowerSocket flowerSocket = player.CurrentFlowerSocket;

        HandleFlip(flowerSocket);

        if (flowerSocket.DockingPoint == null)
        {
            Debug.Log(flowerSocket + " is missing a docking point. Please assign");
            return;
        }

        player.StartCoroutine(ConnectionRoutine());
    }

    private IEnumerator ConnectionRoutine()
    {
        yield return player.transform
            .DOMove(player.CurrentFlowerSocket.DockingPoint.position, player.Data.PLAYER_SOCKET_CONNECTION_SPEED)
            .SetSpeedBased()
            .SetEase(player.Data.PLAYER_SOCKET_CONNECTION_EASE)
            .WaitForCompletion();

        ChangeState(player.PluggedState);
    }

    private void HandleFlip(FlowerSocket flowerSocket)
    {
        Vector2 toFlower = (flowerSocket.DockingPoint.transform.position - player.transform.position).normalized;
        float dot = Vector2.Dot(player.transform.right, toFlower);

        if (Mathf.Abs(dot) > player.Data.PLAYER_SOCKET_CONNECTION_FLIP_DEADZONE)
        {
            bool isToTheRight = dot > 0;
            player.ManuallyUpdateFacingDirection(isToTheRight);
        }
    }
}
