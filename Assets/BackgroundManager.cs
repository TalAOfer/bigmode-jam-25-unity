using DG.Tweening;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Ease fadeEase;
    private Tween fadeTween;

    public void FadePlanetSkybox(Planet planet, bool toVisible)
    {
        if (fadeTween != null && fadeTween.active)
        {
            fadeTween.Kill();
        }

        if (toVisible)
        {
            fadeTween = planet.BackgroundSR.DOFade(planet.BackgroundColor.a, fadeDuration).SetEase(fadeEase);
        }

        else
        {
            fadeTween = planet.BackgroundSR.DOFade(0, fadeDuration).SetEase(fadeEase);
        }
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
