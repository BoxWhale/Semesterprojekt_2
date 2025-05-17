using System.Collections;
using UnityEngine;

public class StaticObjectToPosition : MonoBehaviour
{
    public Vector3 targetPosition;
    public float speed = 100f;
    private bool isMoving = false;
    private bool hasTriggered = false;

    [Tooltip("Angular frequency (radians per second) of the oscillation. Try values between 6 and 10.")]
    public float angularFrequency = 8f;
    [Tooltip("Damping ratio. Values close to 1 (e.g., 0.8 to 0.9) give a heavy, controlled overshoot.")]
    [Range(0.0f, 1.0f)]
    public float dampingRatio = 0.85f;

    [Tooltip("Nodes to invoke SwapNodes on after movement.")]
    public GameObject[] invokeSwapNodes;

    public void TriggerMove()
    {
        if (!hasTriggered && !isMoving)
        {
            StartCoroutine(MoveToTargetPosition());
        }
    }

    private IEnumerator MoveToTargetPosition()
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float interval = 0f;

        while (interval < 1f)
        {
            interval += Time.deltaTime * speed / 100f;
            transform.position = DampedLerp(startPosition, targetPosition, interval);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
        hasTriggered = true; // Prevent further movement

        // Invoke SwapNodes on the specified nodes
        foreach (var node in invokeSwapNodes)
        {
            try
            {
                node.GetComponent<SwapNodeList>().SwapNodes();
            }
            catch
            {
                Debug.LogWarning($"GameObject {node.name} does not have a SwapNodeList component.");
            }
        }
    }

    private Vector3 DampedLerp(Vector3 a, Vector3 b, float t)
    {
        float dampingFactor = Mathf.Exp(-dampingRatio * angularFrequency * t);
        float oscillation = Mathf.Sin(angularFrequency * t) * Mathf.Sqrt(1 - dampingRatio * dampingRatio);
        return a + (b - a) * (1 - dampingFactor * (Mathf.Cos(angularFrequency * t) + oscillation));
    }
}