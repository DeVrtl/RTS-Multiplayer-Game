using UnityEngine;
using Mirror;

public class BuildingsRepositorySyncer : NetworkBehaviour
{
    [SerializeField] private PlayerBuildingsRepository _buildingsRepository;

    public void SubscribeOnServerEvents()
    {
        Building.ServerBuildingSpawned += OnServerBuildingSpawned;
        Building.ServerBuildingDisposed += OnServerBuildingDisposed;
    }

    public void SubscribeOnAuthorityEvents()
    {
        Building.AuthorityBuildingSpawned += OnAuthorityBuildingSpawned;
        Building.AuthorityBuildingDisposedd += OnAuthorityBuildingDisposed;
    }

    public void UnsubscribeOnServerEvents()
    {
        Building.ServerBuildingSpawned -= OnServerBuildingSpawned;
        Building.ServerBuildingDisposed -= OnServerBuildingDisposed;
    }

    public void UnsubscribeOnAuthorityEvents()
    {
        Building.AuthorityBuildingSpawned -= OnAuthorityBuildingSpawned;
        Building.AuthorityBuildingDisposedd -= OnAuthorityBuildingDisposed;
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
}
