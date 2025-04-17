using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour, IInteractable
{

    //public GameObject playerReference;

    public List<Vector3> rotatePositions;
    public Vector3 bufferPostion; //Has to visually be the same as the last position
                                  // but with different values that makes it rotate properly
    public int currentPosition = 0;

    public float drag;

    bool isRotating;

    public List<NodeConnection> nodeConnectionsList;

    void Start()
    {
        foreach(NodeConnection nodeConnectionElement in transform.GetComponents<NodeConnection>())
        {
            nodeConnectionsList.Add(nodeConnectionElement);
        }
    }

    public void OnInteract()
    {
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
                transform.eulerAngles = bufferPostion;
                count += Time.deltaTime*(1/drag);
                transform.eulerAngles = Vector3.Slerp(bufferPostion, rotatePositions[0], count);
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
                transform.eulerAngles = Vector3.Slerp(rotatePositions[currentPosition], rotatePositions[nextPosition], count);
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




