using UnityEngine;

public class ResetScores : MonoBehaviour
{
    public void ResetHighscore()
    {
        PlayerPrefs.DeleteKey("Highscore");
        Debug.Log("Highscore reset!");
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs cleared!");
    }
}

