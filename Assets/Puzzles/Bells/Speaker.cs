using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class Speaker : MonoBehaviour
{
    [Range(0, 3)]
    [SerializeField] private int index;
    [SerializeField, ReadOnly] private readonly List<string> notes = new() { "C", "D", "B", "G" };
    [SerializeField] private readonly List<Color> noteColors = new List<Color> { Color.red, Color.green, Color.blue, Color.cyan };
    [SerializeField] private float noteDelay = 0.5f;
    [SerializeField] ParticleSystem particles;
    public string CurrentNote => notes[index];

    private float timeElapsed;

    public void Awake()
    {
        if (particles != null)
        {
            var main = particles.main;
            main.startColor = noteColors[index];
        }

        Debug.Log(gameObject.name);
    }

    public void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > noteDelay)
        {
            timeElapsed = 0;
            PlayNote();
        }
    }

    private void PlayNote()
    {
        string noteString = $"World interaction/Musical puzzle/{CurrentNote} note";
        GameManager.Instance.audioController.PlayOneShotInPosition(noteString, transform.position);
    }
}
