using EasyTextEffects;
using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private TextEffect effect;
    [SerializeField] private TextMeshProUGUI tmp;

    [FoldoutGroup("Test"), SerializeField] DialogueEntry autoplayExample;
    [FoldoutGroup("Test"), SerializeField] DialogueAsset dialogueExample;

    [SerializeField] float fadeoutDuration = 1.5f;

    private bool clickable;
    private bool playerClicked;

    private bool isAutoPlaying;

    [Button] public void TestAutoPlay() => AutoPlayText(autoplayExample.text, autoplayExample.playDuration);
    [Button] public void TestDialogue() => StartCoroutine(DialogueRoutine(dialogueExample.Value));

    public void AutoPlayText(string text, float durationUntilFadeout)
    {
        if (isAutoPlaying)
        {
            Debug.Log("Animation hasn't ended yet.");
            return;
        }

        isAutoPlaying = true;
        StartCoroutine(AutoPlayTextRoutine(text, durationUntilFadeout));
    }

    private IEnumerator AutoPlayTextRoutine(string text, float durationUntilFadeout)
    {
        tmp.text = text;
        tmp.gameObject.SetActive(true);

        yield return new WaitForSeconds(durationUntilFadeout);

        effect.StartManualEffect("fadeOut");

        yield return new WaitForSeconds(fadeoutDuration);

        effect.gameObject.SetActive(false);
        isAutoPlaying = false;
    }

    public IEnumerator DialogueRoutine(Dialogue dialogue)
    {
        clickable = false;

        foreach (DialogueEntry entry in dialogue.entries)
        {
            tmp.text = entry.text;
            tmp.gameObject.SetActive(true);

            yield return new WaitForSeconds(entry.playDuration);

            clickable = true;

            while (!playerClicked)
            {
                yield return null;
            }

            playerClicked = false;
            clickable = false;

            effect.StartManualEffect("fadeOut");

            yield return new WaitForSeconds(fadeoutDuration);

            tmp.gameObject.SetActive(false);
        }
    }

    public void OnPlayerInput()
    {
        if (!clickable || playerClicked) return;
        playerClicked = true;
    }
}
