using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CapsuleCollider))]
public class EnemyAI : MonoBehaviour
{
    public float damageAmount    = 10f;
    public float arrivalDistance = 1.5f;
    public float moveSpeed       = 3.5f;

    public event Action OnEnemyDied;

    private NavMeshAgent agent;
    private Transform oasisTransform;
    private bool hasDealtDamage;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = arrivalDistance;

        GameObject oasisGO = GameObject.FindWithTag("Oasis");
        if (oasisGO != null)
        {
            oasisTransform = oasisGO.transform;
            agent.SetDestination(oasisTransform.position);
        }
        else
        {
            Debug.LogWarning("[EnemyAI] Could not find GameObject with tag 'Oasis'.");
        }
    }

    void Update()
    {
        if (hasDealtDamage || oasisTransform == null) return;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            hasDealtDamage = true;
            Debug.Log($"[EnemyAI] {name} reached the Oasis — dealing {damageAmount} damage");

            OasisHealth oasisHealth = oasisTransform.GetComponent<OasisHealth>();
            if (oasisHealth != null)
                oasisHealth.TakeDamage(damageAmount);

            OnEnemyDied?.Invoke();
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (!hasDealtDamage)
        {
            OnEnemyDied?.Invoke();
        }
    }
}
