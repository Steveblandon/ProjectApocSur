namespace Projapocsur
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.World;

    public class CharacterSelectionTracker
    {
        public event Action SelectionChangedEvent;

        private HashSet<Character> selectees;
        private BooleanPropTracker draftTracker;

        public CharacterSelectionTracker()
        {
            this.selectees = new HashSet<Character>();
            this.draftTracker = new BooleanPropTracker();
        }

        public Character Current { get; protected set; }

        public IReadOnlyCollection<Character> All => this.selectees;

        public IProp<bool> IsDrafted => this.draftTracker.Latest;

        public void TrackCharacter(Character character)
        {
            character.IsSelected.ValueChangedEvent += this.OnCharacterSelectStateChangeEvent;
            draftTracker.Track(character.IsDrafted);
            this.OnCharacterSelectStateChangeEvent(character.IsSelected);
        }

        public void UntrackCharacter(Character character)
        {
            character.IsSelected.ValueChangedEvent -= this.OnCharacterSelectStateChangeEvent;
            draftTracker.Untrack(character.IsDrafted);
        }

        private void OnCharacterSelectStateChangeEvent(IParentedProp<bool, Character> selectionProp)
        {
            Character character = selectionProp.Parent;

            if (character.IsSelected.Value && !this.selectees.Contains(character))
            {
                this.Current = this.selectees.Count == 0 ? character : null;
                this.selectees.Add(character);
                this.UpdatePropTrackers(character);
                SelectionChangedEvent?.Invoke();
            }
            else if (!character.IsSelected.Value && this.selectees.Count > 0)
            {
                this.selectees.Remove(character);

                if (this.selectees.Count == 0)
                {
                    this.Current = null;
                }

                SelectionChangedEvent?.Invoke();
            }
        }

        private void UpdatePropTrackers(Character character)
        {
            if (draftTracker.Latest.Value)
            {
                draftTracker.Latest.Value = character.IsDrafted.Value;
            }
        }
    }

    public class BooleanPropTracker
    {
        public Prop<bool> Latest { get; private set; } = new Prop<bool>(false);

        public void Track(IProp<bool> prop) => prop.ValueChangedEvent += this.OnStateChangeEvent;

        public void Untrack(IProp<bool> prop) => prop.ValueChangedEvent -= this.OnStateChangeEvent;

        private void OnStateChangeEvent(IProp<bool> prop) => this.Latest.Value = prop.Value;
    }
}
