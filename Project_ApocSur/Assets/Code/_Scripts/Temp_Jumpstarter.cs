namespace Projapocsur.Scripts


{
    using Projapocsur.Things;
    using UnityEngine;

    /// <summary>
    /// Temporary placeholder for initializing connections or triggers for which a robust system isn't in place yet.
    /// </summary>
    public class Temp_Jumpstarter : MonoBehaviour
    {
        public static readonly string className = nameof(Temp_Jumpstarter);

        [Tooltip("will be linked to portrait below, in the future this will be managed by an instance that deals with character creation.")]
        [SerializeField]
        private SimpleSelectable characterAvatar;

        [Tooltip("will be linked to character above, in the future this will be managed by an instance that deals with character creation.")]
        [SerializeField]
        private SelectableUI characterPortrait;

        private Character testCharacter;

        void Start()
        {
            if (this.characterAvatar == null || this.characterPortrait == null)
            {
                Debug.LogError($"{className}: missing character reference(s) in {this.gameObject.name}.");
            }
            else
            {
                this.testCharacter = new Character(this.characterAvatar, this.characterPortrait);
                this.testCharacter.IsInPlayerFaction = true;
            }
        }
    }
}
