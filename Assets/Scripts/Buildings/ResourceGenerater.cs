using Mirror;
using UnityEngine;

public class ResourceGenerater : NetworkBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private int _resourcesPerInterval;
    [SerializeField] private float _interval;

    private float _timer;
    private Player _player;

    public override void OnStartServer()
    {
        _timer = _interval;
        _player = connectionToClient.identity.GetComponent<Player>();

        _health.ServerDied += OnServerDied;
        GameOverHandler.ServerGameOvered += OnServerGameOvered;
    }

    public override void OnStopServer()
    {
        _health.ServerDied -= OnServerDied;
        GameOverHandler.ServerGameOvered -= OnServerGameOvered;
    }

    [ServerCallback]
    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _player.SetResources(_player.Resources + _resourcesPerInterval);

            _timer += _interval;
        }
    }

    [Server]
    private void OnServerDied()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void OnServerGameOvered()
    {
        enabled = false;   
    }
}
