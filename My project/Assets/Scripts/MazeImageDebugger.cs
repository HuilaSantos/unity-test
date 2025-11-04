using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Debug tool to verify how the maze image is being read
/// </summary>
public class MazeImageDebugger : MonoBehaviour
{
    [Header("Image to Test")]
    public Texture2D testTexture;
    
    [Header("Settings")]
    [Range(0f, 1f)]
    public float wallThreshold = 0.5f;
    
    [Tooltip("Detect colored pixels (like solution lines)")]
    public bool detectColoredPixels = true;
    
    [Range(0f, 1f)]
    public float colorSaturationThreshold = 0.3f;

    [ContextMenu("Analyze Image")]
    public void AnalyzeImage()
    {
        if (testTexture == null)
        {
            Debug.LogError("No texture assigned!");
            return;
        }

        Debug.Log($"=== IMAGE ANALYSIS ===");
        Debug.Log($"Image: {testTexture.name}");
        Debug.Log($"Size: {testTexture.width} × {testTexture.height} pixels");
        Debug.Log($"Format: {testTexture.format}");

        int wallPixels = 0;
        int pathPixels = 0;
        int coloredPixels = 0;
        float minBrightness = 1f;
        float maxBrightness = 0f;
        float totalBrightness = 0f;

        for (int x = 0; x < testTexture.width; x++)
        {
            for (int y = 0; y < testTexture.height; y++)
            {
                Color pixel = testTexture.GetPixel(x, y);
                float brightness = pixel.grayscale;
                
                totalBrightness += brightness;
                minBrightness = Mathf.Min(minBrightness, brightness);
                maxBrightness = Mathf.Max(maxBrightness, brightness);

                // Check if colored pixel
                if (detectColoredPixels && IsColoredPixel(pixel))
                {
                    coloredPixels++;
                    pathPixels++; // Colored pixels are treated as paths
                }
                else if (brightness < wallThreshold)
                {
                    wallPixels++;
                }
                else
                {
                    pathPixels++;
                }
            }
        }

        int totalPixels = testTexture.width * testTexture.height;
        float avgBrightness = totalBrightness / totalPixels;

        Debug.Log($"\nBrightness Stats:");
        Debug.Log($"  Min: {minBrightness:F3}");
        Debug.Log($"  Max: {maxBrightness:F3}");
        Debug.Log($"  Average: {avgBrightness:F3}");

        Debug.Log($"\nWith threshold {wallThreshold:F2}:");
        Debug.Log($"  Wall pixels: {wallPixels} ({(wallPixels * 100f / totalPixels):F1}%)");
        Debug.Log($"  Path pixels: {pathPixels} ({(pathPixels * 100f / totalPixels):F1}%)");
        
        if (detectColoredPixels && coloredPixels > 0)
        {
            Debug.Log($"  Colored pixels (ignored): {coloredPixels} ({(coloredPixels * 100f / totalPixels):F1}%)");
            Debug.Log($"  → These are likely solution lines or markers");
        }

        Debug.Log($"\nExpected maze:");
        Debug.Log($"  {wallPixels} walls will be generated");
        Debug.Log($"  Grid will be {testTexture.width}×{testTexture.height}");

        Debug.Log($"=== END ANALYSIS ===");
    }

    [ContextMenu("Show Corner Pixels")]
    public void ShowCornerPixels()
    {
        if (testTexture == null)
        {
            Debug.LogError("No texture assigned!");
            return;
        }

        Debug.Log($"=== CORNER PIXELS ===");
        
        // Bottom-left (0,0)
        Color bl = testTexture.GetPixel(0, 0);
        Debug.Log($"Bottom-Left (0,0): RGB({bl.r:F2},{bl.g:F2},{bl.b:F2}) Brightness:{bl.grayscale:F2} → {GetPixelType(bl)}");

        // Bottom-right
        Color br = testTexture.GetPixel(testTexture.width - 1, 0);
        Debug.Log($"Bottom-Right ({testTexture.width - 1},0): RGB({br.r:F2},{br.g:F2},{br.b:F2}) Brightness:{br.grayscale:F2} → {GetPixelType(br)}");

        // Top-left
        Color tl = testTexture.GetPixel(0, testTexture.height - 1);
        Debug.Log($"Top-Left (0,{testTexture.height - 1}): RGB({tl.r:F2},{tl.g:F2},{tl.b:F2}) Brightness:{tl.grayscale:F2} → {GetPixelType(tl)}");

        // Top-right
        Color tr = testTexture.GetPixel(testTexture.width - 1, testTexture.height - 1);
        Debug.Log($"Top-Right ({testTexture.width - 1},{testTexture.height - 1}): RGB({tr.r:F2},{tr.g:F2},{tr.b:F2}) Brightness:{tr.grayscale:F2} → {GetPixelType(tr)}");

        // Center
        Color center = testTexture.GetPixel(testTexture.width / 2, testTexture.height / 2);
        Debug.Log($"Center ({testTexture.width / 2},{testTexture.height / 2}): RGB({center.r:F2},{center.g:F2},{center.b:F2}) Brightness:{center.grayscale:F2} → {GetPixelType(center)}");

        Debug.Log($"=== END CORNERS ===");
    }

