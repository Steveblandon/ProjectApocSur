namespace Projapocsur.Scripts
{
    using Projapocsur.World;
    using UnityEngine;

    public class HealthWindowManager : MonoBehaviour
    {
        public const string CompName = nameof(HealthWindowManager);

        [SerializeField]
        private StatBarController mainHitPointsBar;

        [SerializeField]
        private StatBarController bloodLossBar;

        [SerializeField]
        private BodyOutlineManager bodyOutlineManager;

        [SerializeField]
        private BodyPartInfoManager bodyPartInfoManager;

        private Character currentCharacter;

        void Start()
        {
            this.DisableOnMissingReference(this.mainHitPointsBar, nameof(this.mainHitPointsBar), CompName);
            this.DisableOnMissingReference(this.bloodLossBar, nameof(this.mainHitPointsBar), CompName);
            this.DisableOnMissingReference(this.bodyOutlineManager, nameof(this.bodyOutlineManager), CompName);
            this.DisableOnMissingReference(this.bodyPartInfoManager, nameof(this.bodyPartInfoManager), CompName);

            if (this.isActiveAndEnabled)
            {
                GameManager.Instance.CharacterSelectionTracker.OnSelectionChangeEvent += this.OnCurrentCharacterSelectChangeEventHandler;
                this.bodyOutlineManager.OnSelectedBodyPartChangeEvent += this.OnSelectedBodyPartChangeEventHandler;
            }
        }

        void OnDestroy()
        {
            GameManager.Instance.CharacterSelectionTracker.OnSelectionChangeEvent -= this.OnCurrentCharacterSelectChangeEventHandler;
            this.bodyOutlineManager.OnSelectedBodyPartChangeEvent -= this.OnSelectedBodyPartChangeEventHandler;
        }

        private void OnCurrentCharacterSelectChangeEventHandler(Character character)
        {
            if (this.currentCharacter != character && character != null)
            {
                this.mainHitPointsBar.Stat = character.Body.HitPointsPercentage;
                this.bloodLossBar.Stat = character.Body.BloodLoss;
                this.currentCharacter = character;
                this.bodyOutlineManager.SetBodyParts(character.Body.BodyParts);
            }
        }

        private void OnSelectedBodyPartChangeEventHandler(BodyPart bodyPart)
        {
            this.bodyPartInfoManager.BodyPart = bodyPart;
        }
    }
}
