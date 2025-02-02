using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{

    [SerializeField] private SpriteRenderer SpaceBG;
    [SerializeField] private SpriteRenderer PlanetBG;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Ease fadeEase;

    public void GoToSpaceBG()
    {
        PlanetBG.DOFade(0, fadeDuration).SetEase(fadeEase);
    }

    public void GoToPlanetSkybox()
    {
        Planet planet = GameManager.Instance.galaxyManager.planets[GameManager.Instance.player.planetIdx];
        Color color = planet.skyboxColor;
        color.a = 0f;
        PlanetBG.color = color;

        PlanetBG.DOFade(planet.skyboxColor.a, fadeDuration).SetEase(fadeEase);
    }
}
