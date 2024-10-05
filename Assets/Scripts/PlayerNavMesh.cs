using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerNavMesh : MonoBehaviour
{
    [SerializeField]
    private Transform playerPosition;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        agent.speed = 1f;
    }

    private void Update()
    {
        agent.destination = playerPosition.position;        
    }
}
