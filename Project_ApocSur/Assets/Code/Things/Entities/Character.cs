namespace Projapocsur.Things
{
    using System;
    using Projapocsur.Scripts;
    using Projapocsur.Common;
    using UnityEngine;

    public class Character
    {
        public const string ClassName = nameof(Character);

        private Selectable avatar;
        private Selectable portrait;
        private Moveable mover;
        private bool _isDrafted;

        public Character(SimpleSelectable avatar, SelectableUI portrait)
        {
            ValidationUtility.ThrowIfNull(nameof(avatar), avatar);
            ValidationUtility.ThrowIfNull(nameof(portrait), portrait);

            this.mover = avatar.GetComponent<Moveable>();
            this.avatar = avatar;
            this.portrait = portrait;
            this.avatar.OnSelectStateChangeEvent += this.OnCompSelectStateChangeEvent;
            this.portrait.OnSelectStateChangeEvent += this.OnCompSelectStateChangeEvent;

            // TODO: on character destruction make sure to unregister from all these events.
        }

        public static event Action<Character> OnSelectStateChangeEvent;
        public static event Action<Character> OnDraftStateChangeEvent;

        public bool IsInPlayerFaction { get; set; }
        public bool IsSelected { get; private set; }
        public bool IsDrafted 
        { 
            get => this._isDrafted;
            set
            {
                bool valueChanged = this._isDrafted != value;
                this._isDrafted = value;

                if (valueChanged)
                {
                    OnDraftStateChangeEvent?.Invoke(this);
                }
            }
        }

        private void OnCompSelectStateChangeEvent(Selectable simpleSelectable)
        {
            if (this.IsSelected && !simpleSelectable.IsSelected)
            {
                this.IsSelected = false;
                InputController.OnNothingClickedEvent -= this.OnNothingClickedEvent;
                this.avatar.OnDeselect();
                this.portrait.OnDeselect();
                OnSelectStateChangeEvent?.Invoke(this);
            }
            else if (!this.IsSelected && simpleSelectable.IsSelected)
            {
                this.IsSelected = true;
                InputController.OnNothingClickedEvent += this.OnNothingClickedEvent;
                this.avatar.OnSelect();
                this.portrait.OnSelect();
                OnSelectStateChangeEvent?.Invoke(this);
            }
        }

        private void OnNothingClickedEvent(InputController.MouseInput mouseInput)
        {
            if (this.IsSelected)
            {
                if (mouseInput == InputController.MouseInput.Left)
                {
                    this.avatar.OnDeselect();
                    this.portrait.OnDeselect();
                }
                else if (mouseInput == InputController.MouseInput.Right && this.IsDrafted)
                {
                    if (this.mover != null)
                    {
                        this.mover.MoveToMousePosition();
                    }
                    else
                    {
                        Debug.Log($"{ClassName}: attempted to move to mouse position, but no {nameof(Moveable)} comp was found on {this.avatar.name}.");
                    }
                }
            }
        }
    }
}