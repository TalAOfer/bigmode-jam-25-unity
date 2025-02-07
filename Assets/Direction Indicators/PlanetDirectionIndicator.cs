
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class PlanetDirectionIndicator : MonoBehaviour
{
    [ReadOnly]
    public Planet planet;
    public SpriteRenderer sr;

    private Tween tween;
    private bool isShown;

    public void Initialize(Planet planet, float radius)
    {
        this.planet = planet;
        sr.sprite = planet.indicatorSprite;
        gameObject.SetActive(planet.shouldShowIndicator);
        gameObject.name = planet.name + " Indicator";

        Vector3 pos = sr.transform.position;
        pos.y = radius;
        sr.transform.position = pos;
    }

    private void OnDisable()
    {
        if (tween != null && tween.active)
        {
            tween.Complete();
        }
    }

    public void ShowIndicator(TweenData fadeData, float alpha)
    {
        if (isShown) return;
        isShown = true;

        if (tween != null && tween.active) 
        {
            tween.Complete();
        }

        tween = sr.DOFade(alpha, fadeData.duration).SetEase(fadeData.ease);
    }

    public void HideIndicator(TweenData fadeData)
    {
        if (!isShown) return;
        isShown = false;

        if (tween != null && tween.active)
        {
            tween.Complete();
        }

        tween = sr.DOFade(0, fadeData.duration).SetEase(fadeData.ease);
    }

    public void UpdateIndicator()
    {
        transform.up = planet.transform.position - transform.position;
    }

}