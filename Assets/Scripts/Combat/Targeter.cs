using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable _target;

    public Targetable Target => _target;

    public override void OnStartServer()
    {
        GameOverHandler.ServerGameOvered += OnServerGameOvered;
    }

    public override void OnStopClient()
    {
        GameOverHandler.ServerGameOvered -= OnServerGameOvered;
    }

    [Command]
    public void CmdSetTarget(GameObject target)
    {
        if (!target.TryGetComponent(out Targetable newTarget))
            return;

        _target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        _target = null;
    }

    [Server]
    private void OnServerGameOvered()
    {
        ClearTarget();
    }
}
