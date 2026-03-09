using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Fly camera using the New Input System.
/// Hold Right Mouse Button to look and move.
/// WASD = move, Q/E = down/up, Shift = fast.
/// </summary>
public class FreeCameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float fastMultiplier = 3f;

    [Header("Look")]
    public float lookSensitivity = 0.2f;

    private Transform _cam;
    private float _pitch = 0f;
    private float _yaw = 0f;

    void Start()
    {
        var camComponent = GetComponentInChildren<Camera>();
        _cam = camComponent != null ? camComponent.transform : transform;

        _yaw   = transform.eulerAngles.y;
        _pitch = _cam.localEulerAngles.x;
        if (_pitch > 180f) _pitch -= 360f;
    }

    void Update()
    {
        var mouse    = Mouse.current;
        var keyboard = Keyboard.current;
        if (mouse == null || keyboard == null) return;

        if (mouse.rightButton.isPressed)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;

            // Look
            Vector2 delta = mouse.delta.ReadValue();
            _yaw   += delta.x * lookSensitivity;
            _pitch -= delta.y * lookSensitivity;
            _pitch  = Mathf.Clamp(_pitch, -89f, 89f);

            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            _cam.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

            // Move
            float speed = moveSpeed * (keyboard.leftShiftKey.isPressed ? fastMultiplier : 1f);
            Vector3 move = Vector3.zero;
            if (keyboard.wKey.isPressed) move += _cam.forward;
            if (keyboard.sKey.isPressed) move -= _cam.forward;
            if (keyboard.aKey.isPressed) move -= transform.right;
            if (keyboard.dKey.isPressed) move += transform.right;
            if (keyboard.eKey.isPressed) move += Vector3.up;
            if (keyboard.qKey.isPressed) move -= Vector3.up;

            transform.position += move * speed * Time.deltaTime;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
    }
}
