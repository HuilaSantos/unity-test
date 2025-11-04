using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MazeFromImageGenerator))]
public class MazeFromImageGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MazeFromImageGenerator generator = (MazeFromImageGenerator)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("Analyze Current Scene", GUILayout.Height(30)))
        {
            generator.AnalyzeCurrentScene();
        }

        EditorGUILayout.Space(5);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Generate Missing Walls from Image", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog("Generate Maze Walls",
                "This will create wall cubes for all missing positions based on the reference image. Continue?",
                "Yes, Generate", "Cancel"))
            {
                generator.GenerateMazeFromImage();
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear Generated Walls", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Clear Generated Walls",
                "This will delete all walls under the 'Generated Walls' parent. Are you sure?",
                "Yes, Clear", "Cancel"))
            {
                generator.ClearGeneratedWalls();
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "How to use:\n\n" +
            "1. Assign your maze reference image (35 by 25 orthogonal maze.png)\n" +
            "2. Optionally assign a wall prefab (or it will create cubes)\n" +
            "3. Adjust cell size to match your existing walls\n" +
            "4. Click 'Generate Missing Walls from Image'\n\n" +
            "The script will skip positions where walls already exist!",
            MessageType.Info);
    }
}
