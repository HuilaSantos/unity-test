using UnityEngine;
using UnityEditor;

public class MazeFromImageGenerator : MonoBehaviour
{
    [Header("Maze Configuration")]
    [Tooltip("Reference texture showing the maze pattern (black = walls, white = paths)")]
    public Texture2D mazeTexture;
    
    [Tooltip("Prefab or primitive to use for wall cubes")]
    public GameObject wallPrefab;
    
    [Tooltip("Size of each maze cell")]
    public float cellSize = 1f;
    
    [Header("Wall Detection")]
    [Tooltip("Brightness threshold: pixels darker than this are considered walls (0-1)")]
    [Range(0f, 1f)]
    public float wallThreshold = 0.5f;
    
    [Tooltip("Ignore colored pixels (like red solution lines)")]
    public bool ignoreColoredPixels = true;
    
    [Tooltip("How colorful a pixel must be to be ignored (saturation threshold)")]
    [Range(0f, 1f)]
    public float colorSaturationThreshold = 0.3f;
    
    [Tooltip("Parent transform to organize generated walls")]
    public Transform wallsParent;
    
    [Header("Generation Options")]
    [Tooltip("Only generate walls that don't exist yet (checks for existing colliders)")]
    public bool skipExistingWalls = true;
    
    [Tooltip("Check distance from existing walls")]
    public float existingWallCheckRadius = 0.3f;

    [Tooltip("Start position offset for the maze")]
    public Vector3 startOffset = Vector3.zero;
    
    [Header("Preview")]
    [Tooltip("Show gizmos for where walls will be placed")]
    public bool showPreview = true;
    
    [Tooltip("Preview color for new walls")]
    public Color previewColor = Color.red;

    private bool[,] wallMap;
    private int mazeWidth;
    private int mazeHeight;

    [ContextMenu("Generate Maze from Image")]
    public void GenerateMazeFromImage()
    {
        if (mazeTexture == null)
        {
            Debug.LogError("Maze texture is not assigned!");
            return;
        }

        if (wallPrefab == null)
        {
            Debug.LogWarning("No wall prefab assigned. Will create cubes.");
        }

        if (wallsParent == null)
        {
            GameObject wallsObj = new GameObject("GeneratedWalls");
            wallsParent = wallsObj.transform;
            wallsParent.SetParent(transform);
        }

        // Read the maze from the texture
        ReadMazeFromTexture();

        // Generate walls
        int wallsCreated = 0;
        int wallsSkipped = 0;

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (wallMap[x, y])
                {
                    Vector3 position = GetWorldPosition(x, y);
                    
                    // Check if wall already exists at this position
                    if (skipExistingWalls && IsWallAtPosition(position))
                    {
                        wallsSkipped++;
                        continue;
                    }

                    // Create wall
                    GameObject wall = CreateWall(position);
                    wall.name = $"Wall_{x}_{y}";
                    wall.transform.SetParent(wallsParent);
                    wallsCreated++;
                }
            }
        }

        Debug.Log($"Maze generation complete! Created {wallsCreated} walls. Skipped {wallsSkipped} existing walls.");
    }

    private void ReadMazeFromTexture()
    {
        // Ensure we have a readable texture (handles non-readable import settings)
        Texture2D tex = GetReadableTexture(mazeTexture);

        mazeWidth = tex.width;
        mazeHeight = tex.height;
        wallMap = new bool[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                Color pixel = tex.GetPixel(x, y);
                
                // Check if this is a colored pixel (like a red solution line)
                if (ignoreColoredPixels && IsColoredPixel(pixel))
                {
                    // Colored pixels are treated as paths (not walls)
                    wallMap[x, y] = false;
                    continue;
                }
                
                float brightness = pixel.grayscale;
                
                // Dark pixels = walls, light pixels = paths
                wallMap[x, y] = brightness < wallThreshold;
            }
        }

        Debug.Log($"Read maze texture: {mazeWidth}x{mazeHeight}. Detected walls: {CountWalls()} cells");
    }

    private int CountWalls()
    {
        int count = 0;
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (wallMap[x, y]) count++;
            }
        }
        return count;
    }

    private bool IsColoredPixel(Color pixel)
    {
        // Calculate saturation (how colorful vs grayscale the pixel is)
        // HSV saturation: (max - min) / max
        float max = Mathf.Max(pixel.r, pixel.g, pixel.b);
        float min = Mathf.Min(pixel.r, pixel.g, pixel.b);
        
        if (max == 0f)
            return false; // Black pixel, not colored
        
        float saturation = (max - min) / max;
        
        // If saturation is above threshold, it's a colored pixel
        // (Red, blue, green lines will have high saturation)
        return saturation > colorSaturationThreshold;
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return startOffset + new Vector3(x * cellSize, 0, y * cellSize);
    }

    private bool IsWallAtPosition(Vector3 position)
    {
        // Check if there's already a collider at this position
        Collider[] colliders = Physics.OverlapSphere(position + Vector3.up * 0.5f, existingWallCheckRadius);
        
        foreach (Collider col in colliders)
        {
            if (col.gameObject.name.Contains("Wall") || col.gameObject.CompareTag("Wall"))
            {
                return true;
            }
        }
        
        return false;
    }

    private GameObject CreateWall(Vector3 position)
    {
        GameObject wall;
        
        if (wallPrefab != null)
        {
            wall = Instantiate(wallPrefab, position, Quaternion.identity);
        }
        else
        {
            // Create a default cube
            wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.position = position + Vector3.up * 0.5f; // Center the cube
            wall.transform.localScale = new Vector3(cellSize * 0.9f, 1f, cellSize * 0.9f);
        }

        return wall;
    }

    // Create a readable copy of the given texture if it's not readable
    private Texture2D GetReadableTexture(Texture2D source)
    {
        if (source == null)
        {
            Debug.LogError("MazeFromImageGenerator: Source texture is null");
            return null;
        }

        if (source.isReadable)
        {
            return source; // Already readable
        }

        // Copy GPU texture to a temporary RenderTexture, then read back to a new readable Texture2D
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
        Graphics.Blit(source, rt);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readable = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        readable.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readable.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        Debug.Log("MazeFromImageGenerator: Created a temporary readable copy of the maze texture (import setting was not readable). Consider enabling Read/Write in import settings for better editor workflow.");

        return readable;
    }

    [ContextMenu("Clear Generated Walls")]
    public void ClearGeneratedWalls()
    {
        if (wallsParent != null)
        {
            int childCount = wallsParent.childCount;
            
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(wallsParent.GetChild(i).gameObject);
            }
            
            Debug.Log($"Cleared {childCount} generated walls.");
        }
    }

    [ContextMenu("Analyze Current Scene")]
    public void AnalyzeCurrentScene()
    {
        GameObject[] allWalls = GameObject.FindObjectsOfType<GameObject>();
        int wallCount = 0;
        
        foreach (GameObject obj in allWalls)
        {
            if (obj.name.Contains("Wall"))
            {
                wallCount++;
            }
        }
        
        Debug.Log($"Found {wallCount} objects with 'Wall' in their name in the scene.");
    }

    private void OnDrawGizmos()
    {
        if (!showPreview || mazeTexture == null || wallMap == null)
            return;

        Gizmos.color = previewColor;

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (wallMap[x, y])
                {
                    Vector3 position = GetWorldPosition(x, y);
                    
                    if (skipExistingWalls && IsWallAtPosition(position))
                        continue;
                    
                    Gizmos.DrawWireCube(position + Vector3.up * 0.5f, new Vector3(cellSize * 0.9f, 1f, cellSize * 0.9f));
                }
            }
        }
    }
}
