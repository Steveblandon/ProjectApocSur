namespace Projapocsur.Common
{
    using System;
    using Projapocsur.Entities;

    public class DraftTracker
    {
        public static readonly string className = nameof(DraftTracker);

        public static DraftTracker Instance { get { return _instance; } }
        private static DraftTracker _instance = new DraftTracker();

        public bool? SelecteesDrafted 
        { 
            get { return this._selecteesDrafted;  }
            private set
            {
                bool valueChanged = this._selecteesDrafted != value;
                this._selecteesDrafted = value;

                if (valueChanged)
                {
                    EventManager.Instance.TriggerEvent(EventType.CM_CharacterDraftStateChanged);
                }
            }
        }

        private bool? _selecteesDrafted;
        private Action draftCallbacks;
        private Action undraftCallbacks;

        public void Add(PlayerCharacter character)
        {
            this.OnCharacterSelectStateChangeEvent(character);
            character.OnSelectStateChangeEvent += this.OnCharacterSelectStateChangeEvent;
        }

        public void DraftSelectees()
        {
            this.draftCallbacks?.Invoke();
            this.SelecteesDrafted = true;
        }

        public void UndraftSelectees()
        {
            this.undraftCallbacks?.Invoke();
            this.SelecteesDrafted = false;
        }

        private void OnCharacterSelectStateChangeEvent(PlayerCharacter character)
        {
            if (character.IsSelected)
            {
                this.draftCallbacks += character.OnDraft;
                this.undraftCallbacks += character.OnUndraft;

                if (this.SelecteesDrafted == null || (this.SelecteesDrafted != null && character.IsDrafted == false))
                {
                    this.SelecteesDrafted = character.IsDrafted;
                }
            }
            else
            {
                this.draftCallbacks -= character.OnDraft;
                this.undraftCallbacks -= character.OnUndraft;

                if (this.draftCallbacks == null && this.undraftCallbacks == null)
                {
                    this.SelecteesDrafted = null;
                }
            }

            EventManager.Instance.TriggerEvent(EventType.CM_CharacterSelectionStateChanged);
        }
    }
}
