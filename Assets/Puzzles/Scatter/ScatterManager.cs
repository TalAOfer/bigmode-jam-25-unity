using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScatterManager : FlowerSocketSequencer
{
    public List<Collectible> collectibles = new();
    public List<Collectible> activeCollectibles = new();
    [ShowInInspector, ReadOnly] public int CollectiblesLeft => activeCollectibles.Count;
    [SerializeField] private float tweenTime;
    [SerializeField] private Ease tweenEase;
    [SerializeField] private float tweenDelay;

    private bool hasScattered;
    private bool finished => activeCollectibles.Any();

    public void AssignCollectible(Collectible collectible)
    {
        collectibles.Add(collectible);
    }

    public override IEnumerator StartSequence()
    {
        if (!hasScattered)
        {
            yield return ScatterSequence();
        } 
        
        else
        {
            if (finished)
            {

            } 
            
            else
            {

            }
        }
    }

    public IEnumerator ScatterSequence()
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

        yield return sequence.WaitForCompletion();

        foreach (Collectible collectible in collectibles)
        {
            collectible.coll.enabled = true;
        }

        yield break;
    }


    [Button]
    public void ActivateScatter()
    {
        StartCoroutine(StartSequence());
    } 

    public void OnColllectibleCollected(Collectible collectible)
    {
        activeCollectibles.Remove(collectible);
        if (activeCollectibles.Any())
        {
            Debug.Log("collected all");
        }
    }

    
}
