using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameOverHandler : NetworkBehaviour
{
    private List<Headquarters> _bases = new List<Headquarters>();

    public static event UnityAction<string> ClientGameOvered;
    public static event UnityAction ServerGameOvered;

    public override void OnStartServer()
    {
        Headquarters.ServerBaseSpawned += OnServerBaseSpawned;
        Headquarters.ServerBaseDisposed += OnServerBaseDisposed;
    }

    public override void OnStopServer()
    {
        Headquarters.ServerBaseSpawned -= OnServerBaseSpawned;
        Headquarters.ServerBaseDisposed -= OnServerBaseDisposed;
    }

    [Server]
    private void OnServerBaseSpawned(Headquarters unitBase)
    {
        _bases.Add(unitBase);
    }

    [Server]
    private void OnServerBaseDisposed(Headquarters unitBase)
    {
        _bases.Remove(unitBase);

        if (_bases.Count != 1) 
            return; 

        int playerId = _bases[0].connectionToClient.connectionId;

        RpcGameOver($"Player {playerId}");

        ServerGameOvered?.Invoke();
    }

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientGameOvered?.Invoke(winner);
    }
}
