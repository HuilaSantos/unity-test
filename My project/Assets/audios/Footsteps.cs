using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioClip[] footstepSounds; // Array com os clipes de passos
    public float stepInterval = 0.5f;  // Intervalo entre passos

    private float stepTimer;
    private CharacterController controller;
    private AudioSource audioSource;

    void Start()
    {
        // Pega os componentes do personagem
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        stepTimer = stepInterval;
    }

    void Update()
    {
        // Verifica se está no chão e se o personagem está se movendo
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = stepInterval; // Reseta o timer quando parado
        }
    }

    void PlayFootstep()
    {
        if (footstepSounds.Length > 0)
        {
            int index = Random.Range(0, footstepSounds.Length); // Escolhe um som aleatório
            audioSource.PlayOneShot(footstepSounds[index]);
        }
    }
}
