using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter _targeter;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _barrel;
    [SerializeField] private float _fireRange;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _rotationSpeed;

    private float _lastFireTime;

    [ServerCallback]
    private void Update()
    {
        Targetable target = _targeter.Target;

        if (target == null)
            return;

        if (!CanFire())
            return;

        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        if (Time.time > (1 / _fireRate) + _lastFireTime)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(target.AimPoint.position - _barrel.position);

            GameObject projectile = Instantiate(_projectile, _barrel.position, projectileRotation);

            NetworkServer.Spawn(projectile, connectionToClient);

            _lastFireTime = Time.time;
        }
    }

    [Server]
    private bool CanFire()
    {
        return (_targeter.Target.transform.position - transform.position).sqrMagnitude <= _fireRange * _fireRange;
    }
}
