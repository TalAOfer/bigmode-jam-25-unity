using UnityEngine;

public class Collectible : MonoBehaviour
{
    public Transform originalParent;
    public ScatterManager manager;
    public Collider2D coll;
    private void Awake()
    {
        manager = FindFirstObjectByType<ScatterManager>();
        manager.AssignCollectible(this);
        originalParent = transform.parent;
        transform.SetParent(manager.transform);
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerPickup"))
        {
            manager.OnColllectibleCollected(this);
            gameObject.SetActive(false);
        }
    }
}
