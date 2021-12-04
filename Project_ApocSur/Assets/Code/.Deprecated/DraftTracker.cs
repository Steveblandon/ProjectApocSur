namespace Projapocsur
{
    using System;
    using Projapocsur.World;

    [Obsolete]
    public class DraftTracker
    {
        private Prop<bool> currentSelecteesAreDrafted;

        public DraftTracker()
        {
            this.currentSelecteesAreDrafted = new Prop<bool>(true);
        }

        public IProp<bool> CurrentSelecteesAreDrafted => this.currentSelecteesAreDrafted;

        public void TrackCharacter(Character character)
        {
            character.IsSelected.ValueChangedEvent += this.OnCharacterSelectStateChangeEvent;
            character.IsDrafted.ValueChangedEvent += this.OnDraftStateChangeEvent;
        }

        public void UntrackCharacter(Character character)
        {
            character.IsSelected.ValueChangedEvent -= this.OnCharacterSelectStateChangeEvent;
            character.IsDrafted.ValueChangedEvent -= this.OnDraftStateChangeEvent;
        }

        private void OnDraftStateChangeEvent(IParentedProp<bool, Character> selectionProp) => this.currentSelecteesAreDrafted.Value = selectionProp.Parent.IsDrafted.Value;

        private void OnCharacterSelectStateChangeEvent(IParentedProp<bool, Character> selectionProp)
        {
            Character character = selectionProp.Parent;

            if (character.IsSelected.Value && this.currentSelecteesAreDrafted.Value)
            {
                this.currentSelecteesAreDrafted.Value = character.IsDrafted.Value;
            }
        }
    }
}
