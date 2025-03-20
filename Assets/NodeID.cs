using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class NodeID : MonoBehaviour
{
    public bool autoConnect = true; // If true, the GameObject will autodetect nearby nodes
    private bool initialized = false; // Tracks if runtime initialization has occurred
    public int nodeID;             // Store the extracted ID
    public bool walkable = true;   // Checkable to see whether the tile is walkable or not

    private void Awake()
    {
        initialized = true; // Ensures the runtime flag is set
    }

    void OnValidate()
    {
        // Skip execution during runtime
        if (Application.isPlaying)
        {
            return;
        }

        // Extract the ID from the GameObject's name
        nodeID = ExtractIDFromName(gameObject.name);

        // Extract adjacent GameObjects, clears if false
        if (autoConnect)
        {
            DetectAdjacentNodes();
        }
    }

    int ExtractIDFromName(string name)
    {
        if (name.Contains("(") && name.Contains(")"))
        {
            int startIndex = name.IndexOf("(") + 1;
            int endIndex = name.IndexOf(")");

            string idString = name.Substring(startIndex, endIndex - startIndex);
            if (int.TryParse(idString, out int id))
            {
                return id;
            }
        }
        return -1; // Return -1 if ID wasn't found
    }

    public List<NodeID> nodes = new(); // List of adjacent nodes
    public float detectionRadius = 0.6f; // Radius to detect adjacent nodes
    public void DetectAdjacentNodes()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (var collider in colliders)
        {
            var nodeIDComponent = collider.gameObject.GetComponent<NodeID>();
            if (nodeIDComponent != null && nodeIDComponent != this && !nodes.Contains(nodeIDComponent) && nodeIDComponent.autoConnect)
            {
                nodes.Add(nodeIDComponent);
            }
        }
    }

    private void OnDrawGizmos()
    {
        switch (nodes.Count) // Visual indicator for if Nodes have been added to List
        {
            case 0:
                Gizmos.color = Color.red; // No nodes registered in List
                break;
            case 1:
                Gizmos.color = Color.yellow; // Only 1 Node registered in List
                break;
            case (>= 2 and <= 6):
                Gizmos.color = Color.green; // More than 1 and less than 7 nodes registered in List
                break;
            case >= 7:
                Gizmos.color = Color.blue; // Number of Listed nodes overwhelms max number of sides of a cube
                break;
        }
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        foreach (var node in nodes)
        {
            if (node != null)
            {
                Gizmos.DrawLine(transform.position + Vector3.up, node.transform.position + Vector3.up);
            }
        }
    }
}
