using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class InteractableColorChanger : MonoBehaviour
{
    public Material interactableMaterial;
    public Color color1;
    public Color color2;
    public float colorChangeSpeed = 0.05f;
    public float colorChangeAmount = 0.1f;
    public GameObject player;
    public List<GameObject> gameobjectToStopColorChange;
    public List<int> nodesToStopColorChange;
    public float colorIndex = 0;


    void Awake()
    {
        if (gameobjectToStopColorChange is null) return;
        foreach (GameObject interactableObject in gameobjectToStopColorChange)
        {
            foreach (NodeID node in interactableObject.GetComponentsInChildren<NodeID>())
            {
                nodesToStopColorChange.Add(node.nodeID);
            }
        }
    }

    void Start()
    {
        StartCoroutine(IncreaseColorIndex());
    }

    void Update()
    {
        interactableMaterial.color = Color.Lerp(color1, color2, colorIndex);
    }

    IEnumerator IncreaseColorIndex()
    {
        while (colorIndex < 1)
        {
            if(!nodesToStopColorChange.Contains(player.GetComponent<Player>().currentNode.GetComponent<NodeID>().nodeID)) colorIndex += colorChangeAmount;
            yield return new WaitForSeconds(colorChangeSpeed);
        }
        StartCoroutine(DecreaseColorIndex());
    }

    IEnumerator DecreaseColorIndex()
    {
        while (colorIndex > 0)
        {
            if(!nodesToStopColorChange.Contains(player.GetComponent<Player>().currentNode.GetComponent<NodeID>().nodeID)) colorIndex -= colorChangeAmount;
            yield return new WaitForSeconds(colorChangeSpeed);
        }
        StartCoroutine(IncreaseColorIndex());
    }




}
