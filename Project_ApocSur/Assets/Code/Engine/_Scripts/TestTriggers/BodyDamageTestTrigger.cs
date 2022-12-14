namespace Projapocsur.Engine
{
    using System.Collections.Generic;
    using Projapocsur.Engine.EditorAttributes;
    using Projapocsur.World;
    using UnityEngine;

    public class BodyDamageTestTrigger : MonoBehaviour
    {
        public const string CompName = nameof(BodyDamageTestTrigger);

        [SerializeField]
        [Button(nameof(ApplyBruiseDamage))]
        private bool punch;

        [SerializeField]
        [Button(nameof(ApplyFractureDamage))]
        private bool breakBones;

        [SerializeField]
        [Button(nameof(ApplyLacerationDamage))]
        private bool cut;

        [SerializeField]
        [Button(nameof(TriggerUpdate))]
        private bool triggerUpdate;

        [SerializeField]
        private float damage;

        [SerializeField]
        private float hitHeight;

        void Start()
        {
            this.enabled = false;
        }
        
        private void TriggerUpdate()
        {
            GameMaster.Instance.PlayerCharacterSelection.Current.Body.OnUpdate();
        }

        private void ApplyBruiseDamage()
        {
            var hitInfo = new DamageInfo(this.hitHeight, new List<string>(1) { DefNameOf.Injury.Bruise }, this.damage);
            this.ApplyDamage(hitInfo);
        }

        private void ApplyFractureDamage()
        {
            var hitInfo = new DamageInfo(this.hitHeight, new List<string>(1) { DefNameOf.Injury.Fracture }, this.damage);
            this.ApplyDamage(hitInfo);
        }

        private void ApplyLacerationDamage()
        {
            var hitInfo = new DamageInfo(this.hitHeight, new List<string>(1) { DefNameOf.Injury.Laceration }, this.damage);
            this.ApplyDamage(hitInfo);
        }

        private void ApplyDamage(DamageInfo hitInfo)
        {
            Character character = GameMaster.Instance.PlayerCharacterSelection.Current;
            
            if (character != null)
            {
                character.Body.TakeDamage(hitInfo);
                character.Body.OnUpdate();
            }
            else
            {
                Debug.LogWarning($"{CompName}: attempted to apply damage, but no single character is currently selected.");
            }
        }
    }
}
