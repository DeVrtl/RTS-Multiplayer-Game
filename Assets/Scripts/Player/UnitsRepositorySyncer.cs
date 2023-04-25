using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class UnitsRepositorySyncer : NetworkBehaviour
{
    [SerializeField] private PlayerUnitsRepository _unitsRepository;

    public void SubscribeOnServerEvents()
    {
        Unit.ServerUnitSpawned += OnServerUnitSpawned;
        Unit.ServerUnitDisposed += OnServerUnitDisposed;
    }

    public void SubscribeOnAuthorityEvents()
    {
        Unit.AuthorityUnitSpawned += OnAuthorityUnitSpawned;
        Unit.AuthorityUnitDisposed += OnAuthorityUnitDisposed;
    }

    public void UnsubscribeOnServerEvents()
    {
        Unit.ServerUnitSpawned -= OnServerUnitSpawned;
        Unit.ServerUnitDisposed -= OnServerUnitDisposed;
    }

    public void UnsubscribeOnAuthorityEvents()
    {
        Unit.AuthorityUnitSpawned -= OnAuthorityUnitSpawned;
        Unit.AuthorityUnitDisposed -= OnAuthorityUnitDisposed;
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
}
