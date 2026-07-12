using System.Collections;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Transform target;
    public NavMeshAgent agent;
    public float LookRadius = 10f;
    [SerializeField] float m_health = 100;
    [SerializeField] public AudioSource _audioFx;
    [SerializeField] public AudioClip _diefx;
    
    private float targetUpdateRate = 0.5f; // Частота обновления цели
    private float lastTargetUpdate;
    private LayerMask raycastMask;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        raycastMask = LayerMask.GetMask("Player", "Obstacles"); // Важно добавить слой препятствий!
        lastTargetUpdate = -targetUpdateRate; // Сразу запустить поиск
    }

    private void Update()
    {
        if (Time.time - lastTargetUpdate >= targetUpdateRate)
        {
            lastTargetUpdate = Time.time;
            FindNearestVisiblePlayer();
        }
        
        // Двигаемся только если есть цель и она валидна
        if (target != null && agent.enabled)
        {
            agent.SetDestination(target.position);
        }
    }

    private void FindNearestVisiblePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform nearestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            
            // Проверка радиуса и наличия прямой видимости
            if (distance <= LookRadius && 
                distance < closestDistance &&
                HasLineOfSight(player.transform))
            {
                closestDistance = distance;
                nearestTarget = player.transform;
            }
        }

        target = nearestTarget;
        
        // Останавливаем агента если цель пропала
        if (target == null && agent.enabled)
        {
            agent.ResetPath();
        }
    }

    private bool HasLineOfSight(Transform player)
    {
        Vector3 direction = player.position - transform.position;
        RaycastHit hit;
        
        if (Physics.Raycast(
            transform.position + Vector3.up * 0.5f, // Старт с высоты груди
            direction, 
            out hit,
            LookRadius,
            raycastMask))
        {
            return hit.transform.CompareTag("Player");
        }
        return false;
    }

    public void InflictDamage(float value)
    {
        m_health -= value;
        Debug.Log($"Hit in {transform.name} HP: {m_health}");
        if (m_health <= 0)
        {
            _audioFx.PlayOneShot(_diefx);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, LookRadius);
        
        // Визуализация луча видимости
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, target.position);
        }
    }
}