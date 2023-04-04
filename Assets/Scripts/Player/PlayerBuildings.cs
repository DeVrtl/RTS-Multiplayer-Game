using Mirror;
using UnityEngine;

public class PlayerBuildings : NetworkBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private PlayerBuildingsRepository _repository;
    [SerializeField] private LayerMask _buildingBlock;
    [SerializeField] private Building[] _buildings;
    [SerializeField] private float _buildingRangeLimit;

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 position)
    {
        if (Physics.CheckBox(position + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, _buildingBlock))
            return false;

        foreach (Building building in _repository.MyBuildings)
        {
            if ((position - building.transform.position).sqrMagnitude <= _buildingRangeLimit * _buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 position)
    {
        Building buildingToPlace = null;

        foreach (Building building in _buildings)
        {
            if (building.Id == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null)
            return;

        if (_player.Resources < buildingToPlace.Price)
            return;

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(buildingCollider, position))
            return;

        Building instantiatedBuilding = Instantiate(buildingToPlace, position, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(instantiatedBuilding.gameObject, connectionToClient);

        _player.SetResources(_player.Resources - buildingToPlace.Price);
    }
}
