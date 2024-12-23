using Ilumisoft.VisualStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyAI : MonoBehaviour
{
    public StateMachine state;
    public Animator animator;
    public NavMeshAgent agent;

    [Header("FOV SETTINGS")]
    public float fovAngle;
    public float numberOfRays;
    public float rayDistance;
    public float rayHeight;

    [Header("AI SETTINGS")]
    public float wanderXRadius;
    public float wanderZRadius;
    public float attackDistance;
    public float attackDuration;

    Transform playerTransform;
    bool isWanderPointSet;
    bool isAttack;
    NavMeshHit hit;

    public void Wander()
    {
        agent.isStopped = false;
        MoveAnimation();
        if (isWanderPointSet)
        {
            agent.SetDestination(hit.position);
            if (Vector3.Distance(transform.position, hit.position) <= attackDistance)
            {
                isWanderPointSet = false;
            }
        }

        else
        {
            GetWanderPoint();
        }
        if (DetectPlayer())
        {
            state.TriggerByState("Chase");
        }

    }

    void GetWanderPoint()
    {
        float randomX = transform.position.x + Random.Range(-wanderXRadius, wanderXRadius);
        float randomZ = transform.position.z + Random.Range(-wanderZRadius, wanderZRadius);
        Vector3 randomPoint = new Vector3(randomX, 0, randomZ);
        NavMesh.SamplePosition(randomPoint, out hit, 5, NavMesh.AllAreas);
        isWanderPointSet = true;
    }

    public void Chase()
    {
        MoveAnimation();
        agent.isStopped = false;
        agent.SetDestination(playerTransform.position);
        if (Vector3.Distance(transform.position, playerTransform.position) <= attackDistance)
        {
            state.TriggerByState("Attack");
        }
    }

    public void Attack()
    {
        agent.isStopped = true;
        animator.SetFloat("Blend", 0);
        if (!isAttack)
        {
            animator.Play("Attack");
            transform.LookAt(playerTransform);
            isAttack = true;
        }
        else
        {
            state.TriggerByState("Attack Cooldown");
        }
    }

    public void AttackCooldown()
    {
        StartCoroutine(IEAttackDelay());
        IEnumerator IEAttackDelay()
        {
            yield return new WaitForSeconds(attackDuration);
            isAttack = false;
            if (playerTransform == null)
            {
                state.TriggerByState("Wander");
            }
            if (Vector3.Distance(transform.position, playerTransform.position) > attackDistance)
            {
                state.TriggerByState("Chase");
            }
            else
            {
                state.TriggerByState("Attack");
            }
        }
    }

    bool DetectPlayer()
    {
        float halfFOV = fovAngle / 2f;
        float angleStep = fovAngle / (numberOfRays - 1);
        for (int i = 0; i < numberOfRays; i++)
        {
            float currentAngle = -halfFOV + i * angleStep;
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Vector3 rayPosition = new Vector3(transform.position.x, transform.position.y + rayHeight, transform.position.z);
            if (Physics.Raycast(rayPosition, direction, out RaycastHit hit, rayDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerTransform = hit.transform;
                    return true;
                }
            }
            Debug.DrawRay(rayPosition, direction * rayDistance, Color.red);
        }
        return false;
    }

    void MoveAnimation()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
        {
            animator.SetFloat("Blend", agent.velocity.magnitude);
        }
    }
}
