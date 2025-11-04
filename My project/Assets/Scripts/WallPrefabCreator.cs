using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper to quickly create a wall prefab from an existing wall
/// </summary>
public class WallPrefabCreator : MonoBehaviour
{
    [Header("Prefab Creation")]
    [Tooltip("Name for the new prefab")]
    public string prefabName = "MazeWall";
    
    [Tooltip("Selected wall to use as template")]
    public GameObject templateWall;

    [ContextMenu("Create Prefab from Template")]
    public void CreatePrefabFromTemplate()
    {
#if UNITY_EDITOR
        if (templateWall == null)
        {
            Debug.LogError("No template wall assigned! Select a wall first.");
            return;
        }

        // Create a clean copy
        GameObject prefabObject = Instantiate(templateWall);
        prefabObject.name = prefabName;
        
        // Remove any scene-specific components
        // Keep only essential components: Transform, MeshFilter, MeshRenderer, Collider
        
        // Save as prefab
        string path = $"Assets/Prefabs/{prefabName}.prefab";
        
        // Ensure Prefabs folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // Create the prefab
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(prefabObject, path);
        
        if (savedPrefab != null)
        {
            Debug.Log($"✅ Prefab created successfully at: {path}");
            Debug.Log($"You can now assign this prefab to MazeFromImageGenerator!");
            
            // Clean up the temporary object
            DestroyImmediate(prefabObject);
            
            // Select the new prefab
            Selection.activeObject = savedPrefab;
            EditorGUIUtility.PingObject(savedPrefab);
        }
        else
        {
            Debug.LogError("Failed to create prefab!");
            DestroyImmediate(prefabObject);
        }
#endif
    }

    [ContextMenu("Create Prefab from Selected Wall")]
    public void CreatePrefabFromSelection()
    {
#if UNITY_EDITOR
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("Please select a wall GameObject in the Hierarchy first!");
            return;
        }

        templateWall = Selection.activeGameObject;
        
        // Auto-name based on selected object
        if (string.IsNullOrEmpty(prefabName) || prefabName == "MazeWall")
        {
            prefabName = templateWall.name.Replace("(Clone)", "").Trim() + "_Prefab";
        }

        CreatePrefabFromTemplate();
#endif
    }

    [ContextMenu("Create Simple Cube Wall Prefab")]
    public void CreateSimpleCubeWallPrefab()
    {
#if UNITY_EDITOR
        // Create a simple cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "SimpleWall";
        
        // Set default scale
        cube.transform.localScale = new Vector3(0.9f, 1f, 0.9f);
        
        // Add material
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.3f, 0.3f, 0.3f); // Dark gray
        cube.GetComponent<Renderer>().material = mat;

        // Save as prefab
        string path = "Assets/Prefabs/SimpleWall.prefab";
        
        // Ensure Prefabs folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(cube, path);
        
        if (savedPrefab != null)
        {
            Debug.Log($"✅ Simple wall prefab created at: {path}");
            DestroyImmediate(cube);
            Selection.activeObject = savedPrefab;
            EditorGUIUtility.PingObject(savedPrefab);
        }
        else
        {
            Debug.LogError("Failed to create simple wall prefab!");
            DestroyImmediate(cube);
        }
#endif
    }

    [ContextMenu("List All Prefabs")]
    public void ListAllPrefabs()
    {
#if UNITY_EDITOR
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
        
        Debug.Log($"=== PREFABS IN PROJECT ===");
        Debug.Log($"Found {guids.Length} prefabs in Assets/Prefabs:");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                Debug.Log($"  • {prefab.name} ({path})");
            }
        }
        
        Debug.Log($"=== END PREFABS ===");
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(WallPrefabCreator))]
public class WallPrefabCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WallPrefabCreator creator = (WallPrefabCreator)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("✨ Create Prefab from Selected Wall", GUILayout.Height(40)))
        {
            creator.CreatePrefabFromSelection();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Create Prefab from Template", GUILayout.Height(30)))
        {
            creator.CreatePrefabFromTemplate();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Utilities", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Simple Cube Wall", GUILayout.Height(30)))
        {
            creator.CreateSimpleCubeWallPrefab();
        }

        if (GUILayout.Button("List All Prefabs", GUILayout.Height(25)))
        {
            creator.ListAllPrefabs();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "How to use:\n\n" +
            "1. Select one of your existing walls in Hierarchy\n" +
            "2. Click 'Create Prefab from Selected Wall'\n" +
            "3. Prefab will be saved to Assets/Prefabs/\n" +
            "4. Assign it to MazeFromImageGenerator's 'Wall Prefab' field\n\n" +
            "Or create a simple cube wall prefab for testing!",
            MessageType.Info);
    }
}
#endif
