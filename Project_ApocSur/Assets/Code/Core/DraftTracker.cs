namespace Projapocsur
{
    using System;
    using Projapocsur.World;

    public class DraftTracker
    {
        public static readonly string className = nameof(DraftTracker);

        public event Action<bool?> OnDraftStateChangeEvent;

        public bool? SelecteesDrafted
        {
            get { return this._selecteesDrafted; }
            private set
            {
                bool valueChanged = this._selecteesDrafted != value;
                this._selecteesDrafted = value;

                if (valueChanged)
                {
                    OnDraftStateChangeEvent?.Invoke(value);
                }
            }
        }

        private bool? _selecteesDrafted;

        public DraftTracker()
        {
            Action<Character> onDraftStateChangeEvent = (character) => this.SelecteesDrafted = character.IsDrafted;
            Character.OnDraftStateChangeEvent += onDraftStateChangeEvent;
            Character.OnSelectStateChangeEvent += this.OnCharacterSelectStateChangeEvent;
        }

        private void OnCharacterSelectStateChangeEvent(Character character)
        {
            if (character.IsInPlayerFaction)
            {
                if (character.IsSelected)
                {
                    if (this.SelecteesDrafted == null || this.SelecteesDrafted != null && character.IsDrafted == false)
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
}
