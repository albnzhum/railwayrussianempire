using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float idleTime = 2f;

    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;
    private Transform target;
    private float idleTimer;
    private enum State { Idle, Walk }
    private State currentState;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        target = pointA;
        SetState(State.Walk);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Walk:
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    SetState(State.Idle);
                }
                break;
            case State.Idle:
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleTime)
                {
                    target = (target == pointA) ? pointB : pointA;
                    SetState(State.Walk);
                }
                break;
        }
    }

    private void SetState(State newState)
    {
        currentState = newState;
        switch (newState)
        {
            case State.Walk:
                agent.SetDestination(target.position);
                animator.SetFloat("Speed", agent.speed); // Устанавливаем скорость в Blend Tree
                break;
            case State.Idle:
                idleTimer = 0f;
                animator.SetFloat("Speed", 0f); // Останавливаем анимацию
                break;
        }
    }
}
