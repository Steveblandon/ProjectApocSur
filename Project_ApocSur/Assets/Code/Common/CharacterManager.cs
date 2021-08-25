namespace Projapocsur.Common
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.Behaviors;
    using UnityEngine;

    public class CharacterManager
    {
        public static readonly string className = nameof(CharacterManager);

        public static bool? SelecteesDrafted { get; private set; }

        public static CharacterManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CharacterManager();
                }
                return instance;
            }
            private set { instance = value; }
        }

        private static CharacterManager instance;
        private IDictionary<Character, CharacterPortrait> characterToPortraitMap;
        private IDictionary<CharacterPortrait, Character> portraitToCharacterMap;
        private ISet<Character> selectees;
        private Action deselectCallbacks;

        private CharacterManager() 
        {
            this.characterToPortraitMap = new Dictionary<Character, CharacterPortrait>();
            this.portraitToCharacterMap = new Dictionary<CharacterPortrait, Character>();
            this.selectees = new HashSet<Character>();

            EventManager.Instance.RegisterListener(EventType.WO_NothingClicked_Left, this.OnFullDeselection);
        }

        public void Add(Character character, CharacterPortrait characterPortrait)
        {
            this.characterToPortraitMap[character] = characterPortrait;
            this.portraitToCharacterMap[characterPortrait] = character;
        }

        public void RegisterSelection(Character character)
        {
            if (this.characterToPortraitMap.TryGetValue(character, out CharacterPortrait portrait))
            {
                if (!selectees.Contains(character))
                {
                    this.AddSelectee(character, portrait);
                    portrait.OnSelect();
                }
            }
            else
            {
                Debug.LogWarning($"{className}: attempted to select {character.name}, but it's not managed.");
            }
        }

        public void RegisterSelection(CharacterPortrait portrait)
        {
            if (this.portraitToCharacterMap.TryGetValue(portrait, out Character character))
            {
                if (!selectees.Contains(character))
                {
                    this.AddSelectee(character, portrait);
                    character.OnSelect();
                }
            }
            else
            {
                Debug.LogWarning($"{className}: attempted to select {portrait.name}, but it's not managed.");
            }
        }

        public void DraftSelectees()
        {
            foreach (var selectee in this.selectees)
            {
                selectee.IsDrafted = true;
            }
        }

        public void UndraftSelectees()
        {
            foreach (var selectee in this.selectees)
            {
                selectee.IsDrafted = false;
            }
        }

        private void AddSelectee(Character character, CharacterPortrait portrait)
        {
            this.AddDeselectCallback(portrait.OnDeselect);
            this.AddDeselectCallback(character.OnDeselect);
            selectees.Add(character);

            if (SelecteesDrafted == null || SelecteesDrafted == true)
            {
                SelecteesDrafted = character.IsDrafted;
            }

            if (!DraftAction.Instance.gameObject.activeSelf)
            {
                DraftAction.Instance.gameObject.SetActive(true);
            }

            if (SelecteesDrafted == true)
            {
                DraftAction.Instance.TurnOn();
            }
            else if (SelecteesDrafted == false)
            {
                DraftAction.Instance.TurnOff();
            }
        }

        private void OnFullDeselection()
        {
            if (selectees.Count > 0 && this.deselectCallbacks != null)
            {
                DraftAction.Instance.gameObject.SetActive(false);
                this.selectees.Clear();
                SelecteesDrafted = null;
                this.deselectCallbacks.Invoke();
                this.deselectCallbacks = null;
            }
        }

        private void AddDeselectCallback(Action callback)
        {
            if (this.deselectCallbacks == null)
            {
                this.deselectCallbacks = callback;
            }
            else
            {
                this.deselectCallbacks += callback;
            }
        }
    }
}
