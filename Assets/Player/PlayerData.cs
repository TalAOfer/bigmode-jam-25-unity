using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName ="Player Data")]
public class PlayerData : ScriptableObject
{
    public float PLAYER_RADIUS = 16.0f;
    public float PLAYER_JUMP_HEIGHT = 9f;
    public float PLAYER_LEAP_HEIGHT = 24f;
    public float PLAYER_FLY_SPEED = 32f;
    public float PLAYER_FLY_HANDLING = 0.05f;
    public float PLAYER_CHARGE_RATE = 0.5f;
    public float PLAYER_DISCHARGE_RATE = 0.3f;
    public float PLAYER_FRICTION_AIR = 2f;
    public float PLAYER_ON_GROUND_THRESHOLD = 2.0f;

    

    [Title("Grounded")]
    public float PLAYER_FRICTION_GROUND = 2f;

    public float WALK_THRESHOLD = 2f;
    public float MAX_WALK_SPEED = 4f;

    public float RUN_THRESHOLD = 5f;
    
    public float SPRINT_THRESHOLD = 8f;
    public float MAX_SPRINT_SPEED = 12f;

    [Title("Grouded", "Acceleration")]
    public float PLAYER_NORMAL_ACCELERATION = 30f;
    public float PLAYER_SPRINT_ACCELERATION = 60f;
    public float PLAYER_DECELERATION = 8f;

}
