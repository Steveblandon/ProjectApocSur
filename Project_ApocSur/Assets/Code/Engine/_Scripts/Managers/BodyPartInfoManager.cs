namespace Projapocsur.Engine
{
    using Projapocsur.World;
    using UnityEngine;
    using UnityEngine.UI;

    public class BodyPartInfoManager : MonoBehaviour
    {
        private const string CompName = nameof(BodyPartInfoManager);

        [SerializeField]
        private Text title;

        [SerializeField]
        private StatBarController hitpointsBar;

        [SerializeField]
        private InjuriesViewManager injuriesViewManager;

        private BodyPart bodyPart;

        public BodyPart BodyPart
        {
            get => this.bodyPart;
            set
            {
                if (!this.isActiveAndEnabled)
                {
                    return;
                }

                if (this.bodyPart != value)
                {
                    if (this.bodyPart != null)
                    {
                        this.bodyPart.OnNewInjuryEvent -= this.OnNewInjuryEventHandler;
                    }

                    this.bodyPart = value;
                    this.bodyPart.OnNewInjuryEvent += this.OnNewInjuryEventHandler;
                    this.UpdateOnDemand();
                }
            }
        }

        void Start()
        {
            this.DisableOnMissingReference(this.title, nameof(this.title), CompName);
            this.DisableOnMissingReference(this.hitpointsBar, nameof(this.hitpointsBar), CompName);
            this.DisableOnMissingReference(this.injuriesViewManager, nameof(this.injuriesViewManager), CompName);
        }

        void OnDestroy()
        {
            if (this.bodyPart != null)
            {
                this.bodyPart.OnNewInjuryEvent -= this.OnNewInjuryEventHandler;
            }
        }

        private void UpdateOnDemand()
        {
            this.title.text = this.bodyPart.Def.Label;
            this.hitpointsBar.Stat = this.bodyPart.HitPoints;
            this.injuriesViewManager.SetInjuries(bodyPart.Injuries);
        }

        private void OnNewInjuryEventHandler(Injury injury)
        {
            this.injuriesViewManager.AddInjury(injury);
        }
    }
}
