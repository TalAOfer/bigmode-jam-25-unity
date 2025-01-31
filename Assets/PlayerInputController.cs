using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 RawMovementInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool SprintInput { get; private set; }
    public void UseDashInput() => SprintInput = false;
    public bool JumpInput { get; private set; }
    public void UseJumpInput() => JumpInput = false;

    public void HandleMoveInput(InputAction.CallbackContext ctx)
    {
        RawMovementInput = ctx.ReadValue<Vector2>();
        NormInputX = Mathf.RoundToInt(RawMovementInput.x);
        NormInputY = Mathf.RoundToInt(RawMovementInput.y);
    }

    public void HandleSprintInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            SprintInput = true;
        }

        else if (ctx.canceled)
        {
            SprintInput = false;
        }
    }

    public void HandleJumpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            JumpInput = true;
        }

        else if (ctx.canceled)
        {
            JumpInput = false;
        }
    }

}