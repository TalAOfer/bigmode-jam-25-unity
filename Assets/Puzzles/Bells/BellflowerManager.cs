using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BellflowerManager : FlowerSocketSequencer
{
    [SerializeField] private List<string> correctSequence = new() { "C", "D", "B", "G" };
    [SerializeField] private float jingleDuration = 1.5f;
    [SerializeField] private float noteDelay = 0.5f;
    [SerializeField] private List<Bellflower> bellFlowers;
    public override IEnumerator StartSequence()
    {
        List<string> currentSequence = new List<string>();

        for (int i = 0; i < bellFlowers.Count; i++)
        {
            currentSequence.Add(bellFlowers[i].CurrentNote);
            bellFlowers[i].PlayNote();
            yield return new WaitForSeconds(noteDelay);
        }

        bool areEqual = currentSequence.SequenceEqual(correctSequence);

        if (areEqual)
        {
            GameManager.Instance.audioController.PlayOneShot("World interaction/Musical puzzle/Music Puzzle jingle");
            yield return new WaitForSeconds(jingleDuration);

            yield return OnPlanetComplete();
        }
    }
}
