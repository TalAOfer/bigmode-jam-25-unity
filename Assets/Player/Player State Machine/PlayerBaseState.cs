using UnityEngine;

public abstract class PlayerBaseState
{
    public Player player;
    public PlayerStateMachine stateMachine;
    public string animBoolName;
    public bool hasStartedTransition;
    protected float timeSinceEntered;
    public PlayerBaseState(Player player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;

    }

    public virtual void OnEnterState()
    {
        hasStartedTransition = false;
        timeSinceEntered = 0;
        player.PlayerStateName = this.GetType().Name;
        player.Anim.SetBool(animBoolName, true);
    }

    public virtual void OnExitState()
    {
        player.Anim.SetBool(animBoolName, false);
    }

    public virtual void UpdateState() 
    {
        timeSinceEntered += Time.deltaTime;
    }

    public void ChangeState(PlayerBaseState newState)
    {
        if (hasStartedTransition || stateMachine.CurrentState == newState) return;

        hasStartedTransition = true;
        stateMachine.ChangeState(newState);
    }

    public void ApplyPlanetPhysics()
    {
        float dt = Time.deltaTime;
        Vector2 toPlanet = player.ToPlanet;

        // Update rotation
        player.rotation = Mathf.Atan2(-toPlanet.x, toPlanet.y) * Mathf.Rad2Deg;

        float dist = toPlanet.magnitude;
        float edge = player.currentPlanet.radius + player.Data.PLAYER_RADIUS;


        // Apply gravity
        if (dist > edge)
        {
            Vector2 normal = toPlanet.normalized;
            player.velocity += dt * player.currentPlanet.gravity * -normal;
        }

        player.ApplyFriction(player.currentPlanet, dist, dt);
    }

}
