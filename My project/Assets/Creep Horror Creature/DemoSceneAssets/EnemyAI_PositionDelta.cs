using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI_PositionDelta : MonoBehaviour
{
    public Transform player;
    public float attackRange = 2f;
    public float moveThreshold = 0.05f;

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 lastPos;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        lastPos = transform.position;

        // importante: parar na distância correta
        agent.stoppingDistance = attackRange;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > attackRange)
        {
            // perseguir
            agent.isStopped = false;
            agent.SetDestination(player.position);

            // cálculo de movimento pelo delta de posição
            Vector3 delta = (transform.position - lastPos) / Time.deltaTime;
            float speed = delta.magnitude;
            bool moving = speed > moveThreshold;

            animator.SetBool("isMoving", moving);
        }
        else
        {
            // atacar
            agent.isStopped = true;
            animator.SetBool("isMoving", false);

            // usar trigger p/ não travar
            animator.SetTrigger("Attack");
        }

        lastPos = transform.position;
    }
}
