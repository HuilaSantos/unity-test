using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MazeAnalyzer : MonoBehaviour
{
    [Header("Analysis Settings")]
    [Tooltip("Search for objects with this tag")]
    public string wallTag = "Wall";
    
    [Tooltip("Or search by name containing this text")]
    public string wallNameContains = "Wall";

    [ContextMenu("Analyze Existing Walls")]
    public void AnalyzeExistingWalls()
    {
        List<GameObject> walls = FindAllWalls();
        
        if (walls.Count == 0)
        {
            Debug.LogWarning("No walls found in the scene!");
            return;
        }

        Debug.Log($"=== MAZE ANALYSIS ===");
        Debug.Log($"Total walls found: {walls.Count}");
        
        // Find bounds
        Vector3 minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        
        List<Vector3> positions = new List<Vector3>();
        
        foreach (GameObject wall in walls)
        {
            Vector3 pos = wall.transform.position;
            positions.Add(pos);
            
            minPos = Vector3.Min(minPos, pos);
            maxPos = Vector3.Max(maxPos, pos);
        }

        Debug.Log($"Maze Bounds:");
        Debug.Log($"  Min: {minPos}");
        Debug.Log($"  Max: {maxPos}");
        Debug.Log($"  Size: {maxPos - minPos}");

        // Detect grid spacing
        float spacing = DetectGridSpacing(positions);
        Debug.Log($"Detected grid spacing: {spacing} units");

        // Count walls per row/column
        Dictionary<float, int> xCounts = new Dictionary<float, int>();
        Dictionary<float, int> zCounts = new Dictionary<float, int>();
        
        float tolerance = spacing * 0.1f;
        
        foreach (Vector3 pos in positions)
        {
            // Round to nearest grid position
            float roundedX = Mathf.Round(pos.x / spacing) * spacing;
            float roundedZ = Mathf.Round(pos.z / spacing) * spacing;
            
            if (!xCounts.ContainsKey(roundedX))
                xCounts[roundedX] = 0;
            if (!zCounts.ContainsKey(roundedZ))
                zCounts[roundedZ] = 0;
                
            xCounts[roundedX]++;
            zCounts[roundedZ]++;
        }

        Debug.Log($"Grid dimensions:");
        Debug.Log($"  X positions: {xCounts.Keys.Count} columns");
        Debug.Log($"  Z positions: {zCounts.Keys.Count} rows");

        // Suggest start offset
        Vector3 suggestedOffset = new Vector3(
            Mathf.Floor(minPos.x / spacing) * spacing,
            minPos.y,
            Mathf.Floor(minPos.z / spacing) * spacing
        );
        
        Debug.Log($"Suggested Start Offset: {suggestedOffset}");
        Debug.Log($"Suggested Cell Size: {spacing}");
        
        // Find first wall position
        GameObject firstWall = walls.OrderBy(w => w.transform.position.x)
                                   .ThenBy(w => w.transform.position.z)
                                   .First();
        Debug.Log($"First wall (lowest X,Z): '{firstWall.name}' at {firstWall.transform.position}");
        
        Debug.Log($"=== END ANALYSIS ===");
    }

    private List<GameObject> FindAllWalls()
    {
        List<GameObject> walls = new List<GameObject>();
        
        // Find by tag
        if (!string.IsNullOrEmpty(wallTag))
        {
            GameObject[] taggedWalls = GameObject.FindGameObjectsWithTag(wallTag);
            if (taggedWalls.Length > 0)
            {
                walls.AddRange(taggedWalls);
                return walls;
            }
        }

        // Find by name
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (!string.IsNullOrEmpty(wallNameContains) && 
                obj.name.Contains(wallNameContains))
            {
                walls.Add(obj);
            }
        }

        return walls;
    }

    private float DetectGridSpacing(List<Vector3> positions)
    {
        if (positions.Count < 2)
            return 1f;

        List<float> distances = new List<float>();

        // Sample some positions to find common distances
        int sampleSize = Mathf.Min(100, positions.Count);
        for (int i = 0; i < sampleSize; i++)
        {
            Vector3 pos1 = positions[i];
            
            // Find nearest neighbor
            float minDist = float.MaxValue;
            foreach (Vector3 pos2 in positions)
            {
                if (pos1 == pos2) continue;
                
                float dist = Vector3.Distance(pos1, pos2);
                if (dist < minDist && dist > 0.01f)
                {
                    minDist = dist;
                }
            }
            
            if (minDist < float.MaxValue)
            {
                distances.Add(minDist);
            }
        }

        if (distances.Count == 0)
            return 1f;

        // Return the most common distance (mode)
        distances.Sort();
        float median = distances[distances.Count / 2];
        
        // Round to nearest 0.1
        return Mathf.Round(median * 10f) / 10f;
    }

    [ContextMenu("List First 10 Walls")]
    public void ListFirstWalls()
    {
        List<GameObject> walls = FindAllWalls();
        
        if (walls.Count == 0)
        {
            Debug.LogWarning("No walls found!");
            return;
        }

        Debug.Log($"First 10 walls (sorted by position):");
        
        var sortedWalls = walls.OrderBy(w => w.transform.position.x)
                              .ThenBy(w => w.transform.position.z)
                              .Take(10);

        int index = 1;
        foreach (GameObject wall in sortedWalls)
        {
            Debug.Log($"  {index}. {wall.name} at {wall.transform.position}");
            index++;
        }
    }

    [ContextMenu("Create Visualization Spheres")]
    public void CreateVisualizationSpheres()
    {
        List<GameObject> walls = FindAllWalls();
        
        if (walls.Count == 0)
        {
            Debug.LogWarning("No walls found!");
            return;
        }

        GameObject vizParent = new GameObject("WallVisualization");
        vizParent.transform.SetParent(transform);

        foreach (GameObject wall in walls)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = wall.transform.position + Vector3.up * 2f;
            sphere.transform.localScale = Vector3.one * 0.3f;
            sphere.transform.SetParent(vizParent.transform);
            sphere.name = $"Viz_{wall.name}";
            
            // Remove collider
            DestroyImmediate(sphere.GetComponent<Collider>());
            
            // Add red material
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.red;
            sphere.GetComponent<Renderer>().material = mat;
        }

        Debug.Log($"Created {walls.Count} visualization spheres above wall positions");
    }

    [ContextMenu("Clear Visualization")]
    public void ClearVisualization()
    {
        Transform vizTransform = transform.Find("WallVisualization");
        if (vizTransform != null)
        {
            DestroyImmediate(vizTransform.gameObject);
            Debug.Log("Visualization cleared");
        }
        else
        {
            Debug.Log("No visualization found");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MazeAnalyzer))]
public class MazeAnalyzerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MazeAnalyzer analyzer = (MazeAnalyzer)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Analysis Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("🔍 Analyze Existing Walls", GUILayout.Height(35)))
        {
            analyzer.AnalyzeExistingWalls();
        }

        EditorGUILayout.Space(5);

        if (GUILayout.Button("📋 List First 10 Walls", GUILayout.Height(30)))
        {
            analyzer.ListFirstWalls();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Visualization", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Visualization Spheres", GUILayout.Height(30)))
        {
            analyzer.CreateVisualizationSpheres();
        }

        if (GUILayout.Button("Clear Visualization", GUILayout.Height(25)))
        {
            analyzer.ClearVisualization();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Use 'Analyze Existing Walls' to get:\n" +
            "• Total wall count\n" +
            "• Maze bounds and size\n" +
            "• Detected grid spacing\n" +
            "• Suggested offset and cell size\n\n" +
            "This info helps configure the Maze Generator!",
            MessageType.Info);
    }
}
#endif
