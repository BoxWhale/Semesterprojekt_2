using UnityEngine;
using UnityEngine.InputSystem;

public class PathScript : MonoBehaviour
{
    public Transform lastNode; // Last node the player walked away from
    public Transform currentNode; // Current node the player is on
    public Transform[] connectedNodes; // Nodes connected to the current node
    public float moveSpeed = 5f;
    public Vector2 moveAction;
    public Vector3 movement;
    private bool isMoving;
    private Transform targetNode;

    private void Update()
    {
        // Stop movement if no key is pressed
        if (moveAction.Equals(Vector2.zero))
        {
            isMoving = false;
            return;
        }

        // Check for input if the player is not already moving
        if (targetNode == null)
        {
            if (moveAction == Vector2.right)
                SetTargetNode(Vector3.right);
            else if (moveAction == Vector2.left)
                SetTargetNode(Vector3.left);
            else if (moveAction == Vector2.up)
                SetTargetNode(Vector3.forward);
            else if (moveAction == Vector2.down) SetTargetNode(Vector3.back);
        }
        else
        {
            // Continue moving towards the target node
            MoveToTarget();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveAction = context.ReadValue<Vector2>();
    }


    private void SetTargetNode(Vector3 direction)
    {
        // Search for a connected node in the pressed direction
        foreach (var node in connectedNodes)
        {
            var directionToNode = (node.position - currentNode.position).normalized;
            if (Vector3.Dot(directionToNode, direction.normalized) > 0.9f)
            {
                targetNode = node;
                isMoving = true;
                return;
            }
        }

        // If no valid node is found, stop
        targetNode = null;
        isMoving = false;
    }

    private void MoveToTarget()
    {
        // Move the player toward the target node
        transform.position = Vector3.MoveTowards(transform.position, targetNode.position, moveSpeed * Time.deltaTime);

        // Snap to target node if close enough
        if (Vector3.Distance(transform.position, targetNode.position) < 0.1f)
        {
            currentNode = targetNode;
            targetNode = null;
            isMoving = false;
        }
    }
}