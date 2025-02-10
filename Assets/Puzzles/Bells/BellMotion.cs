using Sirenix.OdinInspector;
using UnityEngine;

public class BellMotion : MonoBehaviour
{

    [FoldoutGroup("Bell Swing"), SerializeField] private float frequency = 5f;
    [FoldoutGroup("Bell Swing"), SerializeField] private float amplitude = 5f;
    [FoldoutGroup("Bell Swing"), SerializeField] private float stoppingDeadzone = 0.01f;
    private float startTime = -1;
    private bool isRinging = false;

    [Button]
    public void Ring()
    {
        startTime = Time.time;
        isRinging = true;
    }

    [Button]
    public void Stop()
    {
        isRinging = false;
        transform.localEulerAngles = Vector3.zero; // Reset to zero
    }

    void Update()
    {
        if (!isRinging) return;

        float timeSinceStart = Time.time - startTime;
        transform.localEulerAngles = new Vector3(0, 0,
            amplitude * Mathf.Sin(timeSinceStart * frequency) * Mathf.Exp(-timeSinceStart)
        );

        // Auto-stop when amplitude gets very small
        if (Mathf.Exp(-timeSinceStart) < stoppingDeadzone)
        {
            Stop();
        }
    }
}
