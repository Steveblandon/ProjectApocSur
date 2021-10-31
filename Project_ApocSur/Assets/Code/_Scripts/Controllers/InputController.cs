namespace Projapocsur.Scripts
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class InputController : MonoBehaviour
    {
        public static event Action<MouseInput> OnNothingClickedEvent;

        void Update()
        {
            if (Input.GetMouseButtonUp(MouseKey.Left))
            {
                if (this.IsNotBlockedByUI())
                {
                    if (this.RayCast(out Collider2D collider))
                    {
                        if (collider.TryGetComponent(out IPointerLeftClickHandler handler))
                        {
                            handler.OnPointerLeftClick();
                        }
                    }
                    else
                    {
                        OnNothingClickedEvent?.Invoke(MouseInput.Left);
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
                        OnNothingClickedEvent?.Invoke(MouseInput.Right);
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

        public enum MouseInput
        {
            Left = MouseKey.Left,
            Right = MouseKey.Right,
        }

        private class MouseKey
        {
            public const int Left = 0;
            public const int Right = 1;
        }
    }
}