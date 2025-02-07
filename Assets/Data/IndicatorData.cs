using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName ="Indicator Data")]
public class IndicatorData : ScriptableObject
{
    public float radius = 5f;
    public float maxAlpha = 0.5f;
    public TweenData showFade = new(0.5f, Ease.Linear);
    public TweenData hideFade = new(0.5f, Ease.Linear);
}
