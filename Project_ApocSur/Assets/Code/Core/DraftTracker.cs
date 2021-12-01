namespace Projapocsur
{
    using System;
    using Projapocsur.World;

    public class DraftTracker : IEventListener
    {
        public static readonly string className = nameof(DraftTracker);

        public event Action<bool?> DraftStateChangedEvent;

        private bool? _selecteesDrafted;

        public DraftTracker()
        {
            Character.DraftStateChangedEvent += this.OnDraftStateChangeEvent;
            Character.SelectStateChangedEvent += this.OnCharacterSelectStateChangeEvent;
        }

        public bool? SelecteesDrafted
        {
            get { return this._selecteesDrafted; }
            private set
            {
                bool valueChanged = this._selecteesDrafted != value;
                this._selecteesDrafted = value;

                if (valueChanged)
                {
                    this.DraftStateChangedEvent?.Invoke(value);
                }
            }
        }

        public void OnDestroy()
        {
            Character.DraftStateChangedEvent -= this.OnDraftStateChangeEvent;
            Character.SelectStateChangedEvent -= this.OnCharacterSelectStateChangeEvent;
        }

        private void OnDraftStateChangeEvent(Character character) => this.SelecteesDrafted = character.IsDrafted;

        private void OnCharacterSelectStateChangeEvent(Character character)
        {
            if (!character.IsInPlayerFaction)
            {
                return;
            }

            if (character.IsSelected)
            {
                if (this.SelecteesDrafted == null || this.SelecteesDrafted != null && character.IsDrafted == false)     // i.e. can go from 'null' or 'false' to 'false', but not from 'false' to 'true'
                {
                    this.SelecteesDrafted = character.IsDrafted;
                }
            }
            else if (!character.IsSelected && GameMaster.Instance.CharacterSelectionTracker.Selectees.Count == 0)
            {
                this.SelecteesDrafted = null;
            }
        }
    }
}
