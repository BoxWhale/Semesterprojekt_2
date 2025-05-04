using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    public delegate void NodeSelectedHandler(NodeID node);

    public InputActionMap mouse;
    public InputAction cursor;
    public InputAction click;
    public InputAction interact;
    public NodeID node;

    
    [Header("Mouse Settings"), Range(0f,2f)]
    public float cooldownTime = 0.5f;
    [SerializeField]private float _lmbCooldown;
    public float lmbCooldown
    {
        get => _lmbCooldown;
        set => _lmbCooldown = Mathf.Clamp(value, 0f, cooldownTime);
    }
    
    [SerializeField]private float _rmbCooldown;
    public float rmbCooldown
    {
        get => _rmbCooldown;
        set => _rmbCooldown = Mathf.Clamp(value, 0f, cooldownTime);
    }
    
    private void Awake()
    {
        mouse = InputSystem.actions.FindActionMap("Mouse");
        cursor = mouse.FindAction("Position");
        click = mouse.FindAction("Click");
        interact = mouse.FindAction("RClick");
    }

    private void Update()
    {
        // Only decrement cooldowns when they are above 0
        if (lmbCooldown > 0f) lmbCooldown -= Time.deltaTime;
    
        if (rmbCooldown > 0f) rmbCooldown -= Time.deltaTime;

        if (click.WasReleasedThisFrame()) LeftClick();
        if (interact.WasReleasedThisFrame()) RightClick();
    }

    private void OnEnable()
    {
        click.Enable();
        interact.Enable();
        cursor.Enable();
    }

    private void OnDisable()
    {
        click.Disable();
        interact.Disable();
        cursor.Disable();
    }

    public static event NodeSelectedHandler OnNodeSelected;

    private void RightClick()
    {
        Debug.Log("Right mouse button pressed");

        if (rmbCooldown > 0f) return;
        
        var screenPosition = cursor.ReadValue<Vector2>();
        if (Camera.main == null) return;
            
        var ray = Camera.main.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 3000, Color.red);
        if (Physics.Raycast(ray, out var hit))
        {
            var nodeID = hit.collider.gameObject.GetComponent<NodeID>();
            if (nodeID == null) return;
            Debug.Log(nodeID.nodeID);
            lmbCooldown = cooldownTime;
            OnNodeSelected?.Invoke(nodeID);
        }
    }

    private void LeftClick()
    {
        Debug.Log("Left mouse button pressed");
        if (lmbCooldown > 0f) return;
        var screenPosition = cursor.ReadValue<Vector2>();
        if (Camera.main == null) return;

        var ray = Camera.main.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
        if (Physics.Raycast(ray, out var hit))
        {
            var interactable =  hit.collider.gameObject.GetComponent<IInteractable>() ?? 
                                hit.collider.gameObject.GetComponentInParent<IInteractable>() ?? 
                                hit.collider.gameObject.GetComponentInChildren<IInteractable>();
            if (interactable != null)
            {
                lmbCooldown = cooldownTime;
                interactable.OnInteract();
            }
            else Debug.LogError("No interactable found");
        }
    }
}