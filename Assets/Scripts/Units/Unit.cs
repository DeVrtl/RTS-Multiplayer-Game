using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Unit : NetworkBehaviour
{
    [SerializeField] private Health _heatlh;
    [SerializeField] private UnitMovement _movement;
    [SerializeField] private Targeter _targeter;
    [SerializeField] private UnityEvent _onSelected;
    [SerializeField] private UnityEvent _onDeselected;
    [SerializeField] private int _cost;

    public UnitMovement Movement => _movement;
    public Targeter Targeter => _targeter;
    public int Cost => _cost;

    public static event UnityAction<Unit> ServerUnitSpawned;
    public static event UnityAction<Unit> ServerUnitDespawned;
    public static event UnityAction<Unit> AuthorityUnitSpawned;
    public static event UnityAction<Unit> AuthorityUnitDespawned; 

    public override void OnStartServer()
    {
        ServerUnitSpawned?.Invoke(this);
        _heatlh.ServerDied += OnServerDied;
    }

    public override void OnStopServer()
    {
        ServerUnitDespawned?.Invoke(this);
        _heatlh.ServerDied += OnServerDied;
    }

    public override void OnStartAuthority()
    {
        AuthorityUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isOwned)
            return;

        AuthorityUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if (!isOwned)
            return;

        _onSelected?.Invoke();
    }

    [Client]
    public void Deselected()
    {
        if (!isOwned)
            return;

        _onDeselected?.Invoke();
    }

    [Server]
    private void OnServerDied()
    {
        NetworkServer.Destroy(gameObject);
    }
}
// в будущем переделать ивенты по типу "public event UnityAction Selected;" и нормально назвать методы и эвенты