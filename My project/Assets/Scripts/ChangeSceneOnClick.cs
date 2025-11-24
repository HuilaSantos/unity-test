using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene loading

public class ChangeSceneOnClick : MonoBehaviour
{
    // Name of the scene to load
    public string sceneName;

    void OnMouseDown()
    {
        // This is called when the GameObject is clicked
        SceneManager.LoadScene(sceneName);
    }
}

