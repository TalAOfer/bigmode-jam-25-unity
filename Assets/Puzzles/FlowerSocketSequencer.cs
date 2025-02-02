using System.Collections;
using UnityEngine;

public abstract class FlowerSocketSequencer : MonoBehaviour
{
    public abstract IEnumerator StartSequence();
}
