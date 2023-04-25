using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
    [SerializeField] private UnitsRepositorySyncer _unitsRepositorySyncer;
    [SerializeField] private BuildingsRepositorySyncer _buildingsRepositorySyncer;
    [SerializeField] private Transform _camera;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))] private int _resources = 500;
    [SyncVar(hook = nameof(OnAuthorityPartyOwnerStateUpdated))] private bool _isPartyOwner = false;
    [SyncVar(hook = nameof(OnClientInfoUpdated))] private string _displayName;

    private Color _teamColor;

    public int Resources => _resources;
    public Color TeamColor => _teamColor;
    public Transform Camera => _camera;
    public bool IsPartyOwner => _isPartyOwner;
    public string DisplayName => _displayName;

    public event UnityAction<int> ClientResourcesUpdated;

    public static event UnityAction<bool> AuthorityPartyOwnerStateUpdated;
    public static event UnityAction ClientInfoUpdated;

    public override void OnStartServer()
    {
        _unitsRepositorySyncer.SubscribeOnServerEvents();
        _buildingsRepositorySyncer.SubscribeOnServerEvents();

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
        _unitsRepositorySyncer.UnsubscribeOnServerEvents();
        _buildingsRepositorySyncer.UnsubscribeOnServerEvents();
    }

    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
            return;

        _unitsRepositorySyncer.SubscribeOnAuthorityEvents();
        _buildingsRepositorySyncer.SubscribeOnAuthorityEvents();
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active)
            return;

        DontDestroyOnLoad(gameObject);

        ((Network)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        ClientInfoUpdated?.Invoke();

        if (!isClientOnly)
            return;

        ((Network)NetworkManager.singleton).Players.Remove(this);

        if (!isOwned)
            return;

        _unitsRepositorySyncer.UnsubscribeOnAuthorityEvents();
        _buildingsRepositorySyncer.UnsubscribeOnAuthorityEvents();
    }

    [Server]
    public void AssaignTeamColor(Color teamColor)
    {
        _teamColor = teamColor;
    }

    [Server]
    public void SetResources(int amount)
    {
        _resources = amount;
    }

    [Server]
    public void SetDisplayName(string name)
    {
        _displayName = name;
    }

    [Server]
    public void SetPartyOwner(bool state)
    {
        _isPartyOwner = state;
    }

    [Command]
    public void CmdStartGame()
    {
        if (!_isPartyOwner)
            return;

        ((Network)NetworkManager.singleton).StartGame();
    }

    private void OnAuthorityPartyOwnerStateUpdated(bool oldState, bool newState) 
    {
        if (!isOwned)
            return;

        AuthorityPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void ClientHandleResourcesUpdated(int oldAmout, int newAmout)
    {
        ClientResourcesUpdated?.Invoke(newAmout);
    }

    private void OnClientInfoUpdated(string oldName, string newName)
    {
        ClientInfoUpdated?.Invoke();
    }
}