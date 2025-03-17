using UnityEngine;
using UnityEngine.InputSystem;
public enum Direction
{
    None,
    North,
    South,
    West,
    East
}

public class MoveScript2 : MonoBehaviour
{
    public Transform[] connectedNodes;
    public Transform targetNode = null;
    public Direction currentDirection = Direction.None;

    private void Update()
    {
        if (targetNode == null || Vector3.Distance(transform.position, targetNode.position) < 0.1f)
        {
            SetTargetNode(currentDirection);
        }

        if (targetNode != null)
        {
            MoveToTargetNode();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input == Vector2.zero)
        {
            currentDirection = Direction.None;
            return;
        }
        if (input == Vector2.up) currentDirection = Direction.North;
        else if (input == Vector2.down) currentDirection = Direction.South;
        else if (input == Vector2.left) currentDirection = Direction.West;
        else if (input == Vector2.right) currentDirection = Direction.East;
    }

    void SetTargetNode(Direction dir)
    {
        foreach (Transform node in connectedNodes)
        {
            Vector3 toNodeDir = (node.position - transform.position).normalized;

            if (dir == Direction.North && Vector3.Dot(Vector3.forward, toNodeDir) > 0.95f)
            {
                targetNode = node;
                break;
            }
            if (dir == Direction.South && Vector3.Dot(Vector3.back, toNodeDir) > 0.95f)
            {
                targetNode = node;
                break;
            }
            if (dir == Direction.West && Vector3.Dot(Vector3.left, toNodeDir) > 0.95f)
            {
                targetNode = node;
                break;
            }
            if (dir == Direction.East && Vector3.Dot(Vector3.right, toNodeDir) > 0.95f)
            {
                targetNode = node;
                break;
            }
        }
    }

    void MoveToTargetNode()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetNode.position, Time.deltaTime * 10f);
    }
}
