using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour, IInteractable
{
    public Vector3 maxRotation;
    public Vector3 minRotation;
    public float drag;
    public bool orientation;

    public List<GameObject> walkables = new();

    private float _count;

    public float count
    {
        get => _count;
        set => _count = Mathf.Clamp(value, 0, 1);
    }


    private void Update()
    {
        if (orientation && transform.localRotation.eulerAngles.z > maxRotation.z)
        {
            count += Time.deltaTime * (1 / drag);
            transform.eulerAngles = Vector3.Slerp(minRotation, maxRotation, count);
        }
        else
        {
            count -= Time.deltaTime * (1 / drag);
            transform.eulerAngles = Vector3.Slerp(minRotation, maxRotation, count);
        }

        if (count == 1)
            foreach (var node in walkables)
                node.GetComponent<NodeID>().walkable = true;
        else if (count == 0)
            foreach (var node in walkables)
                node.GetComponent<NodeID>().walkable = false;
    }

    public void OnInteract()
    {
        if (orientation)
            orientation = false;
        else
            orientation = true;
    }

    public List<GameObject> Children(GameObject parent)
    {
        var children = new List<GameObject>();
        foreach (Transform child in parent.transform) children.Add(child.gameObject);
        return children;
    }
}