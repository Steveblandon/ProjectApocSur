namespace Projapocsur.Core
{
    using System;
    using Projapocsur.Entities;

    public class DraftTracker
    {
        public static readonly string className = nameof(DraftTracker);

        public event Action<bool?> OnDraftStateChangeEvent;

        public bool? SelecteesDrafted
        {
            get { return _selecteesDrafted; }
            private set
            {
                bool valueChanged = _selecteesDrafted != value;
                _selecteesDrafted = value;

                if (valueChanged)
                {
                    OnDraftStateChangeEvent?.Invoke(value);
                }
            }
        }

        private bool? _selecteesDrafted;

        public DraftTracker()
        {
            Action<Character> onDraftStateChangeEvent = (character) => SelecteesDrafted = character.IsDrafted;
            Character.OnDraftStateChangeEvent += onDraftStateChangeEvent;
            Character.OnSelectStateChangeEvent += OnCharacterSelectStateChangeEvent;
        }

        private void OnCharacterSelectStateChangeEvent(Character character)
        {
            if (character.IsInPlayerFaction)
            {
                if (character.IsSelected)
                {
                    if (SelecteesDrafted == null || SelecteesDrafted != null && character.IsDrafted == false)
                    {
                        SelecteesDrafted = character.IsDrafted;
                    }
                }
                else if (!character.IsSelected && GameManager.Instance.CharacterSelectionTracker.Selectees.Count == 0)
                {
                    SelecteesDrafted = null;
                }
            }
        }
    }
}
