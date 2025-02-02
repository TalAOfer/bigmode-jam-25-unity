using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Bellflower : FlowerSocketSequencer
{
    [Range(0, 3)]
    [SerializeField] private int index;
    [SerializeField, ReadOnly] private readonly List<string> notes = new() { "C", "D", "B", "G" };
    [SerializeField] private readonly List<Color> noteColors = new List<Color> { Color.red, Color.green, Color.blue, Color.cyan };
    [SerializeField] private ParticleSystem particles;
    public string CurrentNote => notes[index];
    public override IEnumerator StartSequence()
    {
        index++;
        if (index == 4)
        {
            index = 0;
        }

        PlayNote();
        EmitParticle();

        yield return null;
    }

    public void PlayNote()
    {
        string noteString = $"World interaction/Musical puzzle/{CurrentNote} note";
        GameManager.Instance.audioController.PlayOneShot(noteString);
    }

    public void EmitParticle()
    {
        if (particles != null)
        {
            ParticleSystem.MainModule module = particles.main;
            module.startColor = noteColors[index];
            particles.Play();
        }

    }
}
