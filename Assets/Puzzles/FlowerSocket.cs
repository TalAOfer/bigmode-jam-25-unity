using System.Collections;
using UnityEngine;

public abstract class FlowerSocket : MonoBehaviour
{
    [SerializeField] protected Planet planet;
    public Transform DockingPoint { get; private set; }
    public ParticleSystem ParticleSystem { get; private set; }


    private void OnEnable()
    {
        DockingPoint = transform.Find("Docking Point");
        
        if (DockingPoint == null )
        {
            Debug.LogError($"No docking point", gameObject);
            return;
        }

        Transform Particles = transform.Find("Particles");

        if ( Particles == null )
        {
            Debug.LogError($"No particle system", gameObject);
            return;
        }
        
        DockingPoint.gameObject.SetActive(false);
        
        ParticleSystem = Particles.GetComponent<ParticleSystem>();
    }

    public virtual IEnumerator StartSequence()
    {
        ParticleSystem.Stop(true);
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSocket"))
        {
            Player player = collision.transform.GetComponentInParent<Player>();
            ParticleSystem.Play();
            player.OnFlowerSocketTriggerEnter(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSocket"))
        {
            Player player = collision.transform.GetComponentInParent<Player>();
            ParticleSystem.Stop(true);
            player.OnFlowerSocketTriggerExit();
        }
    }

    public IEnumerator OnPlanetComplete()
    {
        yield return planet.OnPlanetComplete();
    }
}
