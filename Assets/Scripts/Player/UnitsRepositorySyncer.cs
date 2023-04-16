using UnityEngine;
using Mirror;

public class UnitsRepositorySyncer : NetworkBehaviour
{
    [SerializeField] private PlayerUnitsRepository _unitsRepository;

    public void OnServerUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _unitsRepository.MyUnits.Add(unit);
    }

    public void OnServerUnitDisposed(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _unitsRepository.MyUnits.Remove(unit);
    }

    public void OnAuthorityUnitSpawned(Unit unit)
    {
        _unitsRepository.MyUnits.Add(unit);
    }

    public void OnAuthorityUnitDisposed(Unit unit)
    {
        _unitsRepository.MyUnits.Remove(unit);
    }
}
