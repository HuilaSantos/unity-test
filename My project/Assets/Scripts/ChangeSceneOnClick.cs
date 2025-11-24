using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene loading

public class ChangeSceneOnClick : MonoBehaviour
{
    // Name of the scene to load
    public string sceneName;

    void OnMouseDown()
    {
        // Save the current time before changing scene
        PlayerPrefs.SetFloat("FinalTime", Timer.elapsedTime);
        float bestTime = PlayerPrefs.GetFloat("Highscore", float.MaxValue);

        if (Timer.elapsedTime < bestTime) // smaller time = better
        {
            PlayerPrefs.SetFloat("Highscore", Timer.elapsedTime);
        }
        PlayerPrefs.Save();

        // This is called when the GameObject is clicked
        SceneManager.LoadScene(sceneName);
    }
}
