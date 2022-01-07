namespace Projapocsur.Engine
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
                if (!this.isActiveAndEnabled || this.injury == value)
                {
                    return;
                }

                if (this.injury != null)
                {
                    this.injury.IsHealedEvent -= this.InjuryIsHealedEventHandler;
                    this.Injury.BleedingRate.OnValueAtMinEvent -= this.BleedingStoppedEventHandler;
                }

                this.injury = value;
                this.injury.IsHealedEvent += this.InjuryIsHealedEventHandler;

                if (this.injury.IsBleedingSource)
                {
                    this.bleedingRateIcon.gameObject.SetActive(true);
                    this.Injury.BleedingRate.OnValueAtMinEvent += this.BleedingStoppedEventHandler;
                }
                else
                {
                    this.bleedingRateIcon.gameObject.SetActive(false);
                }

                this.UpdateOnDemand();
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
                this.Injury.BleedingRate.OnValueAtMinEvent -= this.BleedingStoppedEventHandler;
            }
        }

        private void UpdateOnDemand()
        {
            this.title.text = this.injury.Def.Name;
            this.bleedingRateIcon.enabled = this.injury.IsBleeding;
        }

        private void InjuryIsHealedEventHandler()
        {
            this.injury.IsHealedEvent -= this.InjuryIsHealedEventHandler;
            this.InjuryIsHealedEvent?.Invoke(this);
        }

        private void BleedingStoppedEventHandler()
        {
            this.bleedingRateIcon.enabled = false;
            this.Injury.BleedingRate.OnValueAtMinEvent -= this.BleedingStoppedEventHandler;
        }
    }
}
