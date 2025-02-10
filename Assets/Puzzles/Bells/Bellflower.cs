using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Bellflower : MonoBehaviour
{
    [Range(0, 3)]
    [SerializeField] private int index;
    [SerializeField] private NotesAndColors notes;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private BellMotion bellMotion;

    public string CurrentNote => notes.value[index].letter;
    [SerializeField] private float bounceForce = 1.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSocket"))
        {
            Player player = collision.GetComponentInParent<Player>();
            float verticalVelocity = Vector3.Dot(player.velocity, player.transform.up);
            if (verticalVelocity >= 0)
            {
                OnHit(player);
            }
        }
    }

    

    public void OnHit(Player player)
    {
        ReflectVelocity(player);

        index = (index + 1) % 4;

        PlayNote();
    }

    private void ReflectVelocity(Player player)
    {
        Vector2 currentVelocity = player.velocity;
        Vector2 surfaceNormal = player.transform.up;
        Vector2 reflection = Vector2.Reflect(currentVelocity, surfaceNormal);
        player.velocity = reflection.normalized * bounceForce;
    }

    public void PlayNote()
    {
        string noteString = $"World interaction/Musical puzzle/{CurrentNote} note";
        GameManager.Instance.audioController.PlayOneShotInPosition(noteString, GameManager.Instance.player.transform.position);
        EmitParticle();
        bellMotion.Ring();
    }

    private void EmitParticle()
    {
        if (particles != null)
        {
            ParticleSystem.MainModule module = particles.main;
            module.startColor = notes.value[index].color;
            particles.Play();
        }
    }
}
