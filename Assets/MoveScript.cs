using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveScript : MonoBehaviour
{
    public Transform[] connectedNodes;
    public Transform targetNode = null;
    public Transform previousNode = null;
    public Vector3 direction;
    private bool moving;
    private bool movingDirection;


    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 callback = context.ReadValue<Vector2>();
        if (callback == Vector2.zero)
        {
            moving = false;
            direction = Vector3.zero;
            return;
        }
        if (callback == Vector2.up){ direction = Vector3.forward; moving = true;}
        else if (callback == Vector2.down){ direction = Vector3.back; moving = true;}
        else if (callback == Vector2.left){ direction = Vector3.left; moving = true;}
        else if (callback == Vector2.right){ direction = Vector3.right; moving = true;}
    }

    private void Update()
    {
        if( targetNode == null || Vector3.Distance(transform.position, targetNode.position) < 0.1f)
        {
            setTargetNode(direction);
        }
        if (Vector3.Dot(targetNode.position.normalized,direction.normalized)>0.9f && direction!=Vector3.zero)
        {
            MoveToTaretNode();
            Debug.Log("Moving to taret node");
        }
        else if (Vector3.Dot(previousNode.position.normalized,direction.normalized)>0.9f && direction!=Vector3.zero)
        {
            MoveToLastNode();
            Debug.Log("Moving to previous node");
        }
        
    }

    void setTargetNode(Vector3 dir)
    {
        previousNode = targetNode;

        connectedNodes = connectedNodes.OrderBy(point => Vector3.Distance(point.position, transform.position)).ToArray();

        if (dir != Vector3.zero)
        {
            foreach (Transform node in connectedNodes)
            {
                Vector3 toNodeDirection = (node.position - transform.position).normalized;

                if (Vector3.Dot(dir, toNodeDirection) > 0.95f && node != previousNode)
                {
                    targetNode = node; 
                    break;
                }
            }
            Debug.Log("Loop End");
        }
    }

    

    void MoveToTaretNode()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetNode.position, Time.deltaTime * 10f);
        Debug.Log($"Move to {targetNode.name}");
    }

    void MoveToLastNode()
    {
        transform.position = Vector3.MoveTowards(transform.position, previousNode.position, Time.deltaTime * 10f);
        Debug.Log($"Move to {previousNode.name}");
    }
}
