namespace Projapocsur.Scripts
{
    using System;
    using Projapocsur.World;
    using UnityEngine;
    using UnityEngine.UI;

    public class InjuryViewController : MonoBehaviour
    {
        public const string CompName = nameof(InjuryViewController);

        public event Action<InjuryViewController> InjuryIsHealedEvent;

        [SerializeField]
        private Text title;

        [SerializeField]
        private Image bleedingRateIcon;

        private Injury injury;

        public Injury Injury
        {
            get => this.injury;
            set
            {
                if (!this.isActiveAndEnabled)
                {
                    return;
                }

                if (this.injury != null)
                {
                    this.injury.IsHealedEvent -= this.InjuryIsHealedEventHandler;
                }

                this.injury = value;
                this.UpdateOnDemand();
                this.injury.IsHealedEvent += this.InjuryIsHealedEventHandler;
            }
        }

        void OnEnable()
        {
            this.DisableOnMissingReference(title, nameof(title), CompName);
            this.DisableOnMissingReference(bleedingRateIcon, nameof(bleedingRateIcon), CompName);

            if (this.injury != null)
            {
                this.UpdateOnDemand();
            }
        }

        void OnDestroy()
        {
            if (this.injury != null)
            {
                this.injury.IsHealedEvent -= this.InjuryIsHealedEventHandler;
            }
        }

        private void UpdateOnDemand()
        {
            this.title.text = this.injury.Def.Name;
            bleedingRateIcon.enabled = this.injury.IsBleeding;
        }

        private void InjuryIsHealedEventHandler()
        {
            this.InjuryIsHealedEvent?.Invoke(this);
        }
    }
}
