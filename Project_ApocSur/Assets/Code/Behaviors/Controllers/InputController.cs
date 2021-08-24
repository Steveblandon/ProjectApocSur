namespace Projapocsur.Behaviors
{
    using Projapocsur.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using EventType = Common.EventType;

    public class InputController : MonoBehaviour
    {

        public void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (this.IsNotBlockedByUI())
                {
                    EventManager.Instance.TriggerEvent(EventType.UI_NothingClicked);

                    if (this.RayCast(out Collider2D collider))
                    {
                        if (collider.TryGetComponent(out SimpleSelectable selectable))
                        {
                            selectable.OnSelect();
                        }
                    }
                    else
                    {
                        EventManager.Instance.TriggerEvent(EventType.WO_NothingClicked);
                    }
                }
            }
        }

        private bool IsNotBlockedByUI()
        {
            return EventSystem.current.currentSelectedGameObject == null && !EventSystem.current.IsPointerOverGameObject();
        }

        private bool RayCast(out Collider2D colliderHit)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            colliderHit = hit.collider;
            return hit.collider != null;
        }
    }

}