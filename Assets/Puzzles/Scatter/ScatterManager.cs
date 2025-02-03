using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScatterManager : FlowerSocketSequencer
{
    public List<Collectible> collectibles = new();
    public List<Collectible> activeCollectibles = new();
    [ShowInInspector, ReadOnly] public int CollectiblesLeft => activeCollectibles.Count;
    [SerializeField] private float tweenTime;
    [SerializeField] private Ease tweenEase;
    [SerializeField] private float tweenDelay;
    [SerializeField] private TextMeshProUGUI countTMP;

    private bool hasScattered;
    private bool finished => !activeCollectibles.Any();
    private void Awake()
    {
        countTMP.gameObject.SetActive(false);
    }

    public void AssignCollectible(Collectible collectible)
    {
        collectibles.Add(collectible);
    }

    public override IEnumerator StartSequence()
    {
        GameManager.Instance.audioController.PlayOneShot("World interaction/Plug in");

        if (!hasScattered)
        {
            yield return ScatterSequence();
        }

        else
        {
            if (finished && !planet.completed)
            {
                countTMP.gameObject.SetActive(false);
                yield return OnPlanetComplete();
            }

            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void SetCountText()
    {
        int max = collectibles.Count;
        int got = collectibles.Count - activeCollectibles.Count;
        countTMP.text = got.ToString() + "/" + max.ToString();
    }

    public IEnumerator ScatterSequence()
    {
        hasScattered = true;
        activeCollectibles = new(collectibles);

        Sequence sequence = DOTween.Sequence();
        float currentDelay = 0f;

        SetCountText();
        countTMP.gameObject.SetActive(true);

        foreach (Collectible collectible in collectibles)
        {
            sequence.Insert(currentDelay, DOTween.Sequence()
            .OnStart(() =>
            {
                collectible.gameObject.SetActive(true);
                collectible.transform.SetParent(collectible.originalParent, true);
                GameManager.Instance.audioController.PlayOneShot("Player/Jump");
                SetCountText();

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
        SetCountText();

        if (activeCollectibles.Any())
        {
            GameManager.Instance.screenFlash.TriggerFlash();
        }
    }


}
