
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlanetDirectionIndicator : MonoBehaviour
{
    [ReadOnly]
    public Planet planet;
    public SpriteRenderer sr;

    private Tween tween;

    public void Initialize(Planet planet)
    {
        this.planet = planet;
        sr.sprite = planet.indicatorSprite;
    }

    private void OnDisable()
    {
        tween.Complete();
    }

    public void ShowIndicator(float speed, float alpha)
    {
        if (tween != null && !tween.IsComplete()) 
        {
            tween.Complete();
            tween.Kill();
        }

        tween = sr.DOFade(alpha, speed);
    }

    public void HideIndicator(float speed)
    {
        if (tween != null && !tween.IsComplete())
        {
            tween.Complete();
        }

        tween = sr.DOFade(0, speed);
    }

    public void UpdateIndicator()
    {
        transform.up = planet.transform.position - transform.position;
    }
}