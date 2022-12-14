namespace Projapocsur.World
{
    using System;
    using Projapocsur.Engine;

    public class Character : IDisposable
    {
        public const string ClassName = nameof(Character);

        private Selectable avatar;
        private Selectable portrait;
        private IMoveable moveable;
        private IDamageable damageable;
        private RelationsTracker relationsTracker;
        private ProximityScanner proximityScanner;
        private CombatProcessor combatProcessor;
        private ICoroutineHandler coroutineHandler;
        private Prop<bool> isSelected;
        private Prop<bool> isDrafted;
        private Prop<bool> isAutoAttackEnabled;

        public Character(string id, SimpleSelectable avatar, SelectableUI portrait, Body body, ICoroutineHandler coroutineHandler, RelationsTracker relationsTracker)
        {
            ValidationUtility.ThrowIfNull(nameof(avatar), avatar);
            ValidationUtility.ThrowIfNull(nameof(portrait), portrait);
            ValidationUtility.ThrowIfNull(nameof(body), body);

            this.UniqueID = id;
            this.moveable = avatar.GetComponent<IMoveable>();
            this.damageable = avatar.GetComponent<IDamageable>();
            this.damageable.UniqueID = id;
            this.avatar = avatar;
            this.portrait = portrait;
            this.Body = body;
            this.coroutineHandler = coroutineHandler;
            this.relationsTracker = relationsTracker;

            this.isSelected = new Prop<bool>(this.avatar.IsSelected && this.portrait.IsSelected, this);
            this.isDrafted = new Prop<bool>(false, this, OnValueChangePriorityCallback: this.OnDraftStateChangeEvent);
            this.isAutoAttackEnabled = new Prop<bool>(true, this, OnValueChangePriorityCallback: this.OnAutoAttackStateChangeEvent);

            this.avatar.OnSelectStateChangeEvent += this.OnCompSelectStateChangeEvent;
            this.portrait.OnSelectStateChangeEvent += this.OnCompSelectStateChangeEvent;

            this.proximityScanner = new ProximityScanner(this.damageable);
            this.combatProcessor = new CombatProcessor(this.moveable, this.coroutineHandler, this.proximityScanner, this.relationsTracker);
        }

        public string UniqueID { get; }

        public Body Body { get; }

        public bool IsInPlayerFaction { get; set; }

        public IParentedProp<bool, Character> IsSelected => this.isSelected;

        public Prop<bool> IsDrafted => this.isDrafted;

        public Prop<bool> IsAutoAttackEnabled => this.isAutoAttackEnabled;

        public IRangedWeapon RangedWeapon { set => this.combatProcessor.RangedCombatDriver.Weapon = value; }    // temporary direct assignment until inventory system is in place

        public IMeleeWeapon MeleeWeapon { set => this.combatProcessor.MeleeCombatDriver.Weapon = value; }    // temporary direct assignment until inventory system is in place

        public void EngageTarget(IDamageable damageable, CombatEngagementMode engagementMode) => this.combatProcessor.EngageTarget(damageable, engagementMode);

        public void Cease()
        {
            if (this.combatProcessor.IsManualTargetingOverrideActive && this.combatProcessor.HasTarget)
            {
                LogUtility.Log(LogLevel.Info, $"Cease acknowledged, disengaging target... [character='{this.UniqueID}'");
                this.combatProcessor.DisengageTarget();
                this.OnAutoAttackStateChangeEvent();    // revert to auto-attacking if that's enabled... not disabling auto-attack here allows for finer control
            }
            else if (this.moveable.CurrentDirective != MoveDirective.None)
            {
                LogUtility.Log(LogLevel.Info, $"Cease acknowledged, movement stopped... [character='{this.UniqueID}'");
                this.moveable.CancelCurrentDirective();
            }
        }

        public void Dispose()
        {
            this.combatProcessor.Dispose();
            this.avatar.OnSelectStateChangeEvent -= this.OnCompSelectStateChangeEvent;
            this.portrait.OnSelectStateChangeEvent -= this.OnCompSelectStateChangeEvent;
            InputController.Main.OnNothingClickedEvent -= this.OnNothingClickedEvent;
        }

        private void OnCompSelectStateChangeEvent(Selectable simpleSelectable)
        {
            if (this.isSelected.Value && !simpleSelectable.IsSelected)
            {
                InputController.Main.OnNothingClickedEvent -= this.OnNothingClickedEvent;
                this.isSelected.Value = false;
                this.avatar.OnDeselect();
                this.portrait.OnDeselect();
            }
            else if (!this.isSelected.Value && simpleSelectable.IsSelected)
            {
                InputController.Main.OnNothingClickedEvent += this.OnNothingClickedEvent;
                this.isSelected.Value = true;
                this.avatar.OnSelect();
                this.portrait.OnSelect();
            }
        }

        private void OnNothingClickedEvent(MouseButton mouseInput)
        {
            if (this.IsSelected.Value)
            {
                if (mouseInput == MouseButton.Left)
                {
                    this.avatar.OnDeselect();
                    this.portrait.OnDeselect();
                }
                else if (mouseInput == MouseButton.Right && this.isDrafted.Value == true)
                {
                    if (this.moveable != null)
                    {
                        if (this.combatProcessor.IsManualTargetingOverrideActive)
                        {
                            this.combatProcessor.Cease();
                            this.OnAutoAttackStateChangeEvent();
                        }
                        
                        this.moveable.MoveToMousePosition(3);      // TODO: change value to movement speed stat value
                    }
                    else
                    {
                        LogUtility.Log(LogLevel.Info, $"{ClassName}: attempted to move to mouse position, but no {nameof(Moveable)} comp was found on {this.avatar.name}.");
                    }
                }
            }
        }

        private void OnDraftStateChangeEvent()
        {
            this.OnAutoAttackStateChangeEvent();
        }

        private void OnAutoAttackStateChangeEvent()
        {
            if (this.isDrafted.Value && this.isAutoAttackEnabled.Value)
            {
                this.combatProcessor.EngageHostileTargets();
            }
            else
            {
                this.combatProcessor.Cease();
            }
        }

        public class Prop<TValue> : ParentedProp<TValue, Character> where TValue : IComparable<TValue>
        {
            public Prop(TValue initialValue, Character parent, Action OnValueChangePriorityCallback = null)
                : base(initialValue, parent, OnValueChangePriorityCallback)
            {
            }
        }
    }
}