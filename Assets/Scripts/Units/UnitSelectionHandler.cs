using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform _unitSelectionArea;
    [SerializeField] private LayerMask _mask;

    private Vector2 _startPosition;
    private Player _player;
    private Camera _main;

    public List<Unit> SelectedUnits { get; private set; } = new List<Unit>();

    private void Start()
    {
        _main = Camera.main;

        Unit.AuthorityUnitDespawned += OnAuthorityUnitDespawned;
        GameOverHandler.ClientGameOvered += OnClientGameOverd;

        _player = NetworkClient.connection.identity.GetComponent<Player>();
    }

    private void OnDestroy()
    {
        Unit.AuthorityUnitDespawned -= OnAuthorityUnitDespawned;
        GameOverHandler.ClientGameOvered -= OnClientGameOverd;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (var selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselected();
            }

            SelectedUnits.Clear();
        }

        _unitSelectionArea.gameObject.SetActive(true);

        _startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - _startPosition.x;
        float areaHeight = mousePosition.y - _startPosition.y;

        _unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        _unitSelectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void ClearSelectionArea()
    {
        _unitSelectionArea.gameObject.SetActive(false);

        if (_unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = _main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _mask))
                return;

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit))
                return;

            if (!unit.isOwned)
                return;

            SelectedUnits.Add(unit);

            foreach (var selectedUnit in SelectedUnits)
                selectedUnit.Select();

            return;
        }

        Vector2 min = _unitSelectionArea.anchoredPosition - (_unitSelectionArea.sizeDelta / 2);
        Vector2 max = _unitSelectionArea.anchoredPosition + (_unitSelectionArea.sizeDelta / 2);

        foreach(Unit unit in _player.MyUnits)
        {
            if (SelectedUnits.Contains(unit))
                continue;

            Vector3 screenPosition = _main.WorldToScreenPoint(unit.transform.position);

            if(screenPosition.x > min.x && screenPosition.x < max.x && screenPosition.y > min.y && screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }

    private void OnAuthorityUnitDespawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
    }

    private void OnClientGameOverd(string _)
    {
        enabled = false;
    }
}
