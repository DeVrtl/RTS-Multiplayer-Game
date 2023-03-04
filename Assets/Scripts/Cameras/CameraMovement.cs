using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class CameraMovement : NetworkBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private float _speed;
    [SerializeField] private float _screenBorderThickness;
    [SerializeField] private Vector2 _screenXLimits;
    [SerializeField] private Vector2 _screenZLimits;

    private Controls _controls;
    private Vector2 _previousInput;

    public override void OnStartAuthority()
    {
        _camera.gameObject.SetActive(true);

        _controls = new Controls();

        _controls.Player.MoveCamera.performed += SetPreviousInput;
        _controls.Player.MoveCamera.canceled += SetPreviousInput;

        _controls.Enable();
    }

    [ClientCallback]
    private void Update()
    {
        if (!isOwned || !Application.isFocused)
            return;

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 position = _camera.position;

        if (_previousInput == Vector2.zero)
        {
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            if (cursorPosition.y >= Screen.height - _screenBorderThickness)
                cursorMovement.z += 1;
            else if (cursorPosition.y <= _screenBorderThickness)
                cursorMovement.z -= 1;
            if (cursorPosition.x >= Screen.width - _screenBorderThickness)
                cursorMovement.x += 1;
            else if (cursorPosition.x <= _screenBorderThickness)
                cursorMovement.x -= 1;

            position += cursorMovement.normalized * _speed * Time.deltaTime;
        }
        else
        {
            position += new Vector3(_previousInput.x, 0f, _previousInput.y) * _speed * Time.deltaTime;
        }

        position.x = Mathf.Clamp(position.x, _screenXLimits.x, _screenXLimits.y);
        position.z = Mathf.Clamp(position.z, _screenZLimits.x, _screenZLimits.y);

        _camera.position = position;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        _previousInput = ctx.ReadValue<Vector2>();
    }
}
