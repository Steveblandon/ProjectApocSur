namespace Projapocsur.World
{
    using System;
    using System.Collections;
    using Projapocsur.Engine;

    public class RangedCombatDriver : IDisposable
    {
        #region Unity editor configurable variables
        private float TargetInRangeScanInterval => GameMaster.Instance.Config.TargetInRangeScanInterval;
        private int MaxTargetInRangeScans => GameMaster.Instance.Config.MaxTargetInRangeScans;
        #endregion

        private IMoveable moveable;
        private ICoroutineHandler coroutineHandler;
        private IRangedWeapon rangedWeapon;
        private ITargetable currentTarget;
        private ProximityScanner proximityScanner;

        public RangedCombatDriver(ICoroutineHandler coroutineHandler, ProximityScanner proximityScanner, IMoveable moveable)
        {
            this.coroutineHandler = coroutineHandler;
            this.proximityScanner = proximityScanner;
            this.moveable = moveable;
        }

        public event Action TargetUnreachableEvent;

        /// <summary>
        /// once the inventory system is in place, this class will be able to interact with it to pull up a weapon, but until then it must be assigned.
        /// </summary>
        public IRangedWeapon Weapon 
        {
            get => this.rangedWeapon;
            set
            {
                if (this.rangedWeapon != null)
                {
                    this.rangedWeapon.WeaponFiredEvent -= this.OnWeaponFiredEvent;
                    this.rangedWeapon.WeaponReloadedEvent -= this.OnWeaponReloadedEvent;
                }

                this.rangedWeapon = value;
                this.rangedWeapon.WeaponFiredEvent += this.OnWeaponFiredEvent;
                this.rangedWeapon.WeaponReloadedEvent += this.OnWeaponReloadedEvent;
            }
        }

        public ITargetable CurrentTarget
        {
            get => this.currentTarget;
            set
            {
                if (this.currentTarget == value)
                {
                    return;
                }

                this.currentTarget = value;

                if (this.currentTarget != null)
                {
                    this.moveable.LookAt(value);
                    this.FireAtTargetWhenInRange();
                }
                else
                {
                    this.moveable.StopLookingAtTarget();
                }
            }
        }

        public void Dispose()
        {
            if (this.rangedWeapon != null)
            {
                this.rangedWeapon.WeaponFiredEvent -= this.OnWeaponFiredEvent;
                this.rangedWeapon.WeaponReloadedEvent -= this.OnWeaponReloadedEvent;
            }
        }

        private void OnWeaponReloadedEvent() => this.FireAtTargetWhenInRange();

        private void OnWeaponFiredEvent() => this.FireAtTargetWhenInRange();

        private void FireAtTargetWhenInRange()
        {
            if (this.proximityScanner.IsTargetWithinDistance(this.CurrentTarget, this.rangedWeapon.EffectiveRange))
            {
                this.ShootAtCurrentTarget();
            }
            else
            {
                LogUtility.Log(LogLevel.Info, "Target not within range. Waiting...");
                this.coroutineHandler.StartCoroutine(this.WaitForTargetToGetWithinRangeRoutine());
            }
        }

        private void ShootAtCurrentTarget()
        {
            if (this.rangedWeapon.Ammo == 0)
            {
                this.rangedWeapon.Reload();
            }
            else if (this.currentTarget != null)
            {
                this.rangedWeapon.FireProjectile(() => this.currentTarget == null);
            }
        }

        private IEnumerator WaitForTargetToGetWithinRangeRoutine()
        {
            ITargetable preWaitCurrentTarget = this.CurrentTarget;
            int waitIntervals = 0;

            while (this.CurrentTarget == preWaitCurrentTarget 
                && !this.proximityScanner.IsTargetWithinDistance(this.CurrentTarget, this.rangedWeapon.EffectiveRange)
                && waitIntervals++ < this.MaxTargetInRangeScans)
            {
                yield return this.coroutineHandler.Wait(this.TargetInRangeScanInterval);
            }

            if (waitIntervals >= this.MaxTargetInRangeScans)
            {
                this.TargetUnreachableEvent?.Invoke();
            }
            else if (this.CurrentTarget == preWaitCurrentTarget)
            {
                this.ShootAtCurrentTarget();
            }
        }
    }
}