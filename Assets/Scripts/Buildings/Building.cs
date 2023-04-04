using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Building : NetworkBehaviour
{
    [SerializeField] private GameObject _preview;
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _id = -1;
    [SerializeField] private int _price;

    public Sprite Icon  => _icon;
    public int Id => _id;
    public int Price => _price;
    public GameObject Preview => _preview;

    public static event UnityAction<Building> ServerBuildingSpawned;
    public static event UnityAction<Building> ServerBuildingDisposed;
    public static event UnityAction<Building> AuthorityBuildingSpawned;
    public static event UnityAction<Building> AuthorityBuildingDisposedd;

    public override void OnStartServer()
    {
        ServerBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerBuildingDisposed?.Invoke(this);
    }

    public override void OnStartAuthority()
    {
        AuthorityBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isOwned)
            return;

        AuthorityBuildingDisposedd?.Invoke(this);
    }
}
