using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class NodeID : MonoBehaviour
{
    public bool autoConnect = true; // If true, the GameObject will autodetect nearby nodes

    [Tooltip(
        "Updates the node detection on runtime to ensure the node is connected to the correct nodes durring position change.\n" +
        "Both Auto connect and Invoke Auto Connect both need to be checked to work, use at your own risk.\n" +
        " - May cause lag if used with a large number of nodes.\n" +
        " - A less risky approach would be using UnityEventHandler instead, and then invoke DetectAdjacentNodes()")]
    public bool invokeAutoConnect; // If true, the GameObject will invoke the autoConnect method on Update()

    public int nodeID; // Store the extracted ID
    public bool walkable = true; // Checkable to see whether the tile is walkable or not

    public List<NodeID> nodes = new(); // List of adjacent nodes
    public float detectionRadius = 0.6f; // Radius to detect adjacent nodes

    private bool initialized; // Tracks if runtime initialization has occurred

    private void Awake()
    {
        initialized = true; // Ensures the runtime flag is set
    }

    private void Update()
    {
        if (invokeAutoConnect && autoConnect && Application.isPlaying) DetectAdjacentNodes();
    }

    // Add at the top with other fields
    [Tooltip("Marks this node as the goal node. Only one node can be the goal in a scene.")]
    [SerializeField] private bool isGoal;
    
    // Static reference to track the current goal node
    private static NodeID currentGoalNode;
    public bool IsGoal
    {
        get => isGoal;
        set
        {
            if (value && currentGoalNode != null && currentGoalNode != this)
            {
                Debug.LogWarning($"Another node ({currentGoalNode.gameObject.name}) is already set as goal. Unchecking it.");
                currentGoalNode.isGoal = false;
            }
            
            isGoal = value;
            if (value)
            {
                currentGoalNode = this;
            }
            else if (currentGoalNode == this)
            {
                currentGoalNode = null;
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
        Handles.Label(transform.position, nodeID.ToString()); // Displays the ID of the node in the scene view
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Draws a wire sphere around the node to show the detection radius
        foreach (var node in nodes)
            if (node != null)
                Gizmos.DrawLine(transform.position + transform.rotation * Vector3.up,
                    node.transform.position + node.transform.rotation * Vector3.up);
        if (!IsGoal) return;
        Gizmos.DrawCube(transform.position + transform.rotation * Vector3.up, new Vector3(0.2f, 0.2f, 0.2f));
    }
        

    private void OnValidate()
    {
        // Skip execution during runtime
        if (Application.isPlaying) return;
        
        if (isGoal && currentGoalNode != null && currentGoalNode != this) currentGoalNode.isGoal = false;
        if (isGoal) currentGoalNode = this;
        
        // Extract the ID from the GameObject's name
        nodeID = ExtractIDFromName(gameObject.name);

        // Extract adjacent GameObjects, clears if false
        if (autoConnect) DetectAdjacentNodes();
    }

    private int ExtractIDFromName(string name)
    {
        if (name.Contains("(") && name.Contains(")"))
        {
            var startIndex = name.IndexOf("(") + 1;
            var endIndex = name.IndexOf(")");

            var idString = name.Substring(startIndex, endIndex - startIndex);
            if (int.TryParse(idString, out var id)) return id;
        }

        //Debug.LogError($"NodeID: Failed to extract ID from name '{gameObject.name}'. Ensure the name format is 'NodeName(ID)'.", gameObject);
        return -1; // Return -1 if ID wasn't found
    }

    public void DetectAdjacentNodes()
    {
        // Create a temporary list to store newly detected nodes
        List<NodeID> newNodes = new();
        var colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (var collider in colliders)
        {
            var nodeIDComponent = collider.gameObject.GetComponent<NodeID>();
            if (nodeIDComponent != null && nodeIDComponent != this &&
                !newNodes.Contains(nodeIDComponent))
                newNodes.Add(nodeIDComponent);
        }

        // Update the nodes list only if there are changes
        if (!nodes.SequenceEqual(newNodes)) nodes = newNodes;
    }
}