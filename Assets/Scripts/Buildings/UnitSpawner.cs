using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Mirror;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health _health;
    [SerializeField] private Unit _unit;
    [SerializeField] private Transform _unitSpawnPoint;
    [SerializeField] private TMP_Text _remainingUnits;
    [SerializeField] private Image _unitProgress;
    [SerializeField] private int _maxQueue;
    [SerializeField] private float _spawnMoveRange;
    [SerializeField] private float _unitSpawnDuration;

    [SyncVar (hook = nameof(ClientHandleQueuedUnitsUpdated))] private int _queuedUnits;
    [SyncVar] private float _unitTimer;

    private float _progressVelocity;

    private void Update()
    {
        if (isServer)
            ProduceUnits();

        if (isClient)
            UpdateTimerDisplay();
    }

    public override void OnStartServer()
    {
        _health.ServerDied += OnServerDied;
    }

    public override void OnStopServer()
    {
        _health.ServerDied -= OnServerDied;
    }

    [Server]
    private void ProduceUnits()
    {
        if (_queuedUnits == 0)
            return;

        _unitTimer += Time.deltaTime;

        if (_unitTimer < _unitSpawnDuration)
            return;

        GameObject unit = Instantiate(_unit.gameObject, _unitSpawnPoint.position, _unitSpawnPoint.rotation);

        NetworkServer.Spawn(unit, connectionToClient);

        Vector3 spawnOffset = Random.insideUnitSphere * _spawnMoveRange;
        spawnOffset.y = _unitSpawnPoint.position.y;

        UnitMovement unitMovement = unit.GetComponent<UnitMovement>();
        unitMovement.ServerMove(_unitSpawnPoint.position + spawnOffset);

        _queuedUnits--;
        _unitTimer = 0f;
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (_queuedUnits == _maxQueue)
            return;

        Player player = connectionToClient.identity.GetComponent<Player>();

        if (player.Resources < _unit.Cost)
            return;

        _queuedUnits++;

        player.SetResources(player.Resources - _unit.Cost);
    }

    private void UpdateTimerDisplay()
    {
        float newProgress = _unitTimer / _unitSpawnDuration;

        if (newProgress < _unitProgress.fillAmount)
            _unitProgress.fillAmount = newProgress;
        else
            _unitProgress.fillAmount = Mathf.SmoothDamp(_unitProgress.fillAmount, newProgress, ref _progressVelocity, 0.1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (!isOwned)
            return;

        CmdSpawnUnit();
    }

    [Server]
    private void OnServerDied() 
    {
       NetworkServer.Destroy(gameObject); 
    }

    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        _remainingUnits.text = $"{newUnits}";
    }
}