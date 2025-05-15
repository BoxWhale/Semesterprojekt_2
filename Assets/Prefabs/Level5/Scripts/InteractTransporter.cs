using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTransporter : MonoBehaviour, IInteractable
{
    public bool mouseInteract = true;
    public bool movePlayer = false;
    public GameObject[] invokeSwapNodes;
    public Vector3[] positions;
    public bool isInteracting = false;
    [Tooltip("The speed at which the object moves. Higher values result in faster movement.")]
    [Range(0.1f,200f)]public float speed = 100f;

    public float interval
    {
        get => _interval;
        set => _interval = Mathf.Clamp(value,0f,1);
    }
    private float _interval;
    private bool isEventTriggered = false;
    
    [Tooltip("Angular frequency (radians per second) of the oscillation. Try values between 6 and 10.")]
    public float angularFrequency = 8f;
    [Tooltip("Damping ratio. Values close to 1 (e.g., 0.8 to 0.9) give a heavy, controlled overshoot.")]
    [Range(0.0f, 1.0f)]
    public float dampingRatio = 0.85f;

    public void OnInteract()
    {
        if (!isInteracting && mouseInteract)
        {
            StartCoroutine(MoveToNextPosition());
        }
    }

    public void OnEvent()
    {
        if (!isEventTriggered && isInteracting)
        {
            isEventTriggered = true;
            StartCoroutine(GetIntoPosition());
        }
    }

    private IEnumerator GetIntoPosition()
    {
        float previousInterval = interval;
        while (interval != 1)
        {
            interval += Time.deltaTime * speed/100f;
            if (Mathf.Approximately(interval, previousInterval))
            {
                yield return null;
                continue;
            }

            transform.position = Lerp(positions[0], positions[1], interval);
            previousInterval = interval;
            yield return null;
        }
        transform.position = positions[1];
        // Remove the first position
        if (positions.Length > 1)
        {
            Array.Copy(positions, 1, positions, 0, positions.Length - 1);
            Array.Resize(ref positions, positions.Length - 1);
            Vector3 firstPosition = positions[0];
            Array.Copy(positions, 1, positions, 0, positions.Length - 1);
            positions[^1] = firstPosition;
        }

        foreach (var node in invokeSwapNodes)
        {
            node.GetComponent<NodeID>().walkable = true;
        }

        interval = 0f;
        isInteracting = false;
    }

    private IEnumerator MoveToNextPosition()
    {
        isInteracting = true;
        HashSet<NodeID> children = new HashSet<NodeID>();
        var player = GameObject.Find("Player").GetComponent<Player>();

        foreach (var node in gameObject.GetComponentsInChildren<NodeID>())
        {
            if (player.currentNode != node || movePlayer)
            {
                if (children.Add(node))
                {
                    node.walkable = false;
                }
            }
            else
            {
                foreach (var child in children)
                {
                    child.walkable = true;
                }
                node.walkable = true;
                isInteracting = false;
                yield break;
            }
        }

        float previousInterval = interval;
        while (interval != 1)
        {
            interval += Time.deltaTime * speed/100f;
            if (Mathf.Approximately(interval, previousInterval))
            {
                yield return null;
                continue;
            }

            transform.position = Lerp(positions[1], positions[0], interval);
            previousInterval = interval;
            yield return null;
        }

        foreach (var node in gameObject.GetComponentsInChildren<NodeID>())
        {
            node.walkable = true;
        }

        foreach (var swapNode in invokeSwapNodes)
        {
            try
            {
                swapNode.GetComponent<SwapNodeList>().SwapNodes();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"invokeSwapNodes[{swapNode.gameObject.name}] lacks a SwapNodeList component.\n" + e);
            }
        }

        transform.position = positions[0];
        if (positions.Length > 1)
        {
            Vector3 firstPosition = positions[0];
            Array.Copy(positions, 1, positions, 0, positions.Length - 1);
            positions[^1] = firstPosition;
        }

        interval = 0f;
        isInteracting = false;
    }

    // needs damping function which accelerates, and then rubberbands out at end position
    private Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        // Calculate the damping factor
        float dampingFactor = Mathf.Exp(-dampingRatio * angularFrequency * t);
        
        // Calculate the oscillation term
        float oscillation = Mathf.Sin(angularFrequency * t) * Mathf.Sqrt(1 - dampingRatio * dampingRatio);
        
        // Calculate the position using the damped oscillation formula
        Vector3 position = a + (b - a) * (1 - dampingFactor * (Mathf.Cos(angularFrequency * t) + oscillation));
        
        return position;
    }
}