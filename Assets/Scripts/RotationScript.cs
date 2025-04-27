using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RotationScript : MonoBehaviour, IInteractable
{
    public bool canRotateWithPlayerOn = false;
    public GameObject Player;
    public List<Vector3> rotatePositions;
    public Vector3 bufferPostion; //Has to visually be the same as the last position
                                  // but with different values that makes it rotate properly
    public int currentPosition = 0;

    public float drag;

    bool isRotating;

    public List<NodeConnection> nodeConnectionsList;
    public List<int> containedNodesInRotation;
    //public UnityEvent rotationEvent;

    void Start()
    {
        foreach(NodeConnection nodeConnectionElement in transform.GetComponents<NodeConnection>())
        {
            nodeConnectionsList.Add(nodeConnectionElement);
        }
        foreach(NodeID node in GetComponentsInChildren<NodeID>())
        {
            containedNodesInRotation.Add(node.nodeID);
        }
    }

    public void OnInteract()
    {
        if(!canRotateWithPlayerOn && containedNodesInRotation.Contains(Player.GetComponent<Player>().currentNode.GetComponent<NodeID>().nodeID)) return;

        if(!isRotating)
        {
            StartCoroutine(RotateToNextPosition());
        }
    }
    
    public IEnumerator RotateToNextPosition()
    {
        isRotating = true;

        float count = 0;

        if(currentPosition == (rotatePositions.Count - 1)) //If it's in the last position, it loops back to the first (via the buffer position)
        {
            while(count < 1)
            {
                transform.localEulerAngles = bufferPostion;
                count += Time.deltaTime*(1/drag);
                transform.localEulerAngles = Vector3.Slerp(bufferPostion, rotatePositions[0], count);
                yield return new WaitForEndOfFrame();
            }

            currentPosition = 0;
        }
        else //Otherwise it just rotates to the next position on the list
        {
            int nextPosition = currentPosition + 1;
            while(count < 1)
            {
                count += Time.deltaTime*(1/drag);
                transform.localEulerAngles = Vector3.Slerp(rotatePositions[currentPosition], rotatePositions[nextPosition], count);
                yield return new WaitForEndOfFrame();
            }

            currentPosition++;
        }

        foreach(NodeConnection nodeConnection in nodeConnectionsList)
        {
            nodeConnection.UpdateConnectionAccPosition(currentPosition);
        }

        isRotating = false;
    }

}




