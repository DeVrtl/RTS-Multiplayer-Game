using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Targeter _targeter;
    [SerializeField] private float _chaseRange;

    public override void OnStartServer()
    {
        GameOverHandler.ServerGameOvered += OnServerGameOvered;
    }

    public override void OnStopClient()
    {
        GameOverHandler.ServerGameOvered -= OnServerGameOvered;
    }


    [ServerCallback]
    private void Update()
    {
        Targetable target = _targeter.Target;

        if (_targeter.Target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > _chaseRange * _chaseRange)
            {
                _agent.SetDestination(target.transform.position);
            }
            else if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
        }

        if (!_agent.hasPath)
            return;

        if (_agent.remainingDistance > _agent.stoppingDistance)
            return;

        _agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }

    [Server]
    public void ServerMove(Vector3 position)
    {
        _targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            return;

        _agent.SetDestination(hit.position);
    }

    [Server]
    private void OnServerGameOvered()
    {
        _agent.ResetPath();
        enabled = false;
    }
}
