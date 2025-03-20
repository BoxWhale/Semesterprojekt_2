using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    public InputActionMap mouse;
    public InputAction cursor;
    public InputAction click;
    public NodeID node;

    public delegate void NodeSelectedHandler(NodeID node);
    public static event NodeSelectedHandler OnNodeSelected;

    private void Awake()
    {
        mouse = InputSystem.actions.FindActionMap("Mouse");
        cursor = mouse.FindAction("Position");
        click = mouse.FindAction("Click");
    }

    private void Update()
    {
        if (click.WasReleasedThisFrame()) 
        {
            Debug.Log("Left mouse button pressed");
            Vector2 screenPosition = cursor.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                NodeID nodeID = hit.collider.gameObject.GetComponent<NodeID>();
                if (nodeID != null)
                {
                    Debug.Log(nodeID.nodeID);
                    OnNodeSelected?.Invoke(nodeID);
                }
            }
        }

    }
}

