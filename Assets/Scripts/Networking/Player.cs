using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class Player : NetworkBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private LayerMask _buildingBlock;
    [SerializeField] private Building[] _buildings;
    [SerializeField] private float _buildingRangeLimit;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))] private int _resources = 500;
    [SyncVar(hook = nameof(OnAuthorityPartyOwnerStateUpdated))] private bool _isPartyOwner = false;
    [SyncVar(hook = nameof(OnClientInfoUpdated))] private string _displayName;

    private List<Unit> _myUnits = new List<Unit>();
    private List<Building> _myBuildings = new List<Building>();
    private Color _teamColor;

    public List<Unit> MyUnits => _myUnits;
    public List<Building> MyBuildings => _myBuildings;
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
        Unit.ServerUnitDespawned += OnServerUnitDespawned;
        Building.ServerBuildingSpawned += OnServerBuildingSpawned;
        Building.ServerBuildingDespawned += OnServerBuildingDespawned;

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
        Unit.ServerUnitSpawned -= OnServerUnitSpawned;
        Unit.ServerUnitDespawned -= OnServerUnitDespawned;
        Building.ServerBuildingSpawned -= OnServerBuildingSpawned;
        Building.ServerBuildingDespawned -= OnServerBuildingDespawned;
    }

    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
            return;

        Unit.AuthorityUnitSpawned += OnAuthorityUnitSpawned;
        Unit.AuthorityUnitDespawned += OnAuthorityUnitDespawned;
        Building.AuthorityBuildingSpawned += OnAuthorityBuildingSpawned;
        Building.AuthorityBuildingDespawned += OnAuthorityBuildingDespawned;
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
        Unit.AuthorityUnitDespawned -= OnAuthorityUnitDespawned;
        Building.AuthorityBuildingSpawned -= OnAuthorityBuildingSpawned;
        Building.AuthorityBuildingDespawned -= OnAuthorityBuildingDespawned;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 position)
    {
        if (Physics.CheckBox(position + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, _buildingBlock))
            return false;

        foreach (Building building in _myBuildings)
        {
            if ((position - building.transform.position).sqrMagnitude <= _buildingRangeLimit * _buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
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

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 position)
    {
        Building buildingToPlace = null;

        foreach(Building building in _buildings)
        {
            if (building.Id == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null)
            return;

        if (_resources < buildingToPlace.Price)
            return;

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(buildingCollider, position))
            return;

        Building instantiatedBuilding = Instantiate(buildingToPlace, position, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(instantiatedBuilding.gameObject, connectionToClient);

        SetResources(_resources - buildingToPlace.Price);
    }

    private void OnServerUnitSpawned(Unit unit) 
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _myUnits.Add(unit);
    }

    private void OnServerUnitDespawned(Unit unit) 
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _myUnits.Remove(unit);
    }

    private void OnAuthorityUnitSpawned(Unit unit) 
    {
        _myUnits.Add(unit);
    }

    private void OnAuthorityUnitDespawned(Unit unit) 
    {
        _myUnits.Remove(unit);
    }

    private void OnServerBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _myBuildings.Add(building);
    }

    private void OnServerBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _myBuildings.Remove(building);
    }

    private void OnAuthorityBuildingSpawned(Building building)
    {
        _myBuildings.Add(building);
    }

    private void OnAuthorityBuildingDespawned(Building building)
    {
        _myBuildings.Remove(building);
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