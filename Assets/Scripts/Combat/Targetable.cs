using Mirror;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform _aimPoint;

    public Transform AimPoint => _aimPoint;
}
