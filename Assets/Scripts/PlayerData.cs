using UnityEngine;

[CreateAssetMenu(menuName ="Player Data")]
public class PlayerData : ScriptableObject
{
    public float PLAYER_RADIUS = 16.0f;
    public float PLAYER_WALK_SPEED = 6f;
    public float PLAYER_RUN_SPEED = 18f;
    public float PLAYER_CHARGED_SPEED = 24f;
    public float PLAYER_JUMP_HEIGHT = 9f;
    public float PLAYER_LEAP_HEIGHT = 24f;
    public float PLAYER_FLY_SPEED = 32f;
    public float PLAYER_FLY_HANDLING = 0.05f;
    public float PLAYER_CHARGE_RATE = 0.5f;
    public float PLAYER_DISCHARGE_RATE = 0.3f;
    public float PLAYER_FRICTION_GROUND = 2f;
    public float PLAYER_FRICTION_AIR = 2f;
    public float PLAYER_IDLE_VEL_THRESHOLD = 0.3f;
    public float PLAYER_RUN_VEL_THRESHOLD = 4f;
    public float PLAYER_CHARGE_VEL_THRESHOLD = 4f;
    public float PLAYER_ON_GROUND_THRESHOLD = 2.0f;
}
