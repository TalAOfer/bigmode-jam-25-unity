using UnityEngine;

public class Collectible : MonoBehaviour
{
    public Transform originalParent;
    public ScatterManager manager;

    private void Awake()
    {
        manager = FindFirstObjectByType<ScatterManager>();
        manager.AssignCollectible(this);
        originalParent = transform.parent;
        transform.SetParent(manager.transform);
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
}