    [ContextMenu("Print First Row")]
    public void PrintFirstRow()
    {
        if (testTexture == null)
        {
            Debug.LogError("No texture assigned!");
            return;
        }

        Debug.Log($"=== FIRST ROW (Y=0) ===");
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        for (int x = 0; x < Mathf.Min(testTexture.width, 50); x++)
        {
            Color pixel = testTexture.GetPixel(x, 0);
            char c;
            
            if (detectColoredPixels && IsColoredPixel(pixel))
                c = '●'; // Colored pixel (solution line)
            else if (pixel.grayscale < wallThreshold)
                c = '█'; // Wall
            else
                c = '░'; // Path
                
            sb.Append(c);
        }
        
        Debug.Log(sb.ToString());
        Debug.Log($"(█ = wall, ░ = path, ● = colored/solution)");
        
        if (testTexture.width > 50)
        {
            Debug.Log($"(Showing first 50 of {testTexture.width} pixels)");
        }
        
        Debug.Log($"=== END FIRST ROW ===");
    }

    [ContextMenu("Generate Preview Texture")]
    public void GeneratePreviewTexture()
    {
        if (testTexture == null)
        {
            Debug.LogError("No texture assigned!");
            return;
        }

        // Create a new texture showing walls as red, paths as white
        Texture2D preview = new Texture2D(testTexture.width, testTexture.height);
        
        for (int x = 0; x < testTexture.width; x++)
        {
            for (int y = 0; y < testTexture.height; y++)
            {
                Color pixel = testTexture.GetPixel(x, y);
                Color previewColor;
                
                if (detectColoredPixels && IsColoredPixel(pixel))
                {
                    previewColor = Color.blue; // Blue = colored pixels (ignored)
                }
                else if (pixel.grayscale < wallThreshold)
                {
                    previewColor = Color.red; // Red = walls
                }
                else
                {
                    previewColor = Color.white; // White = paths
                }
                
                preview.SetPixel(x, y, previewColor);
            }
        }
        
        preview.Apply();

        // Save to file
        byte[] bytes = preview.EncodeToPNG();
        string path = Application.dataPath + "/Textures/MazePreview.png";
        System.IO.File.WriteAllBytes(path, bytes);
        
        Debug.Log($"Preview saved to: {path}");
        Debug.Log("Red = walls, White = paths, Blue = colored pixels (ignored)");
        
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        DestroyImmediate(preview);
    }

    [ContextMenu("Check Texture Import Settings")]
    public void CheckTextureImportSettings()
    {
#if UNITY_EDITOR
        if (testTexture == null)
        {
            Debug.LogError("No texture assigned!");
            return;
        }

        string path = AssetDatabase.GetAssetPath(testTexture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null)
        {
            Debug.LogError("Could not find texture importer!");
            return;
        }

        Debug.Log($"=== TEXTURE IMPORT SETTINGS ===");
        Debug.Log($"Path: {path}");
        Debug.Log($"Readable: {importer.isReadable}");
        Debug.Log($"Texture Type: {importer.textureType}");
        Debug.Log($"Format: {importer.textureCompression}");
        Debug.Log($"Max Size: {importer.maxTextureSize}");

        if (!importer.isReadable)
        {
            Debug.LogWarning("⚠️ Texture is NOT readable! Enable 'Read/Write Enabled' in import settings.");
            
            if (EditorUtility.DisplayDialog("Fix Texture Settings?",
                "The texture needs 'Read/Write Enabled' to work with the maze generator. Enable it now?",
                "Yes, Fix It", "No"))
            {
                importer.isReadable = true;
                AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();
                Debug.Log("✅ Texture is now readable!");
            }
        }
        else
        {
            Debug.Log("✅ Texture is readable - good to go!");
        }

        Debug.Log($"=== END SETTINGS ===");
#endif
    }

    private bool IsColoredPixel(Color pixel)
    {
        // Calculate saturation (how colorful vs grayscale the pixel is)
        float max = Mathf.Max(pixel.r, pixel.g, pixel.b);
        float min = Mathf.Min(pixel.r, pixel.g, pixel.b);
        
        if (max == 0f)
            return false; // Black pixel, not colored
        
        float saturation = (max - min) / max;
        
        // If saturation is above threshold, it's a colored pixel
        return saturation > colorSaturationThreshold;
    }

    private string GetPixelType(Color pixel)
    {
        if (detectColoredPixels && IsColoredPixel(pixel))
            return "COLORED (ignored)";
        else if (pixel.grayscale < wallThreshold)
            return "WALL";
        else
            return "PATH";
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MazeImageDebugger))]
public class MazeImageDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MazeImageDebugger debugger = (MazeImageDebugger)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Image Analysis", EditorStyles.boldLabel);

        if (GUILayout.Button("🔍 Analyze Image", GUILayout.Height(35)))
        {
            debugger.AnalyzeImage();
        }

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Check Import Settings", GUILayout.Height(30)))
        {
            debugger.CheckTextureImportSettings();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Detailed Analysis", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Corners", GUILayout.Height(25)))
        {
            debugger.ShowCornerPixels();
        }
        if (GUILayout.Button("Print First Row", GUILayout.Height(25)))
        {
            debugger.PrintFirstRow();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Generate Preview Image", GUILayout.Height(30)))
        {
            debugger.GeneratePreviewTexture();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Use this tool to verify your maze image is being read correctly!\n\n" +
            "1. Assign your maze texture\n" +
            "2. Click 'Analyze Image' to see statistics\n" +
            "3. Check that wall/path pixel counts make sense\n" +
            "4. Generate preview to visualize the detection",
            MessageType.Info);
    }
}
#endif
