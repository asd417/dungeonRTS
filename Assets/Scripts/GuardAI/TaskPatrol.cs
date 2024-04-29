using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using UnityEngine.AI;

public class TaskPatrol : Node
{
    private NavMeshAgent _navAgent;
    private Transform _transform;
    //private Animator _animator;
    private Transform[] _waypoints;

    private int _currentWaypointIndex = 0;

    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;

    public TaskPatrol(NavMeshAgent navAgent, Transform[] waypoints)
    {
        _navAgent = navAgent;
        _transform = navAgent.transform;
        //_animator = _transform.GetComponent<Animator>();
        _waypoints = waypoints;
    }

    public override NodeState Evaluate()
    {
        if (_waiting)
        {
            _navAgent.enabled = false;
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _waiting = false;
                //_animator.SetBool("Walking", true);
            }
        }
        else
        {
            _navAgent.enabled = true;
            Transform wp = _waypoints[_currentWaypointIndex];
            _navAgent.destination = wp.position;
            if (NavAgentAtDestination())
            {
                _transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;

                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                //_animator.SetBool("Walking", false);
            }
        }
        state = NodeState.RUNNING;
        return state;
    }

    private bool NavAgentAtDestination()
    {
        if (!_navAgent.pathPending)
        {
            if (_navAgent.remainingDistance <= _navAgent.stoppingDistance)
            {
                if (!_navAgent.hasPath || _navAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;

    }
}
