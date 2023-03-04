using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health _health;

    public static event UnityAction<int> ServerPlayerDied;
    public static event UnityAction<UnitBase> ServerBaseSpawned;
    public static event UnityAction<UnitBase> ServerBaseDespawned; 

    public override void OnStartServer()
    {
        _health.ServerDied += OnServerDied;

        ServerBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        _health.ServerDied -= OnServerDied;

        ServerBaseDespawned?.Invoke(this);
    }

    [Server]
    private void OnServerDied()
    {
        ServerPlayerDied?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }
}
