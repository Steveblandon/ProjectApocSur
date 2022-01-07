namespace Projapocsur
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.World;

    public class CharacterSelectionTracker
    {
        public event Action SelectionChangedEvent;

        private HashSet<Character> selectees;
        private BooleanPropTrackerManager propTrackersManager;

        public CharacterSelectionTracker()
        {
            this.selectees = new HashSet<Character>();
            this.propTrackersManager = new BooleanPropTrackerManager();
            this.propTrackersManager.AddNew(nameof(this.IsDrafted), false, character => character.IsDrafted);
            this.propTrackersManager.AddNew(nameof(this.IsAutoAttackEnabled), true, character => character.IsAutoAttackEnabled);
        }

        public Character Current { get; protected set; }

        public IReadOnlyCollection<Character> All => this.selectees;

        public IProp<bool> IsDrafted => this.propTrackersManager.Trackers[nameof(this.IsDrafted)].Latest;

        public IProp<bool> IsAutoAttackEnabled => this.propTrackersManager.Trackers[nameof(this.IsAutoAttackEnabled)].Latest;

        public void TrackCharacter(Character character)
        {
            this.propTrackersManager.Track(character);
            character.IsSelected.ValueChangedEvent += this.OnCharacterSelectStateChangeEvent;
            this.OnCharacterSelectStateChangeEvent(character.IsSelected);
        }

        public void UntrackCharacter(Character character)
        {
            this.propTrackersManager.Untrack(character);
            character.IsSelected.ValueChangedEvent -= this.OnCharacterSelectStateChangeEvent;
        }

        private void OnCharacterSelectStateChangeEvent(IParentedProp<bool, Character> selectionProp)
        {
            Character character = selectionProp.Parent;

            if (character.IsSelected.Value && !this.selectees.Contains(character))
            {
                this.Current = this.selectees.Count == 0 ? character : null;
                this.selectees.Add(character);
                this.propTrackersManager.OnSelect(character);
            }
            else if (!character.IsSelected.Value && this.selectees.Count > 0)
            {
                this.selectees.Remove(character);

                if (this.selectees.Count == 0)
                {
                    this.Current = null;
                }
            }

            SelectionChangedEvent?.Invoke();
        }
    }
}
