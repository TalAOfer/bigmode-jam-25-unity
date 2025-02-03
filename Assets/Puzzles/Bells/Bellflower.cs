using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Bellflower : FlowerSocketSequencer
{
    [Range(0, 3)]
    [SerializeField] private int index;
    [SerializeField] private NotesAndColors notes;
    [SerializeField] private ParticleSystem particles;
    public string CurrentNote => notes.value[index].letter;
    public override IEnumerator StartSequence()
    {
        index++;
        if (index == 4)
        {
            index = 0;
        }

        PlayNote();

        yield return null;
    }

    public void PlayNote()
    {
        string noteString = $"World interaction/Musical puzzle/{CurrentNote} note";
        GameManager.Instance.audioController.PlayOneShotInPosition(noteString, GameManager.Instance.player.transform.position);
        EmitParticle();
    }

    public void EmitParticle()
    {
        if (particles != null)
        {
            ParticleSystem.MainModule module = particles.main;
            module.startColor = notes.value[index].color;
            particles.Play();
        }

    }
}
