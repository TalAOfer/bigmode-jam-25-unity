using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Image overlay;
    public void GoToScene()
    {
        StartCoroutine(Go());
    }

    public IEnumerator Go()
    {
        yield return overlay.DOFade(1f, 1f).WaitForCompletion();
        SceneManager.LoadScene(1);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
