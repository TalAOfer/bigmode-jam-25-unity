using System.Collections;
using UnityEngine;

public abstract class FlowerSocketSequencer : MonoBehaviour
{
    [SerializeField] protected Planet planet;
    public abstract IEnumerator StartSequence();

    public IEnumerator OnPlanetComplete()
    {
        yield return planet.OnPlanetComplete();
    }
}
