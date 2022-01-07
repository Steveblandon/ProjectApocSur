namespace Projapocsur.Engine
{
    using Projapocsur.World;
    using UnityEngine;

    public class EngageTargetButtonController : TargetSelectButtonControllerBase
    {

        [SerializeField]
        private CombatEngagementMode combatEngagementMode;

        protected override void OnTargetedClickEvent(ITargetable target)
        {
            bool selecteesAreDrafted = GameMaster.Instance.PlayerCharacterSelection.IsDrafted.Value;
            IDamageable damageable = target as IDamageable;

            if (damageable != null && selecteesAreDrafted)
            {
                foreach (var selectee in GameMaster.Instance.PlayerCharacterSelection.All)
                {
                    selectee.EngageTarget(damageable, this.combatEngagementMode);
                }
            }

            this.CancelTargetingAndReset();
        }
    }
}