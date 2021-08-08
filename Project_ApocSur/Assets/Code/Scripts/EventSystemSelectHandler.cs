using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemSelectHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log($"{this.name} deselected");
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log($"{this.name} selected");
    }
}
