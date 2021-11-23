namespace Projapocsur.World
{
    using System;
    using Projapocsur.Scripts;
    using UnityEngine;

    public class Character
    {
        public const string ClassName = nameof(Character);

        private Selectable avatar;
        private Selectable portrait;
        private Moveable mover;
        private bool _isDrafted;

        public Character(SimpleSelectable avatar, SelectableUI portrait, Body body)
        {
            ValidationUtility.ThrowIfNull(nameof(avatar), avatar);
            ValidationUtility.ThrowIfNull(nameof(portrait), portrait);
            ValidationUtility.ThrowIfNull(nameof(body), body);

            this.mover = avatar.GetComponent<Moveable>();
            this.avatar = avatar;
            this.portrait = portrait;
            this.avatar.OnSelectStateChangeEvent += this.OnCompSelectStateChangeEvent;
            this.portrait.OnSelectStateChangeEvent += this.OnCompSelectStateChangeEvent;

            // TODO: on character destruction make sure to unregister from all these events.

            this.Body = body;
        }

        public static event Action<Character> OnSelectStateChangeEvent;
        public static event Action<Character> OnDraftStateChangeEvent;

        public Body Body { get; }

        public bool IsInPlayerFaction { get; set; }
        
        public bool IsSelected { get; protected set; }
        
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

        public void EngageTarget(IDamageable damageable, EngagementMode engagementMode)
        {
            // TBI

            switch(engagementMode)
            {
                case EngagementMode.Shoot:
                    Debug.Log($"{this.avatar.name} shoots target");
                    damageable.TakeDamage(null);
                    break;
            }
        }

        private void OnCompSelectStateChangeEvent(Selectable simpleSelectable)
        {
            if (this.IsSelected && !simpleSelectable.IsSelected)
            {
                this.IsSelected = false;
                InputController.Main.OnNothingClickedEvent -= this.OnNothingClickedEvent;
                this.avatar.OnDeselect();
                this.portrait.OnDeselect();
                OnSelectStateChangeEvent?.Invoke(this);
            }
            else if (!this.IsSelected && simpleSelectable.IsSelected)
            {
                this.IsSelected = true;
                InputController.Main.OnNothingClickedEvent += this.OnNothingClickedEvent;
                this.avatar.OnSelect();
                this.portrait.OnSelect();
                OnSelectStateChangeEvent?.Invoke(this);
            }
        }

        private void OnNothingClickedEvent(KeyCode mouseInput)
        {
            if (this.IsSelected)
            {
                if (mouseInput == KeyCode.Mouse0)
                {
                    this.avatar.OnDeselect();
                    this.portrait.OnDeselect();
                }
                else if (mouseInput == KeyCode.Mouse1 && this.IsDrafted)
                {
                    if (this.mover != null)
                    {
                        this.mover.MoveToMousePosition(3);      // TODO: change value to movement speed stat value
                    }
                    else
                    {
                        Debug.Log($"{ClassName}: attempted to move to mouse position, but no {nameof(Moveable)} comp was found on {this.avatar.name}.");
                    }
                }
            }
        }
    }

    public enum EngagementMode
    {
        Shoot,
        Melee,
        TrackAndEliminate
    }
}