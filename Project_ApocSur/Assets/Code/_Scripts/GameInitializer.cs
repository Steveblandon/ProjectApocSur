namespace Projapocsur.Scripts
{
    using Projapocsur.World;
    using UnityEngine;

    /// <summary>
    /// Temporary placeholder for initializing connections or triggers for which a robust system isn't in place yet.
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        public static readonly string className = nameof(GameInitializer);

        [Tooltip("will be linked to portrait below, in the future this will be managed by an instance that deals with character creation.")]
        [SerializeField]
        private SimpleSelectable testCharacterAvatar;

        [Tooltip("will be linked to character above, in the future this will be managed by an instance that deals with character creation.")]
        [SerializeField]
        private SelectableUI testCharacterPortrait;

        [SerializeField]
        private ProjectileLauncher rangedWeapon;

        private Character testCharacter;

        void Awake()
        {
            DefinitionFinder.Init();
            GameMaster.Init();
        }

        void Start()
        {
            if (this.testCharacterAvatar == null || this.testCharacterPortrait == null || this.rangedWeapon == null)
            {
                Debug.LogError($"{className}: missing character reference(s) in {this.gameObject.name}.");
            }
            else
            {
                Body body = new Body(DefNameOf.Body.Human);
                ICoroutineHandler characterCoroutineHandler = new CoroutineHandler(this.StartCoroutine, this.StopCoroutine);
                this.testCharacter = new Character(this.testCharacterAvatar, this.testCharacterPortrait, body, characterCoroutineHandler);
                this.testCharacter.IsInPlayerFaction = true;
                this.testCharacter.RangedWeapon = rangedWeapon;
            }
        }
    }
}
