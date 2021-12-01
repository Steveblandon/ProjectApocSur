namespace Projapocsur.World
{
    using System;
    using Projapocsur.Scripts;
    using UnityEngine;

    public class Character : IEventListener
    {
        public const string ClassName = nameof(Character);

        private Selectable avatar;
        private Selectable portrait;
        private Moveable moveable;
        private IDamageable damageable;
        private RelationsTracker relationsTracker;
        private ProximityScanner proximityScanner;
        private CombatProcessor combatProcessor;
        private ICoroutineHandler coroutineHandler;
        private bool _isDrafted;

        public Character(string id, SimpleSelectable avatar, SelectableUI portrait, Body body, ICoroutineHandler coroutineHandler, RelationsTracker relationsTracker)
        {
            ValidationUtility.ThrowIfNull(nameof(avatar), avatar);
            ValidationUtility.ThrowIfNull(nameof(portrait), portrait);
            ValidationUtility.ThrowIfNull(nameof(body), body);

            this.UniqueID = id;
            this.moveable = avatar.GetComponent<Moveable>();
            this.damageable = avatar.GetComponent<IDamageable>();
            this.damageable.UniqueID = id;
            this.avatar = avatar;
            this.portrait = portrait;
            this.Body = body;
            this.coroutineHandler = coroutineHandler;
            this.relationsTracker = relationsTracker;

            this.avatar.OnSelectStateChangeEvent += this.OnCompSelectStateChangeEvent;
            this.portrait.OnSelectStateChangeEvent += this.OnCompSelectStateChangeEvent;

            this.proximityScanner = new ProximityScanner(this.damageable);
            this.combatProcessor = new CombatProcessor(this.moveable, this.coroutineHandler, this.proximityScanner, this.relationsTracker);
        }

        public static event Action<Character> SelectStateChangedEvent;
        public static event Action<Character> DraftStateChangedEvent;

        public string UniqueID { get; }

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
                    if (value == true)
                    {
                        this.combatProcessor.EngageHostileTargets();
                    }
                    else
                    {
                        this.combatProcessor.Cease();
                    }

                    DraftStateChangedEvent?.Invoke(this);
                }
            }
        }

        public IRangedWeapon RangedWeapon { set => this.combatProcessor.RangedCombatDriver.RangedWeapon = value; }    // temporary direct assignment until inventory system is in place

        public void EngageTarget(IDamageable damageable, CombatEngagementMode engagementMode) => this.combatProcessor.EngageTarget(damageable, engagementMode);

        public void OnDestroy()
        {
            this.combatProcessor.OnDestroy();
            this.avatar.OnSelectStateChangeEvent -= this.OnCompSelectStateChangeEvent;
            this.portrait.OnSelectStateChangeEvent -= this.OnCompSelectStateChangeEvent;
            InputController.Main.OnNothingClickedEvent -= this.OnNothingClickedEvent;
        }

        private void OnCompSelectStateChangeEvent(Selectable simpleSelectable)
        {
            if (this.IsSelected && !simpleSelectable.IsSelected)
            {
                this.IsSelected = false;
                InputController.Main.OnNothingClickedEvent -= this.OnNothingClickedEvent;
                this.avatar.OnDeselect();
                this.portrait.OnDeselect();
                SelectStateChangedEvent?.Invoke(this);
            }
            else if (!this.IsSelected && simpleSelectable.IsSelected)
            {
                this.IsSelected = true;
                InputController.Main.OnNothingClickedEvent += this.OnNothingClickedEvent;
                this.avatar.OnSelect();
                this.portrait.OnSelect();
                SelectStateChangedEvent?.Invoke(this);
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
                    if (this.moveable != null)
                    {
                        this.moveable.MoveToMousePosition(3);      // TODO: change value to movement speed stat value
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