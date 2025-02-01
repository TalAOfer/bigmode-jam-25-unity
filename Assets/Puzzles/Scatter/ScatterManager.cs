using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScatterManager : MonoBehaviour
{
    public List<Collectible> collectibles = new();
    public List<Collectible> activeCollectibles = new();
    [SerializeField] private float tweenTime;
    [SerializeField] private Ease tweenEase;
    [SerializeField] private float tweenDelay;

    public void AssignCollectible(Collectible collectible)
    {
        collectibles.Add(collectible);
    }

    [Button]
    public void ActivateScatter()
    {
        activeCollectibles = new(collectibles);

        Sequence sequence = DOTween.Sequence();
        float currentDelay = 0f;

        foreach (Collectible collectible in collectibles)
        {
            sequence.Insert(currentDelay, DOTween.Sequence()
            .OnStart(() => {
                collectible.gameObject.SetActive(true);
                collectible.transform.SetParent(collectible.originalParent, true);
            })
            .Append(collectible.transform.DOLocalMove(Vector3.zero, tweenTime).SetEase(tweenEase)));
            currentDelay += tweenDelay;
        }
    } 

    public void OnColllectibleCollected(Collectible collectible)
    {
        activeCollectibles.Remove(collectible);
        if (activeCollectibles.Count == 0)
        {
            Debug.Log("collected all");
        }
    }
}
