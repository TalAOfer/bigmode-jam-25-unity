using System.Collections;
using UnityEngine;

public class SunManager : FlowerSocketSequencer
{
    [SerializeField] private int numberOfPuzzlesToWin = 2;
    public override IEnumerator StartSequence()
    {
        int puzzlesSolved = 0;

        foreach (var planet in GameManager.Instance.galaxyManager.planets)
        {
            if (planet.completed) puzzlesSolved++;
        }

        if (numberOfPuzzlesToWin == puzzlesSolved)
        {
            Debug.Log("you won");
            yield break;
        }
    }
}
