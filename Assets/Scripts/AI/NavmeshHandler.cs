using System;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectC.AI
{
    public class NavmeshHandler : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private bool _hasReached;
        private bool _isMoving;
        public event Action DestinationReachedEvent; 

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void SetSpeed(float value)
        {
            _agent.speed = value;
        }
        
        public void GoToDestination(Vector3 target)
        {
            _isMoving = true;
            _agent.SetDestination(target);
            _agent.isStopped = false;
            _agent.autoBraking = true; 
            _agent.autoRepath = true;
        }

        private void FixedUpdate()
        {
            if(!_isMoving) return;
            ReachedDestinationOrGaveUp();
        }

        private bool ReachedDestinationOrGaveUp()
        {
            if (!_agent.pathPending)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                    {
                        _isMoving = false;
                        _agent.isStopped = true;
                        DestinationReachedEvent?.Invoke();
                        return true;
                    }
                }
            }
            return false;
        }

        public void SetStoppingDistance(float shootingDistance)
        {
            _agent.stoppingDistance = shootingDistance;
        }

        public void Stop()
        {
            _agent.isStopped = true;
            _agent.enabled = false;
            enabled = false;
        }
    }
}