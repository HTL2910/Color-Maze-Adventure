using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    private Level level;

    public override void OnInspectorGUI()
    {
        level = (Level)target;

        // Default Inspector
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("== Level Image Tools ==", EditorStyles.boldLabel);

        if (GUILayout.Button("Load All Images + Copy To Level"))
        {
            level.allImages = level.LoadAllImages();
            level.CopyFromAllImages();
            Debug.Log($"✅ Loaded {level.allImages.Count} images and copied index {level.startIndex} to {level.endIndex}.");
            EditorUtility.SetDirty(level);
        }

        if (GUILayout.Button("Paste Level Images to AllImages"))
        {
            level.PasteToAllImages();
            Debug.Log("✅ Pasted levelImages to allImages.");
            EditorUtility.SetDirty(level);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(level);
        }
    }
}
