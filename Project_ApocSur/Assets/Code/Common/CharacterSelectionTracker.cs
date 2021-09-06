namespace Projapocsur.Common
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.Entities;

    public class CharacterSelectionTracker
    {
        public static readonly string className = nameof(CharacterSelectionTracker);

        public event Action<Character> OnSelectionChangeEvent;

        private HashSet<Character> _selectees;

        public CharacterSelectionTracker()
        {
            this._selectees = new HashSet<Character>();
            Character.OnSelectStateChangeEvent += this.OnCharacterSelectStateChangeEvent;
        }

        public Character MainSelelectee { get; private set; }
        public IReadOnlyCollection<Character> Selectees { get { return this._selectees; } }

        private void OnCharacterSelectStateChangeEvent(Character character)
        {
            if (character.IsInPlayerFaction)
            {
                if (character.IsSelected)
                {
                    this.MainSelelectee = this._selectees.Count == 0 ? character : null;
                    this._selectees.Add(character);
                    this.OnSelectionChangeEvent?.Invoke(this.MainSelelectee);
                }
                else if (!character.IsSelected)
                {
                    this._selectees.Remove(character);

                    if (this._selectees.Count == 0)
                    {
                        this.MainSelelectee = null;
                    }
                    this.OnSelectionChangeEvent?.Invoke(this.MainSelelectee);
                }
            } 
        }
    }
}
