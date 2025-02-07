using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Image overlay;
    [SerializeField] private Image buffer;
    public void GoToScene()
    {
        StartCoroutine(Go());
    }

    public IEnumerator Go()
    {
        yield return overlay.DOFade(1f, 1f).WaitForCompletion();

        yield return Buffer();

        SceneManager.LoadScene(1);
    }

    public IEnumerator Buffer()
    {
        float elapsed = 0;
        while (elapsed < 10f)
        {
            elapsed += Time.deltaTime;
            buffer.fillAmount = elapsed / 10f;
            yield return null;
        }

        buffer.gameObject.SetActive(false);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
