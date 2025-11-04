using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MazeWallStyler : MonoBehaviour
{
    [Header("Wall Appearance")]
    public Material wallMaterial;
    public Color wallColor = Color.gray;
    
    [Header("Wall Dimensions")]
    public Vector3 wallScale = new Vector3(0.9f, 1f, 0.9f);
    public float wallHeight = 1f;
    
    [Header("Target Walls")]
    [Tooltip("Apply styling to walls under this parent (leave empty to affect all walls in scene)")]
    public Transform targetParent;
    
    [Tooltip("Only affect walls with names containing this text")]
    public string wallNameFilter = "Wall";

    [ContextMenu("Apply Material to Walls")]
    public void ApplyMaterialToWalls()
    {
        if (wallMaterial == null)
        {
            Debug.LogWarning("No material assigned!");
            return;
        }

        GameObject[] walls = FindWalls();
        int count = 0;

        foreach (GameObject wall in walls)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = wallMaterial;
                count++;
            }
        }

        Debug.Log($"Applied material to {count} walls");
    }

    [ContextMenu("Apply Color to Walls")]
    public void ApplyColorToWalls()
    {
        GameObject[] walls = FindWalls();
        int count = 0;

        foreach (GameObject wall in walls)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = wallColor;
                count++;
            }
        }

        Debug.Log($"Applied color to {count} walls");
    }

    [ContextMenu("Apply Scale to Walls")]
    public void ApplyScaleToWalls()
    {
        GameObject[] walls = FindWalls();
        int count = 0;

        foreach (GameObject wall in walls)
        {
            wall.transform.localScale = wallScale;
            
            // Adjust Y position to keep walls on ground
            Vector3 pos = wall.transform.position;
            pos.y = wallHeight * 0.5f;
            wall.transform.position = pos;
            
            count++;
        }

        Debug.Log($"Applied scale to {count} walls");
    }

    [ContextMenu("Apply All Styling")]
    public void ApplyAllStyling()
    {
        ApplyScaleToWalls();
        
        if (wallMaterial != null)
        {
            ApplyMaterialToWalls();
        }
        else
        {
            ApplyColorToWalls();
        }
        
        Debug.Log("All styling applied!");
    }

    [ContextMenu("Add Colliders to Walls")]
    public void AddCollidersToWalls()
    {
        GameObject[] walls = FindWalls();
        int count = 0;

        foreach (GameObject wall in walls)
        {
            if (wall.GetComponent<Collider>() == null)
            {
                wall.AddComponent<BoxCollider>();
                count++;
            }
        }

        Debug.Log($"Added colliders to {count} walls");
    }

    [ContextMenu("Add Wall Tag")]
    public void AddWallTag()
    {
        // Check if Wall tag exists
        try
        {
            GameObject.FindGameObjectWithTag("Wall");
        }
        catch
        {
            Debug.LogWarning("'Wall' tag doesn't exist. Please add it in Edit → Project Settings → Tags and Layers");
            return;
        }

        GameObject[] walls = FindWalls();
        int count = 0;

        foreach (GameObject wall in walls)
        {
            wall.tag = "Wall";
            count++;
        }

        Debug.Log($"Tagged {count} walls with 'Wall' tag");
    }

    [ContextMenu("Make Walls Static")]
    public void MakeWallsStatic()
    {
        GameObject[] walls = FindWalls();
        int count = 0;

        foreach (GameObject wall in walls)
        {
            wall.isStatic = true;
            count++;
        }

        Debug.Log($"Made {count} walls static (for baking/optimization)");
    }

    [ContextMenu("Copy Style from Selected Wall")]
    public void CopyStyleFromSelected()
    {
#if UNITY_EDITOR
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Please select a wall first!");
            return;
        }

        GameObject selectedWall = Selection.activeGameObject;
        
        // Copy scale
        wallScale = selectedWall.transform.localScale;
        wallHeight = selectedWall.transform.localScale.y;
        
        // Copy material
        Renderer renderer = selectedWall.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial != null)
        {
            wallMaterial = renderer.sharedMaterial;
            wallColor = renderer.sharedMaterial.color;
        }

        Debug.Log($"Copied style from '{selectedWall.name}':");
        Debug.Log($"  Scale: {wallScale}");
        Debug.Log($"  Material: {wallMaterial?.name}");
        Debug.Log($"  Color: {wallColor}");
#endif
    }

    private GameObject[] FindWalls()
    {
        GameObject[] allObjects;
        
        if (targetParent != null)
        {
            // Find walls under specific parent
            int childCount = targetParent.childCount;
            System.Collections.Generic.List<GameObject> childWalls = new System.Collections.Generic.List<GameObject>();
            
            for (int i = 0; i < childCount; i++)
            {
                Transform child = targetParent.GetChild(i);
                if (string.IsNullOrEmpty(wallNameFilter) || child.name.Contains(wallNameFilter))
                {
                    childWalls.Add(child.gameObject);
                }
            }
            
            return childWalls.ToArray();
        }
        else
        {
            // Find all walls in scene
            allObjects = FindObjectsOfType<GameObject>();
            System.Collections.Generic.List<GameObject> walls = new System.Collections.Generic.List<GameObject>();
            
            foreach (GameObject obj in allObjects)
            {
                if (string.IsNullOrEmpty(wallNameFilter) || obj.name.Contains(wallNameFilter))
                {
                    walls.Add(obj);
                }
            }
            
            return walls.ToArray();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MazeWallStyler))]
public class MazeWallStylerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MazeWallStyler styler = (MazeWallStyler)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("📋 Copy Style from Selected Wall", GUILayout.Height(30)))
        {
            styler.CopyStyleFromSelected();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Apply Styling", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("✨ Apply All Styling", GUILayout.Height(40)))
        {
            styler.ApplyAllStyling();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Apply Material", GUILayout.Height(30)))
        {
            styler.ApplyMaterialToWalls();
        }
        if (GUILayout.Button("Apply Color", GUILayout.Height(30)))
        {
            styler.ApplyColorToWalls();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Apply Scale", GUILayout.Height(30)))
        {
            styler.ApplyScaleToWalls();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Additional Settings", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Colliders", GUILayout.Height(30)))
        {
            styler.AddCollidersToWalls();
        }

        if (GUILayout.Button("Add 'Wall' Tag", GUILayout.Height(25)))
        {
            styler.AddWallTag();
        }

        if (GUILayout.Button("Make Static (for baking)", GUILayout.Height(25)))
        {
            styler.MakeWallsStatic();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "This tool lets you style all walls at once!\n\n" +
            "Quick workflow:\n" +
            "1. Select one of your manually-created walls\n" +
            "2. Click 'Copy Style from Selected Wall'\n" +
            "3. Set 'Target Parent' to 'GeneratedWalls'\n" +
            "4. Click 'Apply All Styling'",
            MessageType.Info);
    }
}
#endif
