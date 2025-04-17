using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [Range(0f,25f)]
    public float run;
    [Range(0f,25f)]
    public float walk;
    InputActionMap _playerActionMap;
    Rigidbody _rb;

    private void Start()
    {
        _playerActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        MovePlayer(_playerActionMap["Move"]);
        
    }

    void MovePlayer(InputAction _moveValue)
    {
        Vector3 pos = new Vector3(_moveValue.ReadValue<Vector2>().x,
                                  0,
                                  _moveValue.ReadValue<Vector2>().y)*Time.deltaTime;

        if (_playerActionMap["Run"].IsPressed())
        {
            _rb.Move(Quaternion.AngleAxis(45,Vector3.up)*pos*run+transform.position,quaternion.identity);
        }
        else
        {
            _rb.Move(Quaternion.AngleAxis(45,Vector3.up)*pos*walk+transform.position,quaternion.identity);
        }
    }
}
