using UnityEngine;
using Mirror;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private int _damage;
    [SerializeField] private float _destroyAfterSeconds;
    [SerializeField] private float _launchForce;

    private void Start()
    {
        _rigidbody.velocity = transform.forward * _launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), _destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NetworkIdentity identity))
        {
            if (identity.connectionToClient == connectionToClient)
                return;
        }

        if (other.TryGetComponent(out Health health))
            health.DealDamage(_damage);

        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
