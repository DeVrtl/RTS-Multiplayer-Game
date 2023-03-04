using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler _selectionHandler;
    [SerializeField] private LayerMask _mask;

    private Camera _main;

    private void Start()
    {
        _main = Camera.main;

        GameOverHandler.ClientGameOvered -= OnClientGameOverd;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientGameOvered -= OnClientGameOverd;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;

        Ray ray = _main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _mask))
            return;

        if(hit.collider.TryGetComponent(out Targetable target))
        {
            if (target.isOwned)
            {
                TryMove(hit.point);
                return;
            }

            TryTarget(target);
            return;
        }

        TryMove(hit.point);
    }

    private void TryMove(Vector3 point)
    {
        foreach (Unit unit in _selectionHandler.SelectedUnits)
            unit.Movement.CmdMove(point);
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in _selectionHandler.SelectedUnits)
            unit.Targeter.CmdSetTarget(target.gameObject);
    }

    private void OnClientGameOverd(string _)
    {
        enabled = false;
    }
}
