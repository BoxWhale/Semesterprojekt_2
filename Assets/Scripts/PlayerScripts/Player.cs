using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public bool allowMovement = true;

    public NodeID currentNode;
    public NodeID targetNode;
    public List<NodeID> path;
    private float _interval;

    private BfsManager3 manager;

    public float interval
    {
        get => _interval;
        set => _interval = Mathf.Clamp(value, 0f, 1f);
    }

    private void Start()
    {
        manager = gameObject.GetComponent<BfsManager3>();
        path = new List<NodeID>();
    }

    private void Update()
    {
        if (targetNode != null && path.Count > 0 && allowMovement)
        {
            interval += speed * Time.deltaTime;
            transform.position =
                Vector3.Lerp(currentNode.transform.position + currentNode.transform.rotation * Vector3.up, 
                             path[0].transform.position + path[0].transform.rotation * Vector3.up, interval);
            transform.rotation = Quaternion.Slerp(currentNode.transform.rotation, path[0].transform.rotation, interval);
            if (interval >= 1f || path[0] == currentNode) // Use >= instead of == for floating-point comparison
            {
                currentNode = path[0]; // Update currentNode to the new position
                gameObject.layer = currentNode.gameObject.layer;
                path.RemoveAt(0);
                interval = 0f;
            }
        }
        else
        {
            transform.position = currentNode.transform.position + currentNode.transform.rotation * Vector3.up;
            transform.rotation = currentNode.transform.rotation;
            gameObject.layer = currentNode.gameObject.layer;
        }
    }

    private void OnEnable()
    {
        CursorController.OnNodeSelected += HandleNodeSelected3;
    }

    private void OnDisable()
    {
        CursorController.OnNodeSelected -= HandleNodeSelected3;
    }
    private bool IsValidMovement(NodeID targetNode)
    {
        return targetNode != null && 
               targetNode.walkable && 
               targetNode.gameObject.layer == currentNode.gameObject.layer;
    }

    public async void HandleNodeSelected3(NodeID node)
    {
        try
        {
            // Only process new path if target changed or no current path exists
            if (node != targetNode || path.Count == 0)
            {
                targetNode = node;
                var newPath = await manager.FindPath(currentNode, node);
            
                // Only update path if we found a valid one
                if (newPath != null && newPath.Count > 0)
                {
                    path = newPath;
                    Debug.Log("Shortest Path: " + string.Join(" -> ", path));
                }
            }
        }
        catch
        {
            Debug.Log("No path found");
            targetNode = null;
            path.Clear();
        }
    }
}