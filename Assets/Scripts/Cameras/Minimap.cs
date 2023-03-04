using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Mirror;

public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform _minimapTransform;
    [SerializeField] private float _mapScale;
    [SerializeField] private float _offset;

    private Transform _playerCamera;

    private void Update()
    {
        if (_playerCamera != null)
            return;

        if (NetworkClient.connection?.identity == null)
            return;

        _playerCamera = NetworkClient.connection.identity.GetComponent<Player>().Camera;
    }

    private void MoveCamera()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_minimapTransform, mousePosition, null, out Vector2 localPoint))
            return;

        Vector2 lerp = new Vector2((localPoint.x - _minimapTransform.rect.x) / _minimapTransform.rect.width, (localPoint.y - _minimapTransform.rect.y) / _minimapTransform.rect.height);

        Vector3 newCameraPosition = new Vector3(Mathf.Lerp(-_mapScale, _mapScale, lerp.x), _playerCamera.position.y, Mathf.Lerp(-_mapScale, _mapScale, lerp.y));

        _playerCamera.position = newCameraPosition + new Vector3(0f, 0f, _offset);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }
}
