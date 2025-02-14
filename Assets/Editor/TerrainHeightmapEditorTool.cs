using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainHeightmapLoader))]
public class TerrainHeightmapEditorTool : Editor 
{
    SerializedProperty heightmapTexture;
    SerializedProperty heightMultiplier;

    private void OnEnable()
    {
        heightmapTexture = serializedObject.FindProperty("heightmapTexture");
        heightMultiplier = serializedObject.FindProperty("heightMultiplier");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TerrainHeightmapLoader loader = (TerrainHeightmapLoader)target;

        // Draw default inspector properties
        EditorGUILayout.PropertyField(heightmapTexture);
        EditorGUILayout.PropertyField(heightMultiplier);

        // Add custom buttons
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Apply Heightmap"))
        {
            if (loader.GetComponent<Terrain>() != null && heightmapTexture.objectReferenceValue != null)
            {
                loader.ApplyHeightmap();
                EditorUtility.SetDirty(loader.gameObject);
            }
            else
            {
                Debug.LogError("Missing terrain component or heightmap texture!");
            }
        }

        if (GUILayout.Button("Reset Terrain"))
        {
            loader.ResetTerrain();
            EditorUtility.SetDirty(loader.gameObject);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
