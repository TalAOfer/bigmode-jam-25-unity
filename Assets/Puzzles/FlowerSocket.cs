using System.Collections;
using UnityEngine;

public class FlowerSocket : MonoBehaviour
{
    public FlowerSocketSequencer sequencer;

    public IEnumerator StartSequence()
    {
        yield return sequencer.StartSequence();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSocket"))
        {
            Player player = collision.transform.GetComponentInParent<Player>();
            player.OnFlowerSocketTriggerEnter(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSocket"))
        {
            Player player = collision.transform.GetComponentInParent<Player>();
            player.OnFlowerSocketTriggerExit();
        }
    }
}
