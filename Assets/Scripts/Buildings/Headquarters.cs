using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Headquarters : NetworkBehaviour
{
    [SerializeField] private Health _health;

    public static event UnityAction<int> ServerPlayerDied;
    public static event UnityAction<Headquarters> ServerBaseSpawned;
    public static event UnityAction<Headquarters> ServerBaseDisposed; 

    public override void OnStartServer()
    {
        _health.ServerDied += OnServerDied;

        ServerBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        _health.ServerDied -= OnServerDied;

        ServerBaseDisposed?.Invoke(this);
    }

    [Server]
    private void OnServerDied()
    {
        ServerPlayerDied?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }
}
