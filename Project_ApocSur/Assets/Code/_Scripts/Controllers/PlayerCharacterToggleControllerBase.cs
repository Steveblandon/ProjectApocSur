namespace Projapocsur.Scripts
{
    using System;
    using Projapocsur.World;
    using UnityEngine;

    [RequireComponent(typeof(ToggleUI))]
    public abstract class PlayerCharacterToggleControllerBase : MonoBehaviour
    {
        private const string CompName = nameof(PlayerCharacterToggleControllerBase);

        private ToggleUI toggleButton;

        protected abstract IProp<bool> TrackedProp { get; }

        protected abstract Func<Character, Prop<bool>> AffectedCharacterProp { get; }

        protected virtual void Start()
        {
            this.toggleButton = this.GetComponent<ToggleUI>();
            this.toggleButton.OnSelectStateChangeEvent += this.OnToggleStateChangeEvent;

            if (!this.DisableOnMissingReference(this.TrackedProp, nameof(this.TrackedProp), CompName)
                && !this.DisableOnMissingReference(this.AffectedCharacterProp, nameof(this.AffectedCharacterProp), CompName))
            {
                this.TrackedProp.ValueChangedEvent += this.OnPropStateChangeEvent;
                this.OnPropStateChangeEvent(this.TrackedProp);
            }
        }

        protected virtual void OnDestroy()
        {
            this.toggleButton.OnSelectStateChangeEvent -= this.OnToggleStateChangeEvent;
            
            if (this.TrackedProp != null)
            {
                this.TrackedProp.ValueChangedEvent -= this.OnPropStateChangeEvent;
            }
        }

        private void OnToggleStateChangeEvent(Selectable simpleSelectable)
        {
            if (this.toggleButton.IsSelected && !this.TrackedProp.Value)
            {
                foreach (Character character in GameMaster.Instance.PlayerCharacterSelection.All)
                {
                    this.AffectedCharacterProp(character).Value = true;
                }
            }
            else if (!this.toggleButton.IsSelected && this.TrackedProp.Value)
            {
                foreach (Character character in GameMaster.Instance.PlayerCharacterSelection.All)
                {
                    this.AffectedCharacterProp(character).Value = false;
                }
            }
        }

        private void OnPropStateChangeEvent(IProp<bool> prop)
        {
            if ((this.toggleButton.IsSelected && !this.TrackedProp.Value) || (!this.toggleButton.IsSelected && this.TrackedProp.Value))
            {
                this.toggleButton.Toggle();
            }
        }
    }
}