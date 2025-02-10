using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class DecorationInitializer
{
    static DecorationInitializer()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            var decorationParents = Object.FindObjectsByType<Planet>(FindObjectsSortMode.None);
            foreach (var planet in decorationParents)
            {
                planet.InitializeDecorations();
            }
        }
    }
}
