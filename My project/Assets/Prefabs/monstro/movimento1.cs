using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Movimento1 : MonoBehaviour
{
    public bool isGrabbed = false;
    public int vidaPlayer = 100;
    
private AudioSource audioSource;

public static class RespawnFlag
{
    public static bool respawned = false;
}

    private CharacterController character;
    private Animator animator;
     public GameObject vidro;
     public AudioClip somDano;

public AudioClip somRespawn;
    private Vector3 inputs;
    private Transform cam;        // Referência da câmera

    private float velocidade = 2f;

void Start()
{
    character = GetComponent<CharacterController>();
    animator = GetComponentInChildren<Animator>();
    audioSource = GetComponent<AudioSource>();
    vidro.SetActive(false);
    cam = Camera.main.transform;

    // Toca o som se acabou de respawnar
    if (RespawnFlag.respawned)
    {
        audioSource.PlayOneShot(somRespawn);
        RespawnFlag.respawned = false; // Limpa a flag para não repetir
    }
}


void Update()
{
   //  --- SE O JOGADOR FOI AGARRADO, TRAVA TUDO ---
   if (isGrabbed)
    {
        animator.SetBool("andando", false);
        return;
    }

    // --- 1. ENTRADAS DO JOGADOR ---
    inputs = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

    // --- 2. MOVIMENTO RELATIVO À CÂMERA ---
    Vector3 direcao = cam.TransformDirection(inputs);
    direcao.y = 0;
    direcao.Normalize();

    // --- 3. MOVIMENTO DO CHARACTER CONTROLLER ---
    character.Move(direcao * Time.deltaTime * velocidade);
    character.Move(Vector3.down * Time.deltaTime);

    // --- 4. ANIMAÇÃO E ROTAÇÃO SUAVE ---
    if (inputs != Vector3.zero)
    {
        animator.SetBool("andando", true);
        transform.forward = Vector3.Slerp(transform.forward, direcao, Time.deltaTime * 10);
    }
    else
    {
        animator.SetBool("andando", false);
    }
if (vidaPlayer <= 0)
{
    RespawnFlag.respawned = true;  // Marca que houve respawn
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}



}

void OnTriggerEnter(Collider collider)
{
    if(collider.gameObject.tag=="maoInimigo")
    {
        audioSource.PlayOneShot(somDano); // ← SOM DE DANO
        vidaPlayer -= 20;
        vidro.SetActive(true);
    }
}
}
