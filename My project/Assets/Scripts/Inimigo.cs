using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]

public class Inimigo : MonoBehaviour
{
    private Animator animInimigo;
    
    private NavMeshAgent navMesh;
    private GameObject player;
     private GameObject maoInimigo;
    public float velocidadeInimigo;

   void Start()
{
    animInimigo = GetComponentInChildren<Animator>();
    navMesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
    player = GameObject.FindWithTag("Player");
    maoInimigo = GameObject.FindWithTag("maoInimigo");
    navMesh.speed = velocidadeInimigo;
maoInimigo.SetActive(false);

        
    if (animInimigo == null)
    {
        Debug.LogError("Animator não encontrado no inimigo! Coloque ele no modelo do inimigo.");
    }
}



void Update()
{
    navMesh.destination = player.transform.position;

    if (Vector3.Distance(transform.position, player.transform.position) < 3.0f)
    {
       
        navMesh.speed = 0;
         maoInimigo.SetActive(true);
        animInimigo.SetBool("atack", true);
        StartCoroutine("ataque");
    }
}

IEnumerator ataque()
{
    yield return new WaitForSeconds(2.8f);
    animInimigo.SetBool("atack", false);
    navMesh.speed = velocidadeInimigo;
     maoInimigo.SetActive(false);
}

}
