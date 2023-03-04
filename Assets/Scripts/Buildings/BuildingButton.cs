using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.InputSystem;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building _building;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _price;
    [SerializeField] private LayerMask _floor;

    private Camera _main;
    private BoxCollider _buildingCollider;
    private Player _player;
    private GameObject _buildingPreview;
    private Renderer _buildingRenderer;

    private void Start()
    {
        _main = Camera.main;

        _icon.sprite = _building.Icon;
        _price.text = _building.Price.ToString();

        _player = NetworkClient.connection.identity.GetComponent<Player>();

        _buildingCollider = _building.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (_buildingPreview == null)
            return;

        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (_player.Resources < _building.Price)
            return;

        _buildingPreview = Instantiate(_building.Preview);
        _buildingRenderer = _buildingPreview.GetComponentInChildren<Renderer>();

        _buildingPreview.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_buildingPreview == null)
            return;

        Ray ray = _main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floor))
        {
            _player.CmdTryPlaceBuilding(_building.Id, hit.point);
        }

        Destroy(_buildingPreview);
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = _main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floor))
            return;

        _buildingPreview.transform.position = hit.point;

        if(!_buildingPreview.activeSelf)
            _buildingPreview.SetActive(true);

        Color color = _player.CanPlaceBuilding(_buildingCollider, hit.point) ? Color.green : Color.red;

        _buildingRenderer.material.SetColor("_BaseColor", color);
    }
}
