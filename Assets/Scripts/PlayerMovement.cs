using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Range(0f, 25f)] public float run;

    [Range(0f, 25f)] public float walk;

    private InputActionMap _playerActionMap;
    private Rigidbody _rb;

    private void Start()
    {
        _playerActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MovePlayer(_playerActionMap["Move"]);
    }

    private void MovePlayer(InputAction _moveValue)
    {
        var pos = new Vector3(_moveValue.ReadValue<Vector2>().x,
            0,
            _moveValue.ReadValue<Vector2>().y) * Time.deltaTime;

        if (_playerActionMap["Run"].IsPressed())
            _rb.Move(Quaternion.AngleAxis(45, Vector3.up) * pos * run + transform.position, quaternion.identity);
        else
            _rb.Move(Quaternion.AngleAxis(45, Vector3.up) * pos * walk + transform.position, quaternion.identity);
    }
}