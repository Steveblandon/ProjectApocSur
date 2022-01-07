namespace Projapocsur.Engine
{
    using System;
    using System.Collections;
    using Projapocsur.World;
    using UnityEngine;

    [RequireComponent(typeof(ToggleUI))]
    public abstract class PlayerCharacterEventToggleControllerBase : MonoBehaviour
    {
        protected const string CompName = nameof(CharacterPropToggleControllerBase);

        private ToggleUI toggleButton;

        [SerializeField]
        private float DelayedToggleOffSeconds = 0.01f;

        protected abstract Action<Character> TriggerEvent { get; }

        protected virtual void Start()
        {
            if (!this.DisableOnMissingReference(this.TriggerEvent, nameof(this.TriggerEvent), CompName))
            {
                this.toggleButton = this.GetComponent<ToggleUI>();
                this.toggleButton.OnSelectStateChangeEvent += this.OnToggleStateChangeEvent;
            }
        }

        protected virtual void OnDestroy()
        {
            if (this.toggleButton != null)
            {
                this.toggleButton.OnSelectStateChangeEvent -= this.OnToggleStateChangeEvent;
            }
        }

        private void OnToggleStateChangeEvent(Selectable simpleSelectable)
        {
            if (this.toggleButton.IsSelected)
            {
                foreach (Character character in GameMaster.Instance.PlayerCharacterSelection.All)
                {
                    this.TriggerEvent(character);
                    this.StartCoroutine(this.DelayedToggleOffRoutine());
                }
            }
        }

        private IEnumerator DelayedToggleOffRoutine()
        {
            yield return new WaitForSeconds(this.DelayedToggleOffSeconds);
            
            if (this.toggleButton.IsSelected)
            {
                this.toggleButton.Toggle();
            }
        }
    }
}