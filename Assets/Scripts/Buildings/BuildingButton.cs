using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.InputSystem;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private BuildingInputHandler _inputHandler;
    [SerializeField] private Building _building;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _price;
    [SerializeField] private LayerMask _floor;

    private Camera _main;
    private BoxCollider _buildingCollider;
    private PlayerBuildings _playerBuildings;
    private Player _player;
    private GameObject _buildingPreview;
    private Renderer _buildingRnderer;

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
        _inputHandler.ShowPreview(_player, _building, _buildingPreview, _buildingRnderer);
    }

    public void OnPointerUp(PointerEventData eventData) 
    {
        _inputHandler.HidePreview(_playerBuildings, _building, _buildingPreview, _main, _floor);
    }

    private void UpdateBuildingPreview()
    {
        _inputHandler.UpdatePreview(_playerBuildings, _buildingPreview, _buildingCollider, _buildingRnderer, _main, _floor);
    }
}
