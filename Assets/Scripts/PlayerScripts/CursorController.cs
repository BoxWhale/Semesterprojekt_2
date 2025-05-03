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

    private void Awake()
    {
        mouse = InputSystem.actions.FindActionMap("Mouse");
        cursor = mouse.FindAction("Position");
        click = mouse.FindAction("Click");
        interact = mouse.FindAction("RClick");
    }

    private void Update()
    {
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

    private void LeftClick()
    {
        Debug.Log("Left mouse button pressed");
        var screenPosition = cursor.ReadValue<Vector2>();
        if (Camera.main == null) return;


        var ray = Camera.main.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 3000, Color.red);
        if (Physics.Raycast(ray, out var hit))
        {
            var nodeID = hit.collider.gameObject.GetComponent<NodeID>();
            if (nodeID == null) return;
            Debug.Log(nodeID.nodeID);
            OnNodeSelected?.Invoke(nodeID);
        }
    }

    private void RightClick()
    {
        Debug.Log("Right mouse button pressed");
        var screenPosition = cursor.ReadValue<Vector2>();
        if (Camera.main == null) return;

        var ray = Camera.main.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
        if (Physics.Raycast(ray, out var hit))
        {
            var interactable = hit.collider.gameObject.GetComponent<IInteractable>();
            if (interactable == null) interactable = hit.collider.gameObject.GetComponentInParent<IInteractable>();
            if (interactable == null) interactable = hit.collider.gameObject.GetComponentInChildren<IInteractable>();
            if (interactable != null) interactable.OnInteract();
            else Debug.LogError("No interactable found");
        }
    }
}