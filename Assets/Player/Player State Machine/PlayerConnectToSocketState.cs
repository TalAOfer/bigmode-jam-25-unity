using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerConnectToSocketState : PlayerBaseState
{
    public PlayerConnectToSocketState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        player.velocity = Vector3.zero;
        player.transform.position = player.CurrentFlowerSocket.transform.position - (player.transform.up * player.Data.PLAYER_SOCKET_PADDING);
        player.StartCoroutine(SocketSequence());
    }

    private IEnumerator SocketSequence()
    {
        yield return player.CurrentFlowerSocket.StartSequence();

        player.OnFlowerSocketTriggerExit();

        ChangeState(player.AirborneState);
    }
}
