using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool _invokeNextScene = false;

    private Animator animator;

    public float interval
    {
        get => _interval;
        set => _interval = Mathf.Clamp01(value);
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        manager = gameObject.GetComponent<BfsManager3>();
        path = new List<NodeID>();
    }

    private void Update()
    {
        if (path.Count == 0 && currentNode.IsGoal && !_invokeNextScene)
        {
            _invokeNextScene = true;
            currentNode.loadSceneInvoke();
            return;
        }

        if (targetNode != null && path.Count > 0 && allowMovement)
        {
            SetAnimationToRunning();
            interval += speed * Time.deltaTime;
            
            // Get positions with the correct up vector alignment
            Vector3 currentPosition = currentNode.transform.position + currentNode.transform.rotation * Vector3.up;
            Vector3 targetPosition = path[0].transform.position + path[0].transform.rotation * Vector3.up;
            
            // Calculate movement direction
            Vector3 moveDirection = (targetPosition - currentPosition).normalized;
            
            // Update position
            transform.position = Vector3.Lerp(currentPosition, targetPosition, interval);
            
            // Set rotation to face movement direction while maintaining up vector from current node
            if (moveDirection != Vector3.zero)
            {
                Vector3 upVector = currentNode.transform.rotation * Vector3.up;
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, upVector);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, interval);
            }
            
            if (interval >= 1f || path[0] == currentNode) // Use >= instead of == for floating-point comparison
            {
                currentNode = path[0]; // Update currentNode to the new position
                path.RemoveAt(0);
                interval = 0f;
            }
        }
        else
        {
            SetAnimationToWalking();
            transform.position = currentNode.transform.position + currentNode.transform.rotation * Vector3.up;
            
            // Calculate the up vector from the current node
            Vector3 upVector = currentNode.transform.rotation * Vector3.up;
            
            // Maintain the current forward direction but aligned with the node's up vector
            Vector3 forwardDir = transform.forward;
            // Project the forward vector onto the plane defined by the up vector
            Vector3 projectedForward = Vector3.ProjectOnPlane(forwardDir, upVector).normalized;
            
            // If the projected forward is not zero, use it for the rotation
            if (projectedForward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(projectedForward, upVector);
                transform.rotation = targetRotation;
            }
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


    public void SetAnimationToRunning()
    {
        animator.SetBool("isRunning", true);
    }

    public void SetAnimationToWalking()
    {
        animator.SetBool("isRunning", false);
    }


}