using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SunManager : FlowerSocket
{
    [SerializeField] private int numberOfPuzzlesToWin = 2;
    public override IEnumerator StartSequence()
    {
        int puzzlesSolved = 0;
        GameManager.Instance.audioController.PlayOneShot("World interaction/Plug in");

        foreach (var planet in GameManager.Instance.galaxyManager.planets)
        {
            if (planet.completed) puzzlesSolved++;
        }

        if (numberOfPuzzlesToWin == puzzlesSolved)
        {
            GameManager.Instance.audioController.PlayOneShot("Music/Ending song");
            yield return CameraController.Instance.WatchPlanetRoutine(planet);
            yield return GameManager.Instance.screenFlash.TriggerSlowFlash();
            SceneManager.LoadScene(2);
        } 
        
        else
        {
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

    }
}
