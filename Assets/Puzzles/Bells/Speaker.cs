using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class Speaker : MonoBehaviour
{
    [Range(0, 3)]
    [SerializeField] private int index;
    [SerializeField] private NotesAndColors notes;
    [SerializeField] private float noteDelay = 2f;
    [SerializeField] ParticleSystem particles;
    public string CurrentNote => notes.value[index].letter;

    private float timeElapsed;

    public void Awake()
    {
        if (particles != null)
        {
            var main = particles.main;
            main.startColor = notes.value[index].color;
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
