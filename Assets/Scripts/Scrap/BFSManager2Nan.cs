using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BfsManager2Nan : MonoBehaviour
{
    /// <summary>
    ///     Takes in a start NodeID and target NodeID and returns a path that can be walked on between the 2 points
    /// </summary>
    /// <param name="startNode"> The start node</param>
    /// <param name="targetNode"> The node of which needs to be reached</param>
    /// <returns></returns>
    public IEnumerator FindPath(NodeID startNode, NodeID targetNode, Action<List<NodeID>> callback)
    {
        var queue = new Queue<NodeID>();
        var parentMap = new Dictionary<NodeID, NodeID>();
        var visited = new HashSet<NodeID>();

        // Initialize BFS
        queue.Enqueue(startNode);
        visited.Add(startNode);

        // BFS traversal
        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            // Check if we've reached the target
            if (currentNode == targetNode)
            {
                var path = ConstructPath(startNode, targetNode, parentMap);
                callback(path);
                yield break;
            }

            // Add adjacent nodes to the queue
            foreach (var neighbor in currentNode.nodes)
                if (!visited.Contains(neighbor) && neighbor.walkable)
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    parentMap[neighbor] = currentNode; // Map this node to its parent
                }

            yield return null;
        }

        // If no path is found, return null
        callback(null);
    }

    private List<NodeID> ConstructPath(NodeID startNode, NodeID targetNode, Dictionary<NodeID, NodeID> parentMap)
    {
        var path = new List<NodeID>();
        var currentNode = targetNode;

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