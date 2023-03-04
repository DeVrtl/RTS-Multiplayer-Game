using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameOverHandler : NetworkBehaviour
{
    private List<UnitBase> _bases = new List<UnitBase>();

    public static event UnityAction<string> ClientGameOvered;
    public static event UnityAction ServerGameOvered;

    public override void OnStartServer()
    {
        UnitBase.ServerBaseSpawned += OnServerBaseSpawned;
        UnitBase.ServerBaseDespawned += OnServerBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerBaseSpawned -= OnServerBaseSpawned;
        UnitBase.ServerBaseDespawned -= OnServerBaseDespawned;
    }

    [Server]
    private void OnServerBaseSpawned(UnitBase unitBase)
    {
        _bases.Add(unitBase);
    }

    [Server]
    private void OnServerBaseDespawned(UnitBase unitBase)
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
