namespace Projapocsur.Scripts
{
    using System.Collections.Generic;
    using Projapocsur.EditorAttributes;
    using Projapocsur.World;
    using UnityEngine;

    [RequireComponent(typeof(BodyPartInfoManager))]
    public class BodyPartInfoManagerTestTrigger : MonoBehaviour
    {
        [SerializeField]
        [Button(nameof(SetBodyPart1), ButtonWidth = 200)]
        private bool SetToBodyPart1;

        [SerializeField]
        [Button(nameof(SetBodyPart2), ButtonWidth = 200)]
        private bool SetToBodyPart2;

        [SerializeField]
        [ReadOnly]
        private string currentBodyPartName;

        [SerializeField]
        [Button(nameof(ApplyDamage), ButtonWidth = 200)]
        private bool Punch;

        [SerializeField]
        private float punchDamage;

        private BodyPart bodyPart1, bodyPart2, currentBodyPart; 
        private BodyPartInfoManager bodyPartInfoManager;
        private InjuryProcessingContext currentContext;
        private Stat pain, bloodLoss, healingRate;

        void Start()
        {
            DefinitionFinder.Init();

            this.bodyPartInfoManager = this.GetComponent<BodyPartInfoManager>();

            this.pain = new Stat(DefNameOf.Stat.Pain, 0, 100);
            this.bloodLoss = new Stat(DefNameOf.Stat.BloodLoss, 0, 100);
            this.healingRate = new Stat(DefNameOf.Stat.HealingRate, 5, 100);
            this.currentContext = new InjuryProcessingContext(pain, bloodLoss, healingRate);

            this.bodyPart1 = new BodyPart(DefNameOf.BodyPart.Human_Head);
            this.bodyPart2 = new BodyPart(DefNameOf.BodyPart.Human_Torso);
            this.bodyPart1.OnStart(this.currentContext);
            this.bodyPart2.OnStart(this.currentContext);

            this.enabled = false;
        }

        private void SetBodyPart1()
        {
            this.currentBodyPart = this.bodyPart1;
            this.bodyPartInfoManager.BodyPart = this.currentBodyPart;
            this.currentBodyPartName = this.currentBodyPart.Def.Label;
        }

        private void SetBodyPart2()
        {
            this.currentBodyPart = this.bodyPart2;
            this.bodyPartInfoManager.BodyPart = this.currentBodyPart;
            this.currentBodyPartName = this.currentBodyPart.Def.Label;
        }

        private void ApplyDamage()
        {
            if (this.currentBodyPart != null)
            {
                this.currentBodyPart.TakeDamage(this.punchDamage, new List<string>() { DefNameOf.Injury.Bruise });
                this.currentBodyPart.OnUpdate(this.currentContext);     // must call update, otherwise injuries won't transfer from new-list to established-list and therefore won't be visible on the UI when switching between bodyParts
            }
        }
    }
}
