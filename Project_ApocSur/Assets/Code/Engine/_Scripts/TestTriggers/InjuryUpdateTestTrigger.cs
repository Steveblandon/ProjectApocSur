namespace Projapocsur.Engine
{
    using System.Collections.Generic;
    using Projapocsur.Engine.EditorAttributes;
    using Projapocsur.World;
    using UnityEngine;

    [RequireComponent(typeof(InjuriesViewManager))]
    public class InjuryUpdateTestTrigger : MonoBehaviour
    {
        [SerializeField]
        [Button(nameof(InsertMultipleInjuries), ButtonWidth = 200)]
        private bool SetInjuries;

        [SerializeField]
        [Button(nameof(InjuryUpdate), ButtonWidth = 200)]
        private bool UpdateInjuries;

        [SerializeField]
        [Button(nameof(InsertOneInjury), ButtonWidth = 200)]
        private bool AddInjury;

        [SerializeField]
        private float healingRateValue = 5f;

        [SerializeField]
        [ReadOnly]
        private float painValue;

        [SerializeField]
        [ReadOnly]
        private float bloodLossValue;

        private InjuriesViewManager injuryManager;
        private List<Injury> currentInjuries;
        private InjuryProcessingContext currentContext;
        private Stat pain, bloodLoss, healingRate;
        

        // Start is called before the first frame update
        void Start()
        {
            this.injuryManager = this.GetComponent<InjuriesViewManager>();
            this.pain = new Stat(DefNameOf.Stat.Pain, 0, 100);
            this.bloodLoss = new Stat(DefNameOf.Stat.BloodLoss, 0, 100);
            this.healingRate = new Stat(DefNameOf.Stat.HealingRate, this.healingRateValue, 100);

            this.currentContext = new InjuryProcessingContext(pain, bloodLoss, healingRate);
            this.enabled = false;
        }

        void Update()
        {
            this.painValue = pain?.Value ?? 0;
            this.bloodLossValue = bloodLoss?.Value ?? 0;

            if (this.currentContext != null && this.healingRate?.Value != healingRateValue)
            {
                this.currentContext.HealingRate = new Stat(DefNameOf.Stat.HealingRate, this.healingRateValue, 100);
            }
        }

        private void InsertMultipleInjuries()
        {
            this.currentInjuries = new List<Injury>();
            this.currentInjuries.Add(new Injury(DefNameOf.Injury.Bruise, SeverityLevel.Minor));
            this.currentInjuries.Add(new Injury(DefNameOf.Injury.Bruise, SeverityLevel.Minor));
            this.currentInjuries.Add(new Injury(DefNameOf.Injury.Bruise, SeverityLevel.Minor));
            this.currentInjuries.Add(new Injury(DefNameOf.Injury.Fracture, SeverityLevel.Major));
            this.currentInjuries.Add(new Injury(DefNameOf.Injury.Laceration, SeverityLevel.Trivial));
            this.currentInjuries.Add(new Injury(DefNameOf.Injury.Laceration, SeverityLevel.Trivial));
            this.currentInjuries.Add(new Injury(DefNameOf.Injury.Bruise, SeverityLevel.Minor));
            this.currentInjuries.Add(new Injury(DefNameOf.Injury.Bruise, SeverityLevel.Minor));
            this.currentInjuries.Add(new Injury(DefNameOf.Injury.Laceration, SeverityLevel.Trivial));

            this.currentInjuries.ForEach(injury => injury.OnStart(this.currentContext));

            this.injuryManager.SetInjuries(this.currentInjuries);
        }

        private void InsertOneInjury()
        {
            var injury = new Injury(DefNameOf.Injury.Fracture, SeverityLevel.Minor);
            this.currentInjuries ??= new List<Injury>();
            this.currentInjuries.Add(injury);
            this.injuryManager.AddInjury(injury);
        }

        private void InjuryUpdate()
        {
            if (this.currentInjuries.IsNullOrEmpty())
            {
                Debug.Log("no injuries to update.");
            }
            else
            {
                this.currentInjuries.ForEach(injury => injury.OnUpdate(this.currentContext));
            }            
        }
    }
}
