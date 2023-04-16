using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System.Collections.Generic;

public class Network : NetworkManager
{
    [SerializeField] private SessionInitializer _session;

    private bool _isGameInProgress = false;

    public static event UnityAction ClientConnected;
    public static event UnityAction ClientDisconnected;

    public List<Player> Players { get; private set; } = new List<Player>();

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        ClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        ClientDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(!_isGameInProgress)

        base.OnServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Player player = conn.identity.GetComponent<Player>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Player player = conn.identity.GetComponent<Player>();

        Players.Add(player);

        player.SetDisplayName($"Player {Players.Count}");

        player.AssaignTeamColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        player.SetPartyOwner(Players.Count == 1);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        _isGameInProgress = false;
    }

    public void StartGame()
    {
        if (Players.Count < 2)
            return;

        _isGameInProgress = true;

        ServerChangeScene("Map_01");
    }

    public override void OnServerSceneChanged(string sceneName) 
    {
        _session.Initialize(Players, this);
    }
}