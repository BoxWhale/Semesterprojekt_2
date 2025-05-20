using UnityEngine;

public class NodeConnection : MonoBehaviour
{
    public string noteAboutConnection;
    public NodeID node1;
    public NodeID node2;

    public bool[] shouldConnectAccPosition = new bool[0];

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (transform.GetComponent<RotationScript>().rotatePositions.Count != shouldConnectAccPosition.Length)
            shouldConnectAccPosition = new bool[transform.GetComponent<RotationScript>().rotatePositions.Count];
    }

    public void UpdateConnectionAccPosition(int currentPosition)
    {
        if (currentPosition > shouldConnectAccPosition.Length) return;

        if (shouldConnectAccPosition[currentPosition]) Invoke("ConnectNodes",0.01f);
        else DisconnecetNodes();
    }

    public void ConnectNodes()
    {
        if (!node1.GetComponent<NodeID>().nodes.Contains(node2.GetComponent<NodeID>()))
            node1.GetComponent<NodeID>().nodes.Add(node2.GetComponent<NodeID>());
        if (!node2.GetComponent<NodeID>().nodes.Contains(node1.GetComponent<NodeID>()))
            node2.GetComponent<NodeID>().nodes.Add(node1.GetComponent<NodeID>());
    }

    public void DisconnecetNodes()
    {
        if (node1.GetComponent<NodeID>().nodes.Contains(node2.GetComponent<NodeID>()))
            node1.GetComponent<NodeID>().nodes.Remove(node2.GetComponent<NodeID>());
        if (node2.GetComponent<NodeID>().nodes.Contains(node1.GetComponent<NodeID>()))
            node2.GetComponent<NodeID>().nodes.Remove(node1.GetComponent<NodeID>());
    }
}