namespace Projapocsur.Engine
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// An Input handler for non-UI interactivity. UI interaction is left to the EventSystem and it's established paradigm. This is so as to not waste time rewriting and
    /// executing similar code (i.e. graphic raycasts).
    /// </summary>
    public class InputController : MonoBehaviour
    {
        public event Action<MouseButton> OnNothingClickedEvent;

        private const string CompName = nameof(InputController);

        private static InputController _instance;
        public static InputController Main { get => _instance; }

        private event Action<ITargetable> TargetClickListeners;
        private Collider2D lastHitCollider;
        private Dictionary<KeyCode, Action<Collider2D>> mouseButtonClickHandler;
        private bool skipFrame;

        public void Awake()
        {
            mouseButtonClickHandler = new Dictionary<KeyCode, Action<Collider2D>>()
            {
                { KeyCode.Mouse0, this.OnLeftClick},
                { KeyCode.Mouse1, this.OnRightClick},
            };
        }

        public void Start()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Debug.LogWarning($"{CompName}: multiple instances of input controller detected. Only the first one will be used as main.");
            }
        }

        void Update()
        {
            if (skipFrame)
            {
                skipFrame = false;
                return;
            }

            foreach (KeyCode keyCode in this.mouseButtonClickHandler.Keys)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    this.lastHitCollider = this.RayCast(out Collider2D collider) ? collider : null;
                }
                else if (Input.GetKeyUp(keyCode))
                {
                    if (this.IsBlockedByUI())
                    {
                        this.TargetClickListeners?.Invoke(null);
                        this.DisableTargetingMode();
                    }
                    else if (this.RayCast(out Collider2D collider) && collider.Equals(lastHitCollider))
                    {
                        if (this.TargetClickListeners != null)
                        {
                            collider.TryGetComponent(out ITargetable targetable);
                            this.TargetClickListeners.Invoke(targetable);
                            this.DisableTargetingMode();
                        }
                        else
                        {
                            this.mouseButtonClickHandler[keyCode].Invoke(collider);
                        }
                    }
                    else
                    {
                        this.OnNothingClickedEvent?.Invoke(keyCode.ConvertToMouseButton());
                    }

                    this.lastHitCollider = null;
                }
            }
        }

        /// <summary>
        /// Checks if the next click is on a <see cref="ITargetable"/>. If so it returns it in the callback, null otherwise.
        /// </summary>
        /// <param name="callback"> Callback that notifies whether or not the next click is a <see cref="ITargetable"/>.</param>
        /// <param name="skipFrameUpdate"> 
        /// Since EventSystem listeners will get notified of UI clicks before InputController, we skip a frame to avoid having 
        /// targeting mode being disabled on the same click that enabled it. This however shouldn't be necessary if the targeting
        /// is being triggered via a keyboard shortcut.
        /// </param>
        public void RegisterCheckForTargetOnNextClick(Action<ITargetable> callback, bool skipFrameUpdate)
        {
            this.skipFrame = skipFrameUpdate;

            if (this.TargetClickListeners == null)
            {
                Debug.Log("targeting mode enabled");
            }

            this.TargetClickListeners += callback;

            // TBI: change cursor icon to crosshair
        }

        public void CancelCheckForTargetOnNextClick(Action<ITargetable> callback)
        {
            if (this.TargetClickListeners == null)
            {
                return;
            }

            this.TargetClickListeners -= callback;

            if (this.TargetClickListeners == null)  // if null, this was the last or only callback
            {
                this.TargetClickListeners += callback;
                this.DisableTargetingMode();
            }
        }

        private void DisableTargetingMode()
        {
            if (this.TargetClickListeners != null)
            {
                this.TargetClickListeners = null;

                Debug.Log("targeting mode disabled");

                // TBI: change cursor icon to regular
            }
        }

        private void OnLeftClick(Collider2D collider)
        {
            if (collider.TryGetComponent(out ILeftClickHandler handler))
            {
                handler.OnLeftClick();
            }
        }

        private void OnRightClick(Collider2D collider)
        {
            if (collider.TryGetComponent(out IRightClickHandler handler))
            {
                handler.OnRightClick();
            }
        }

        private bool IsBlockedByUI() => EventSystem.current.IsPointerOverGameObject();

        private bool RayCast(out Collider2D collider)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            collider = rayHit.collider;
            return collider != null;
        }
    }
}