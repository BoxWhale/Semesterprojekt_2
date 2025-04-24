using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    BfsManager manager;
    public float speed;
    private float _interval;

    public float interval
    {
        get => _interval;
        set => _interval = Mathf.Clamp(value, 0f, 1f);
    }

    public NodeID currentNode;
    public NodeID targetNode;
    public List<NodeID> path;

    private void OnEnable()
    {
        CursorController.OnNodeSelected += HandleNodeSelected;
    }

    private void OnDisable()
    {
        CursorController.OnNodeSelected -= HandleNodeSelected;
    }

    private void Start()
    {
        manager = gameObject.GetComponent<BfsManager>();
    }

    private void HandleNodeSelected(NodeID node)
    {
        try
        {
            targetNode = node;
            path = manager.FindPath(currentNode, node);
            Debug.Log("Shortest Path: " + string.Join(" -> ", path));
        }
        catch
        {
            Debug.Log("No path found");
            targetNode = null;
        }
    }

    private void Update()
    {
        if (targetNode != null && path.Count > 0)
        {
            interval += speed * Time.deltaTime; // Multiply by Time.deltaTime for smooth movement
            transform.position = Vector3.Lerp(currentNode.transform.position + currentNode.transform.rotation * Vector3.up, path[0].transform.position + path[0].transform.rotation*Vector3.up, interval);
            transform.rotation = Quaternion.Slerp(currentNode.transform.rotation, path[0].transform.rotation, interval);

            if (interval >= 1f || path[0] == currentNode) // Use >= instead of == for floating-point comparison
            {
                currentNode = path[0]; // Update currentNode to the new position
                path.RemoveAt(0);
                interval = 0f;
            }
        }
        else
        {
            transform.position = currentNode.transform.position + currentNode.transform.rotation * Vector3.up;
        }
    }
}