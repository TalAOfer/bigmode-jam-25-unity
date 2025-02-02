using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DecorationManager : MonoBehaviour
{
    [SerializeField] private bool isScatterFolder;

    [SerializeField] private Planet parentPlanet;
    [SerializeField] private float baseDecorationHeight = 1f;
    [SerializeField] private DecorationLayers layerName;
    [SerializeField] private int orderInLayer;

    [HideIf("isScatterFolder")]
    [SerializeField] private GameObject decorationPrefab;
    [HideIf("isScatterFolder")]
    [SerializeField] private Sprite decorationSprite; 

    [ShowIf("isScatterFolder")]
    [SerializeField] private GameObject scatterPrefab;

#if UNITY_EDITOR

    [Button("Spawn Decoration")]
    public void SpawnDecoration()
    {
        GameObject prefab = isScatterFolder ? scatterPrefab : decorationPrefab;
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.SetParent(transform, false);

        if (!isScatterFolder)
        {
            instance.name = decorationSprite.name;
            instance.GetComponentInChildren<SpriteRenderer>().sprite = decorationSprite;
        }

        Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
        Selection.activeGameObject = instance;

        RescaleDecorations();
    }

#endif


    [Button]
    public void RescaleDecorations()
    {
        if (parentPlanet == null)
        {
            parentPlanet = GetComponentInParent<Planet>();
        }

        gameObject.name = isScatterFolder ? "Scatter Folder" : layerName.ToString() + " " + orderInLayer.ToString();

        List<Decoration> decorations = GetComponentsInChildren<Decoration>().ToList();

        float planetScale = parentPlanet.transform.localScale.x;
        foreach (var decoration in decorations)
        {
            decoration.transform.localPosition = new(0, baseDecorationHeight, 0);
            decoration.transform.localScale = Vector3.one * (1f / planetScale);
            decoration.SR.sortingLayerName = "Decoration_" + layerName.ToString();
            decoration.SR.sortingOrder = orderInLayer;
        }
    }

    public enum DecorationLayers
    {
        BG,
        FG,
    }
}
