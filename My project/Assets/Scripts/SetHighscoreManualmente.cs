using UnityEngine;

public class SetHighscoreManualmente : MonoBehaviour
{
    public float novoHighscore = 45.5f; // tempo em segundos

    void Start()
    {
        PlayerPrefs.SetFloat("Highscore", novoHighscore);
        Debug.Log("Highscore atualizado para: " + novoHighscore);
    }
}
