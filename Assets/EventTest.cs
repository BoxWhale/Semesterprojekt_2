using UnityEngine;
using UnityEngine.Events;

public class EventTest : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteractEvent;
    public void OnInteract()
    {
        OnInteractEvent?.Invoke();
    }
}
