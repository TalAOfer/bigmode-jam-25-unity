using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName ="Player Data")]
public class PlayerData : ScriptableObject
{
    public float PLAYER_RADIUS = 16.0f;
    public float PLAYER_ON_GROUND_THRESHOLD = 2.0f;

    [FoldoutGroup("Socket")]
    public float PLAYER_SOCKET_DEFAULT_CONNECTION_TIME = 0.5f;
    [FoldoutGroup("Socket")]
    public float PLAYER_SOCKET_SHAKE;
    [FoldoutGroup("Socket")]
    public float PLAYER_SOCKET_CONNECTION_SPEED;
    [FoldoutGroup("Socket")]
    public Ease PLAYER_SOCKET_CONNECTION_EASE;
    [FoldoutGroup("Socket")]
    public float PLAYER_SOCKET_CONNECTION_FLIP_DEADZONE = 0.3f;


    [FoldoutGroup("Airborne")]
    public float PLAYER_FRICTION_AIR = 2f;
    [FoldoutGroup("Airborne"), Title("Jump")]
    public float PLAYER_JUMP_HEIGHT = 9f;

    [FoldoutGroup("Grounded")]
    [Title("Walk")]
    public float PLAYER_FRICTION_GROUND = 2f;
    [FoldoutGroup("Grounded")]
    public float WALK_THRESHOLD = 2f;
    [FoldoutGroup("Grounded")]
    public float MAX_WALK_SPEED = 4f;

    [Title("Run")]
    [FoldoutGroup("Grounded")]
    public float RUN_THRESHOLD = 5f;

    [Title("Sprint")]
    [FoldoutGroup("Grounded")]
    public float SPRINT_THRESHOLD = 8f;
    [FoldoutGroup("Grounded")]
    public float MAX_SPRINT_SPEED = 12f;

    [Title("Acceleration")]
    [FoldoutGroup("Grounded")]
    public float PLAYER_NORMAL_ACCELERATION = 30f;
    [FoldoutGroup("Grounded")]
    public float PLAYER_SPRINT_ACCELERATION = 60f;
    [FoldoutGroup("Grounded")]
    public float PLAYER_DECELERATION = 8f;

    [FoldoutGroup("Flying")]
    public float PLAYER_FLY_SPEED = 32f;
    [FoldoutGroup("Flying")]
    public float PLAYER_FLY_HANDLING = 0.05f;
    [FoldoutGroup("Flying")]
    public ScreenShakeProfile PLAYER_TAKEOFF_SHAKE;
    [FoldoutGroup("Flying")]
    public ScreenShakeProfile PLAYER_LAND_SHAKE;

}

[System.Serializable]
public class ScreenShakeProfile
{
    public ScreenShakeProfile(float amount, float duration)
    {
        this.amount = amount;
        this.duration = duration;
    }

    public float amount;
    public float duration;
}