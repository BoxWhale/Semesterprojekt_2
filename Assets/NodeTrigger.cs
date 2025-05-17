using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NodeID))]
public class NodeTrigger : MonoBehaviour
{
    [Tooltip("If true, the trigger will only be activated once.")]
    public bool singleUse = true;
    [Tooltip("Events to invoke that triggers when the player stands on the node.")]
    public UnityEngine.Events.UnityEvent onTriggerEnterEvent;

    private bool triggered;
    private Player player;
    private NodeID nodeID;

    [Tooltip("Prefab for the indicator (e.g., cube or diamond).")]
    public GameObject indicatorPrefab;

    private GameObject indicatorInstance;

    [Tooltip("Hover speed for the indicator.")]
    public float hoverSpeed = 2f;
    [Tooltip("Hover height for the indicator.")]
    public float hoverHeight = 0.2f;
    [Tooltip("Spin speed for the indicator.")]
    public float spinSpeed = 50f;
    [Tooltip("Time offset for the indicator to hover up and down.")]
    [Range(0f,Mathf.PI)]public float piOffset = 0.5f; // Offset for the sine wave

    [Tooltip("Direction in which the indicator flies when triggered.")]
    public Vector3 flyDirection = Vector3.up;
    [Tooltip("Speed at which the indicator flies.")]
    public float flySpeed = 5f;
    [Tooltip("Time before the event is triggered after the indicator vanishes.")]
    public float eventDelay = 1f;

    private Vector3 initialIndicatorPosition;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        nodeID = gameObject.GetComponent<NodeID>();

        CreateIndicator();
    }

    private void Update()
    {
        if (player.currentNode == nodeID && !triggered)
        {
            triggered = true;
            StartCoroutine(TriggerIndicator());
        }

        if (!triggered)
        {
            AnimateIndicator();
        }
    }

    private void CreateIndicator()
    {
        if (indicatorPrefab != null)
        {
            indicatorInstance = Instantiate(indicatorPrefab, transform.position + Vector3.up, Quaternion.identity, transform);
            indicatorInstance.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); // Adjust size if needed
            initialIndicatorPosition = indicatorInstance.transform.position;
        }
        else
        {
            Debug.LogWarning("Indicator prefab is not assigned.");
        }
    }

    private void AnimateIndicator()
    {
        if (indicatorInstance != null)
        {
            // Hover effect relative to the object's local Y-axis
            float hoverOffset = Mathf.Sin(Time.time * hoverSpeed + piOffset) * hoverHeight;
            indicatorInstance.transform.position = transform.position + transform.rotation * (Vector3.up * hoverOffset + flyDirection);

            // Spin effect around the object's local up direction
            indicatorInstance.transform.Rotate(transform.up, spinSpeed * Time.deltaTime, Space.World);
        }
    }

    private IEnumerator TriggerIndicator()
    {
        if (indicatorInstance != null)
        {
            // Fly the indicator in the specified direction
            float flightDuration = 1f; // Adjust as needed
            float elapsedTime = 0f;

            Vector3 startPosition = indicatorInstance.transform.position;
            Vector3 endPosition = startPosition + flyDirection.normalized * flySpeed;

            while (elapsedTime < flightDuration)
            {
                indicatorInstance.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / flightDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Destroy or deactivate the indicator
            Destroy(indicatorInstance);
        }

        // Wait for the specified delay
        yield return new WaitForSeconds(eventDelay);

        // Trigger the event
        onTriggerEnterEvent.Invoke();
    }
}