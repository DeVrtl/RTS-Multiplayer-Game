using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
    [SerializeField] private PlayerUnitsRepository _unitsRepository;
    [SerializeField] private PlayerBuildingsRepository _buildingsRepository;
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
        Unit.ServerUnitSpawned += OnServerUnitSpawned;
        Unit.ServerUnitDisposed += OnServerUnitDisposed;
        Building.ServerBuildingSpawned += OnServerBuildingSpawned;
        Building.ServerBuildingDisposed += OnServerBuildingDisposed;

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
        Unit.ServerUnitSpawned -= OnServerUnitSpawned;
        Unit.ServerUnitDisposed -= OnServerUnitDisposed;
        Building.ServerBuildingSpawned -= OnServerBuildingSpawned;
        Building.ServerBuildingDisposed -= OnServerBuildingDisposed;
    }

    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
            return;

        Unit.AuthorityUnitSpawned += OnAuthorityUnitSpawned;
        Unit.AuthorityUnitDisposed += OnAuthorityUnitDisposed;
        Building.AuthorityBuildingSpawned += OnAuthorityBuildingSpawned;
        Building.AuthorityBuildingDisposedd += OnAuthorityBuildingDisposed;
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

        Unit.AuthorityUnitSpawned -= OnAuthorityUnitSpawned;
        Unit.AuthorityUnitDisposed -= OnAuthorityUnitDisposed;
        Building.AuthorityBuildingSpawned -= OnAuthorityBuildingSpawned;
        Building.AuthorityBuildingDisposedd -= OnAuthorityBuildingDisposed;
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

    private void OnServerUnitSpawned(Unit unit) 
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _unitsRepository.MyUnits.Add(unit);
    }

    private void OnServerUnitDisposed(Unit unit) 
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _unitsRepository.MyUnits.Remove(unit);
    }

    private void OnAuthorityUnitSpawned(Unit unit) 
    {
        _unitsRepository.MyUnits.Add(unit);
    }

    private void OnAuthorityUnitDisposed(Unit unit) 
    {
        _unitsRepository.MyUnits.Remove(unit);
    }

    private void OnServerBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _buildingsRepository.MyBuildings.Add(building);
    }

    private void OnServerBuildingDisposed(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _buildingsRepository.MyBuildings.Remove(building);
    }

    private void OnAuthorityBuildingSpawned(Building building)
    {
        _buildingsRepository.MyBuildings.Add(building);
    }

    private void OnAuthorityBuildingDisposed(Building building)
    {
        _buildingsRepository.MyBuildings.Remove(building);
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