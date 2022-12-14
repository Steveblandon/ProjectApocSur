namespace Projapocsur.Engine
{
    using Projapocsur.World;
    using UnityEngine;

    /// <summary>
    /// Temporary placeholder for initializing connections or triggers for which a robust system isn't in place yet.
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        public static readonly string className = nameof(GameInitializer);

        [SerializeField]
        private GameConfiguration config;

        [Tooltip("will be linked to portrait below, in the future this will be managed by an instance that deals with character creation.")]
        [SerializeField]
        private SimpleSelectable testCharacterAvatar;

        [Tooltip("will be linked to character above, in the future this will be managed by an instance that deals with character creation.")]
        [SerializeField]
        private SelectableUI testCharacterPortrait;

        [SerializeField]
        private ProjectileLauncher rangedWeapon;

        [SerializeField]
        private MeleeWeapon meleeWeapon;

        [Tooltip("targets hostile to player characters, currently used for Id management until a more robust system is in place.")]
        [SerializeField]
        private Damageable[] hostileTargets;
        private Character testCharacter;

        void Awake()
        {
            DefinitionFinder.Init();

            if (!this.DisableOnMissingReference(config, nameof(this.config), className))
            {
                GameMaster.Init(config);
            }

            LayerMasks.Validate();
        }

        void Start()
        {
            if (this.testCharacterAvatar == null || this.testCharacterPortrait == null || this.rangedWeapon == null || this.meleeWeapon == null)
            {
                Debug.LogError($"{className}: missing character reference(s) in {this.gameObject.name}.");
                return;
            }

            var relationsTracker = new RelationsTracker();

            if (hostileTargets != null)
            {
                int idIncrement = 0;
                foreach (var hostileTarget in hostileTargets)
                {
                    string Id = "npc-hostile-" + idIncrement++;
                    hostileTarget.UniqueID = Id;
                    relationsTracker.HostileIndividualsById.Add(Id);
                }
            }

            Body body = new Body(DefNameOf.Body.Human);
            ICoroutineHandler characterCoroutineHandler = new CoroutineHandler(this.StartCoroutine, this.StopCoroutine);
            this.testCharacter = new Character("testCharacter0", this.testCharacterAvatar, this.testCharacterPortrait, body, characterCoroutineHandler, relationsTracker);
            this.testCharacter.IsInPlayerFaction = true;
            this.testCharacter.RangedWeapon = rangedWeapon;
            this.testCharacter.MeleeWeapon = meleeWeapon;

            GameMaster.Instance.PlayerCharacterSelection.TrackCharacter(this.testCharacter);     // this should ultimately end up in whatever will be responsible for adding characters to player faction
        }
    }
}
