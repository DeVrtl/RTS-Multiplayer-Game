using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    [SerializeField] private int _max;

    [SyncVar(hook = nameof(HandleHealthUpdated))] private int _current;

    public event UnityAction ServerDied; 
    public event UnityAction<int, int> ClientHealthUpdated; 

    public override void OnStartServer()
    {
        _current = _max;
        UnitBase.ServerPlayerDied += OnServerPlayerDied;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerPlayerDied -= OnServerPlayerDied;
    }

    [Server]
    public void DealDamage(int damage)
    {
        if (_current == 0)
            return;

        _current = Mathf.Max(_current - damage, 0);

        if (_current != 0)
            return;

        ServerDied?.Invoke();
    }

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientHealthUpdated?.Invoke(newHealth, _max);
    }

    [Server]
    private void OnServerPlayerDied(int playerId)
    {
        if (connectionToClient.connectionId != playerId)
            return;

        DealDamage(_current);
    }
}
