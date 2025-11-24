using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;

    void Start()
    {
        // Current run time
        float finalTime = PlayerPrefs.GetFloat("FinalTime", 0);
        int minutes = Mathf.FloorToInt(finalTime / 60);
        int seconds = Mathf.FloorToInt(finalTime % 60);
        scoreText.text = "Seu Tempo: " + string.Format("{0:00}:{1:00}", minutes, seconds);

        // Highscore
        float bestTime = PlayerPrefs.GetFloat("Highscore", float.MaxValue);
        if (bestTime != float.MaxValue)
        {
            int bestMinutes = Mathf.FloorToInt(bestTime / 60);
            int bestSeconds = Mathf.FloorToInt(bestTime % 60);
            highscoreText.text = "Melhor Tempo: " + string.Format("{0:00}:{1:00}", bestMinutes, bestSeconds);
        }
        else
        {
            highscoreText.text = "Melhor Tempo: --:--"; // No highscore yet
        }
    }
}

