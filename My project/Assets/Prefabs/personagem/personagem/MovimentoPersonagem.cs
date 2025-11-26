using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoPersonagem : MonoBehaviour
{
    private CharacterController character;
    private Animator animator;

    private Vector3 inputs;
    private Transform cam;        // Referência da câmera

    private float velocidade = 2f;

    void Start()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        cam = Camera.main.transform; // pega a câmera usada pela Cinemachine
    }

    void Update()
    {
        // --- 1. ENTRADAS DO JOGADOR ---
        inputs = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // --- 2. MOVIMENTO RELATIVO À CÂMERA ---
        Vector3 direcao = cam.TransformDirection(inputs);
        direcao.y = 0; // evita inclinar o personagem
        direcao.Normalize();

        // --- 3. MOVIMENTO DO CHARACTER CONTROLLER ---
        character.Move(direcao * Time.deltaTime * velocidade);
        character.Move(Vector3.down * Time.deltaTime); // "gravidade" simples

        // --- 4. ANIMAÇÃO E ROTAÇÃO SUAVE ---
        if (inputs != Vector3.zero)
        {
            animator.SetBool("andando", true);

            // rotaciona o personagem para onde está andando (com base na câmera)
            transform.forward = Vector3.Slerp(transform.forward, direcao, Time.deltaTime * 10);
        }
        else
        {
            animator.SetBool("andando", false);
        }
    }
}
