namespace Projapocsur.Behaviors
{
    using Projapocsur.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using EventType = Common.EventType;

    public class InputController : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonUp(MouseKey.Left))
            {
                if (this.IsNotBlockedByUI())
                {
                    EventManager.Instance.TriggerEvent(EventType.UI_NothingClicked);

                    if (this.RayCast(out Collider2D collider))
                    {
                        if (collider.TryGetComponent(out IPointerLeftClickHandler handler))
                        {
                            handler.OnPointerLeftClick();
                        }
                    }
                    else
                    {
                        EventManager.Instance.TriggerEvent(EventType.WO_NothingClicked_Left);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(MouseKey.Right))
            {
                if (this.IsNotBlockedByUI())
                {
                    if (this.RayCast(out Collider2D collider))
                    {
                        // TBD, expected usage as trigger for floating menus
                    }
                    else
                    {
                        EventManager.Instance.TriggerEvent(EventType.WO_NothingClicked_Right);
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

    public class MouseKey
    {
        public static int Left { get; } = 0;

        public static int Right { get; } = 1;
    }
}