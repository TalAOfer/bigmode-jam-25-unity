using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public float startAlpha;
    public Image whiteImage;
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
}
