using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public float startAlpha;
    public Image whiteImage;
    public Image puzzleCompleteText;
    public float fadeDuration;
    public Ease fadeEase;

    [Button]
    public void TriggerFlash()
    {
        Color color = Color.white;
        color.a = startAlpha;
        whiteImage.color = color;
        whiteImage.DOFade(0, fadeDuration).SetEase(fadeEase);
    }

    public void TriggerPuzzleComplete()
    {
        Color color = Color.white;
        color.a = startAlpha;
        puzzleCompleteText.color = color;
        puzzleCompleteText.DOFade(0, fadeDuration).SetEase(fadeEase);
    }
}
