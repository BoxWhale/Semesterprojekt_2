using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
public class CursorController : MonoBehaviour
{
    public InputActionMap mouse;
    public InputAction cursor;
    public InputAction click;
    public InputAction interact;
    public NodeID node;

    public delegate void NodeSelectedHandler(NodeID node);
    public static event NodeSelectedHandler OnNodeSelected;

    private void Awake()
    {
        mouse = InputSystem.actions.FindActionMap("Mouse");
        cursor = mouse.FindAction("Position");
        click = mouse.FindAction("Click");
        interact = mouse.FindAction("RClick");
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

    private void Update()
    {
        if (click.WasReleasedThisFrame()) 
        {
            LeftClick();
        }

        if (interact.WasReleasedThisFrame())
        {
            RightClick();
        }
    }

    private void LeftClick()
    {
        Debug.Log("Left mouse button pressed");
        Vector2 screenPosition = cursor.ReadValue<Vector2>();
        if (Camera.main == null) return;
        
        
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 3000, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit)) 
        {
            NodeID nodeID = hit.collider.gameObject.GetComponent<NodeID>();
            if (nodeID == null) return;
            Debug.Log(nodeID.nodeID); 
            OnNodeSelected?.Invoke(nodeID);
        }
        
    }

    private void RightClick()
    {
        Debug.Log("Right mouse button pressed");
        Vector2 screenPosition = cursor.ReadValue<Vector2>();
        if (Camera.main == null) return;
        
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
        if (Physics.Raycast(ray, out RaycastHit hit))
        { 
            IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
            if (interactable == null) interactable = hit.collider.gameObject.GetComponentInParent<IInteractable>();
            if (interactable == null) interactable = hit.collider.gameObject.GetComponentInChildren<IInteractable>();
            if (interactable != null) interactable.OnInteract();
            else Debug.LogError("No interactable found");
        }
    }
}


