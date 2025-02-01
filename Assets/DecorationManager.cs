using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecorationManager : MonoBehaviour
{
    [SerializeField] private Planet parentPlanet;
    [SerializeField] private float baseDecorationHeight = 1f;
    [SerializeField] private DecorationLayers layerName;
    [SerializeField] private int orderInLayer;


    [Button]
    public void UpdateDecorations()
    {
        if (parentPlanet == null)
        {
            parentPlanet = GetComponentInParent<Planet>();
        }

        List<Decoration> decorations = GetComponentsInChildren<Decoration>().ToList();

        float planetScale = parentPlanet.transform.localScale.x;
        foreach (var decoration in decorations)
        {
            decoration.transform.localPosition = new(0, baseDecorationHeight, 0);
            decoration.transform.localScale = Vector3.one * (1f /planetScale);
            decoration.SR.sortingLayerName = layerName.ToString();
            decoration.SR.sortingOrder = orderInLayer;
        }
    }

    public enum DecorationLayers
    {
        Decoration_BG,
        Decoration_FG,
    }
}
