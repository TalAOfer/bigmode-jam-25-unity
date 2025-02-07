using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerPluggedState : PlayerBaseState
{
    public PlayerPluggedState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        player.StartCoroutine(SocketSequence());
    }

    private IEnumerator SocketSequence()
    {
        yield return player.CurrentFlowerSocket.StartSequence();

        ChangeState(player.AirborneState);
    }
}
