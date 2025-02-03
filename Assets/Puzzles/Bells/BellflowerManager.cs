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
    [SerializeField] private List<Speaker> speakers;

    public override IEnumerator StartSequence()
    {
        GameManager.Instance.audioController.PlayOneShot("World interaction/Plug in");

        if (planet.completed)
        {
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

        List<string> currentSequence = new List<string>();

        yield return CameraController.Instance.WatchPlanetRoutine(planet);

        for (int i = 0; i < bellFlowers.Count; i++)
        {
            currentSequence.Add(bellFlowers[i].CurrentNote);
            bellFlowers[i].PlayNote();
            yield return new WaitForSeconds(noteDelay);
        }

        bool areEqual = currentSequence.SequenceEqual(correctSequence);

        if (areEqual)
        {
            foreach(Speaker speaker in speakers)
            {
                speaker.gameObject.SetActive(false);
            }

            GameManager.Instance.audioController.PlayOneShot("World interaction/Musical puzzle/Music Puzzle jingle");
            yield return new WaitForSeconds(jingleDuration);

            yield return OnPlanetComplete();
        } 
        
        else
        {
            yield return CameraController.Instance.GoBackToPlayerRoutine();
        }
    }
}
