using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public AudioSource audioFx;
    public AudioClip dieClip;

    [Header("Combat")]
    public float health = 100f;
    public float lookRadius = 50f;
    public float attackDistance = 2f;
    public float damage = 10f;
    public float attackCooldown = 1f;

    [Header("Senses")]
    public float updateRate = 0.5f;
    public LayerMask obstaclesMask = -1;

    private Transform target;
    private float lastUpdateTime;
    private float nextAttackTime;
    private bool isDead;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        obstaclesMask = LayerMask.GetMask("Player", "Obstacles");
        lastUpdateTime = -updateRate;
    }

    void Update()
    {
        if (isDead) return;

        if (Time.time - lastUpdateTime >= updateRate)
        {
            lastUpdateTime = Time.time;
            UpdateTarget();
        }

        if (target != null && agent.enabled)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist <= attackDistance)
            {
                agent.ResetPath();
                TryAttack();
            }
            else
            {
                agent.SetDestination(target.position);
            }
        }
        else
        {
            if (agent.enabled && agent.hasPath) agent.ResetPath();
        }
    }

    void UpdateTarget()
    {
        target = null;
        float bestDist = lookRadius;

        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist > lookRadius) continue;
            if (dist >= bestDist) continue;
            if (!HasLineOfSight(player.transform)) continue;

            bestDist = dist;
            target = player.transform;
        }
    }

    bool HasLineOfSight(Transform player)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = player.position - origin;
        float dist = dir.magnitude;
        dir.Normalize();

        return !Physics.Raycast(origin, dir, out RaycastHit hit, dist, obstaclesMask);
    }

    void TryAttack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        if (target == null) return;

        var health = target.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.DamageTaken(damage);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        if (audioFx != null && dieClip != null)
            audioFx.PlayOneShot(dieClip);

        if (agent != null) agent.enabled = false;

        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        if (target != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, target.position);
        }
    }
}