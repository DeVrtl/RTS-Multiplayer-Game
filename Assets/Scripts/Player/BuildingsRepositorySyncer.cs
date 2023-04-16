using UnityEngine;
using Mirror;

public class BuildingsRepositorySyncer : NetworkBehaviour
{
    [SerializeField] private PlayerBuildingsRepository _buildingsRepository;

    public void OnServerBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _buildingsRepository.MyBuildings.Add(building);
    }

    public void OnServerBuildingDisposed(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _buildingsRepository.MyBuildings.Remove(building);
    }

    public void OnAuthorityBuildingSpawned(Building building)
    {
        _buildingsRepository.MyBuildings.Add(building);
    }

    public void OnAuthorityBuildingDisposed(Building building)
    {
        _buildingsRepository.MyBuildings.Remove(building);
    }
}
