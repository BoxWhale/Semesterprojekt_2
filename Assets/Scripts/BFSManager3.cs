using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BfsManager3 : MonoBehaviour
{
    /// <summary>
    /// Takes in a start NodeID and target NodeID and returns a path that can be walked on between the 2 points
    /// Needs to be run in async as it runs on a background thread
    /// </summary>
    /// <param name="startNode"> The start node</param>
    /// <param name="targetNode"> The node of which needs to be reached</param>
    /// <returns> Returns Graph upon path between Start node and Target node</returns>
    public async Task<List<NodeID>> FindPath(NodeID startNode, NodeID targetNode)
    {
        return await Task.Run(() =>
        {
            Queue<NodeID> queue = new Queue<NodeID>();
            Dictionary<NodeID, NodeID> parentMap = new Dictionary<NodeID, NodeID>();
            HashSet<NodeID> visited = new HashSet<NodeID>();

            // Initialize BFS
            queue.Enqueue(startNode);
            visited.Add(startNode);

            // BFS traversal
            while (queue.Count > 0)
            {
                NodeID currentNode = queue.Dequeue();

                // Check if we've reached the target
                if (currentNode == targetNode) return ConstructPath(startNode, targetNode, parentMap);
                
                // Add adjacent nodes to the queue
                foreach (var neighbor in currentNode.nodes)
                {
                    if (!visited.Contains(neighbor) && neighbor.walkable)
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                        parentMap[neighbor] = currentNode; // Map this node to its parent
                    }
                }
            }
            // If no path is found, return null
            return null;
        });
    }

    private List<NodeID> ConstructPath(NodeID startNode, NodeID targetNode, Dictionary<NodeID, NodeID> parentMap)
    {
        List<NodeID> path = new List<NodeID>();
        NodeID currentNode = targetNode;

        // Backtrack from target to start
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = parentMap[currentNode];
        }
        path.Add(startNode);
        path.Reverse();

        return path;
    }
}