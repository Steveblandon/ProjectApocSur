namespace Projapocsur.Core
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
            _selectees = new HashSet<Character>();
            Character.OnSelectStateChangeEvent += OnCharacterSelectStateChangeEvent;
        }

        public Character MainSelelectee { get; private set; }
        public IReadOnlyCollection<Character> Selectees { get { return _selectees; } }

        private void OnCharacterSelectStateChangeEvent(Character character)
        {
            if (character.IsInPlayerFaction)
            {
                if (character.IsSelected)
                {
                    MainSelelectee = _selectees.Count == 0 ? character : null;
                    _selectees.Add(character);
                    OnSelectionChangeEvent?.Invoke(MainSelelectee);
                }
                else if (!character.IsSelected)
                {
                    _selectees.Remove(character);

                    if (_selectees.Count == 0)
                    {
                        MainSelelectee = null;
                    }
                    OnSelectionChangeEvent?.Invoke(MainSelelectee);
                }
            }
        }
    }
}
