using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DecorationManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private Planet parentPlanet;
    [SerializeField, ReadOnly] List<Decoration> decorations;

    [SerializeField] private DecorationType decorationType;
    public DecorationType Type => decorationType;
    [SerializeField] private bool overrideFolderName;
    [SerializeField] private bool shouldSpin;
    [ShowIf("shouldSpin"), SerializeField] private float spinSpeed;
    [ShowIf("overrideFolderName"), SerializeField] private string nameOverride;

    [SerializeField] private float baseDecorationHeight = 1f;
    [SerializeField] private DecorationLayers layerName;
    [SerializeField] private int orderInLayer;

    private bool IsInteractable => decorationType == DecorationType.Interactable;
    [HideIf("IsInteractable")]
    [SerializeField] private GameObject decorationPrefab;
    [HideIf("IsInteractable")]
    [SerializeField] private Sprite decorationSprite; 

    [ShowIf("IsInteractable")]
    [SerializeField] private GameObject interactablePrefab;


#if UNITY_EDITOR

    [Button("Spawn Decoration")]
    public void SpawnDecoration()
    {
        GameObject prefab = IsInteractable ? interactablePrefab : decorationPrefab;
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.SetParent(transform, false);

        if (!IsInteractable)
        {
            instance.name = decorationSprite.name;
            instance.GetComponentInChildren<SpriteRenderer>().sprite = decorationSprite;
        }

        Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
        Selection.activeGameObject = instance;

        RescaleDecorations();
    }

#endif

    public void Initialize(Planet planet)
    {
        parentPlanet = planet;
        decorations = GetComponentsInChildren<Decoration>().ToList();
    }

    public void InitializeMaterial(Material material)
    {
        foreach (Decoration decoration in decorations)
        {
            decoration.SR.material = material;
        }
    }


    private void Update()
    {
        if (shouldSpin)
        {
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
        }
    }

    [Button]
    public void RescaleDecorations()
    {
        if (parentPlanet == null)
        {
            parentPlanet = GetComponentInParent<Planet>();
        }

        gameObject.name = IsInteractable ? interactablePrefab.name + " Folder" : layerName.ToString() + " " + orderInLayer.ToString();
        if (overrideFolderName) gameObject.name = nameOverride;

        List<Decoration> decorations = GetComponentsInChildren<Decoration>().ToList();

        float planetScale = parentPlanet.transform.localScale.x;
        foreach (var decoration in decorations)
        {
            decoration.transform.localPosition = new(0, baseDecorationHeight, 0);
            decoration.transform.localScale = Vector3.one * (1f / planetScale);
            decoration.SR.sortingLayerName = "Decoration_" + layerName.ToString();
            decoration.SR.sortingOrder = orderInLayer;
            decoration.transform.parent.gameObject.name = decoration.SR.sprite.name;
        }
    }

    public enum DecorationLayers
    {
        BG,
        FG,
    }

    public enum DecorationType
    {
        OnGround,
        Atmoshperic,
        Interactable
    }
}
