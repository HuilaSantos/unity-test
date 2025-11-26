using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelInstrucoes;

    public void Jogar()
    {
        SceneManager.LoadScene(nomeDoLevelDeJogo);
    }

    public void AbrirOpcoes()
    {
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

    public void AbrirInstrucoes()
    {
        painelMenuInicial.SetActive(false);
        painelInstrucoes.SetActive(true);
    }

    public void FecharInstrucoes()
    {
        painelInstrucoes.SetActive(false);
        painelMenuInicial.SetActive(true);
    }


    public void SairDoJogo()
    {
        Application.Quit();
    }
}
