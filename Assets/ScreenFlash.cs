using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public float startAlpha;
    public Image whiteImage;
    public Image puzzleCompleteText;
    public float fadeDuration;
    public Ease fadeEase;
    public float slowFadeTime;
    public Ease slowFadeEase;

    private void Awake()
    {
        whiteImage.color = Color.white;
        whiteImage.DOFade(0, 1.5f);
    }

    [Button]
    public void TriggerFlash()
    {
        Color color = Color.white;
        color.a = startAlpha;
        whiteImage.color = color;
        whiteImage.DOFade(0, fadeDuration).SetEase(fadeEase);
    }

    public IEnumerator TriggerSlowFlash()
    {
        yield return whiteImage.DOFade(1, slowFadeTime).SetEase(slowFadeEase).WaitForCompletion();
    }

    public void TriggerPuzzleComplete()
    {
        Color color = Color.white;
        color.a = startAlpha;
        puzzleCompleteText.color = color;
        puzzleCompleteText.DOFade(0, fadeDuration).SetEase(fadeEase);
    }
}
