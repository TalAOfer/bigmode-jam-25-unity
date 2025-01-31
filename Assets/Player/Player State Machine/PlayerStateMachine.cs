public class PlayerStateMachine
{
    public PlayerBaseState CurrentState;
    public void InitializeState(PlayerBaseState startState)
    {
        CurrentState = startState;
        CurrentState.OnEnterState();
    }

    public void ChangeState(PlayerBaseState newState)
    {
        if (newState == CurrentState) return;

        CurrentState.OnExitState();
        CurrentState = newState;
        CurrentState.OnEnterState();
        CurrentState.hasStartedTransition = false;
    }
}
