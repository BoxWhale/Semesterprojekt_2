using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[RequireComponent(typeof(NodeID))]public class SwapNodeList : MonoBehaviour
{
    [SerializeField] private NodeID nodeID;
    [SerializeField] private List<NodeID> nodesToSwap;

    [ExecuteAlways]
    private void OnValidate()
    {
        nodeID = gameObject.GetComponent<NodeID>();
        if (nodeID == null)
        {
            Debug.LogWarning("NodeID component not found on this GameObject.");
            return;
        }

        if (nodesToSwap == null || nodesToSwap.Count == 0)
        {
            Debug.LogWarning("No nodes to swap specified.");
            return;
        }
    }

    public void SwapNodes()
    {
        //if (nodeID == null || nodesToSwap == null) return;
        if (nodeID.nodes == null) nodeID.nodes = new List<NodeID>();
        if (nodesToSwap == null) nodesToSwap = new List<NodeID>();

        (nodeID.nodes, nodesToSwap) = (nodesToSwap, nodeID.nodes);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        switch (nodesToSwap.Count) // Visual indicator for if Nodes have been added to List
        {
            case 0:
                Gizmos.color = Color.red; // No nodes registered in List
                break;
            case 1:
                Gizmos.color = Color.yellow; // Only 1 Node registered in List
                break;
            case >= 2 and <= 6:
                Gizmos.color = Color.green; // More than 1 and less than 7 nodes registered in List
                break;
            case >= 7:
                Gizmos.color = Color.blue; // Number of Listed nodes overwhelms max number of sides of a cube
                Debug.LogWarning($"NodeID: Node overload detected on {gameObject.name}!\n" +
                                 "Number of nodes exceeds the number of faces on a cube,\n" +
                                 "Consider reducing the number of nodes or reduce the detection radius.");
                break;
        }
        foreach (var node in nodesToSwap)
        {
            if (node != null)
            {
                Vector3 start = transform.position + transform.rotation * Vector3.up;
                Vector3 end = node.transform.position + node.transform.rotation * Vector3.up;
                float dashLength = 0.05f; // Length of each dash
                float gapLength = 0.1f; // Length of each gap
                float totalLength = Vector3.Distance(start, end);
                Vector3 direction = (end - start).normalized;

                for (float i = 0; i < totalLength; i += dashLength + gapLength)
                {
                    Vector3 dashStart = start + direction * i;
                    Vector3 dashEnd = start + direction * Mathf.Min(i + dashLength, totalLength);
                    Gizmos.DrawLine(dashStart, dashEnd);
                }
            }
        }
    }
#endif //Gizmos draw for editor only
}
