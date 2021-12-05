namespace Projapocsur.World
{
    using System.Collections;
    using Projapocsur;
    using Projapocsur.Scripts;
    using UnityEngine;

    public class CombatProcessor : IEventListener
    {
        #region Unity editor configurable variables
        private float HostileScanRadius => GameMaster.Instance.Config.HostileScanRadius;
        private float HostileScanInterval => GameMaster.Instance.Config.HostileScanInterval;
        #endregion

        private IMoveable moveable;
        private ICoroutineHandler coroutineHandler;
        private ProximityScanner proximityScanner;
        private RelationsTracker relationsTracker;
        private ITargetable currentTarget;
        private Coroutine latestActiveRoutine;

        public CombatProcessor(IMoveable moveable, ICoroutineHandler coroutineHandler, ProximityScanner proximityScanner, RelationsTracker relationsTracker)
        {
            this.moveable = moveable;
            this.coroutineHandler = coroutineHandler;
            this.proximityScanner = proximityScanner;
            this.relationsTracker = relationsTracker;
            this.RangedCombatDriver = new RangedCombatDriver(coroutineHandler, proximityScanner);
            this.RangedCombatDriver.TargetUnreachableEvent += this.OnTargetUnreachableEvent;
        }

        public bool HasTarget => this.currentTarget != null;

        public bool IsManualTargetingOverrideActive { get; private set; }

        public RangedCombatDriver RangedCombatDriver { get; }       // this shouldn't be exposed, but need it for now to assign rangedWeapon... in the future, CombatProcessor should be interfacing with inventory

        public void EngageHostileTargets()
        {
            if (this.latestActiveRoutine == null)
            {
                this.latestActiveRoutine = this.coroutineHandler.StartCoroutine(this.ScanForHostileTargetsRoutine());
            }
        }

        public void Cease()
        {
            if (this.latestActiveRoutine != null)
            {
                this.coroutineHandler.StopCoroutine(this.latestActiveRoutine);
                this.latestActiveRoutine = null;
            }

            this.DisengageTarget();
        }

        public void EngageTarget(IDamageable damageable, CombatEngagementMode engagementMode)
        {
            this.Cease();
            this.IsManualTargetingOverrideActive = true;
            this.EngageTargetInternal(damageable, engagementMode);
        }

        public void DisengageTarget()
        {
            this.IsManualTargetingOverrideActive = false;
            this.currentTarget = null;
            this.RangedCombatDriver.CurrentTarget = null;
            this.moveable.StopLookingAtTarget();
            Debug.Log("Target disengaged");
        }

        public void OnDestroy()
        {
            this.RangedCombatDriver.TargetUnreachableEvent -= this.OnTargetUnreachableEvent;
            this.RangedCombatDriver.OnDestroy();
        }

        private void EngageTargetInternal(IDamageable damageable, CombatEngagementMode engagementMode)
        {
            if (this.currentTarget == damageable)
            {
                return;
            }

            this.currentTarget = damageable;
            this.moveable.LookAt(damageable);

            switch (engagementMode)
            {
                case CombatEngagementMode.Shoot:
                    this.EngageTargetInRangedCombat(damageable);
                    break;
                case CombatEngagementMode.Melee:
                    // TBI
                    break;
            }

            Debug.Log("Target engaged");
        }

        private void EngageTargetInRangedCombat(ITargetable targetable) => this.RangedCombatDriver.CurrentTarget = targetable;  

        private IEnumerator ScanForHostileTargetsRoutine()
        {
            while (!this.IsManualTargetingOverrideActive)
            {
                ITargetable target = proximityScanner.GetNearestTargetByIdWithinRadius(relationsTracker.HostileIndividualsById, HostileScanRadius);

                if (target != null)
                {
                    this.OnHostileTargetAcquired(target as IDamageable);
                }
                
                yield return new WaitForSeconds(HostileScanInterval);
            }
        }

        private void OnHostileTargetAcquired(IDamageable target)
        {
            // this should contain some more complex logic to determine how to engage, but for now just shoot.

            this.EngageTargetInternal(target, CombatEngagementMode.Shoot);
        }

        private void OnTargetUnreachableEvent() => this.DisengageTarget();
    }
}