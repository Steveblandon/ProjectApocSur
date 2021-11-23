namespace Projapocsur.World
{
    using System.Collections;
    using Projapocsur;
    using Projapocsur.Scripts;
    using UnityEngine;

    public class CombatDriver : IEventListener
    {
        private const float SecondsToWaitForTargetToComeWithinFiringRange = 1;
        private const int WaitIntervalsBeforeDisengagingTarget = 10;

        private Moveable moveable;
        private ICoroutineHandler coroutineHandler;
        private IRangedWeapon rangedWeapon;
        private ITargetable currentTarget;

        public CombatDriver(Moveable moveable, ICoroutineHandler coroutineHandler)
        {
            this.moveable = moveable;
            this.coroutineHandler = coroutineHandler;
        }

        /// <summary>
        /// once the inventory system is in place, this class will be able to interact with it to pull up a weapon, but until then it must be assigned.
        /// </summary>
        public IRangedWeapon RangedWeapon 
        {
            get => this.rangedWeapon;
            set
            {
                if (this.rangedWeapon != null)
                {
                    this.rangedWeapon.WeaponFiredEvent -= this.FireAtTargetWhenInRange;
                }

                this.rangedWeapon = value;
            }
        }

        public void EngageTarget(IDamageable damageable, CombatEngagementMode engagementMode)
        {
            this.currentTarget = damageable;
            this.moveable.TrackTarget(damageable);

            switch (engagementMode)
            {
                case CombatEngagementMode.Shoot:
                    this.FireAtTargetWhenInRange();
                    break;
                case CombatEngagementMode.Melee:
                case CombatEngagementMode.TrackAndEliminate:
                    // TBI
                    break;
            }

            Debug.Log("Target engaged");
        }

        public void DisengageTarget()
        {
            this.currentTarget = null;
            this.moveable.DisengageTarget();
            Debug.Log("Target disengaged");
        }

        public void OnDestroy()
        {
            if (this.RangedWeapon != null)
            {
                this.RangedWeapon.WeaponFiredEvent -= this.FireAtTargetWhenInRange;
                this.rangedWeapon.WeaponReloadedEvent -= this.OnWeaponReloadedEvent;
            }
        }

        private void ShootAtCurrentTarget()
        {
            if (this.rangedWeapon.Ammo == 0)
            {
                this.rangedWeapon.WeaponReloadedEvent += this.OnWeaponReloadedEvent;
                this.rangedWeapon.Reload();
            }
            else
            {
                this.rangedWeapon.WeaponFiredEvent += this.FireAtTargetWhenInRange;
                this.rangedWeapon.FireProjectile();
            }
        }

        private void OnWeaponReloadedEvent()
        {
            this.rangedWeapon.WeaponReloadedEvent -= this.OnWeaponReloadedEvent;
            this.FireAtTargetWhenInRange();
        }

        private void FireAtTargetWhenInRange()
        {
            this.rangedWeapon.WeaponFiredEvent -= this.FireAtTargetWhenInRange;

            if (this.IsTargetWithinFiringRange())
            {
                this.ShootAtCurrentTarget();
            }
            else
            {
                Debug.Log("Target not within range. Waiting...");
                this.coroutineHandler.StartCoroutine(this.WaitForTargetToGetWithinRange());
            }
        }

        private IEnumerator WaitForTargetToGetWithinRange()
        {
            ITargetable preWaitCurrentTarget = this.currentTarget;
            int waitIntervals = 0;

            while (this.currentTarget == preWaitCurrentTarget && !this.IsTargetWithinFiringRange() && waitIntervals++ < WaitIntervalsBeforeDisengagingTarget)
            {
                yield return this.coroutineHandler.WaitForSeconds(SecondsToWaitForTargetToComeWithinFiringRange);
            }

            if (waitIntervals >= WaitIntervalsBeforeDisengagingTarget)
            {
                this.DisengageTarget();
            }
            else if (this.currentTarget == preWaitCurrentTarget)
            {
                this.ShootAtCurrentTarget();
            }
        }

        private bool IsTargetWithinFiringRange() => this.currentTarget != null && Vector3.Distance(this.moveable.transform.position, this.currentTarget.Position) < this.rangedWeapon.EffectiveRange;
    }
}