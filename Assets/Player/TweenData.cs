using DG.Tweening;

[System.Serializable]
public class TweenData
{
    public TweenData(float duration, Ease ease)
    {
        this.duration = duration;
        this.ease = ease;
    }

    public float duration;
    public Ease ease;
}