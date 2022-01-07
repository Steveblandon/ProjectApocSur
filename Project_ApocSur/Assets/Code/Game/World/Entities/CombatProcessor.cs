namespace Projapocsur.World
{
    using System;
    using System.Collections;
    using Projapocsur;
    using Projapocsur.Engine;

    public class CombatProcessor : IDisposable
    {
        #region Unity editor configurable variables
        private float HostileScanRadius => GameMaster.Instance.Config.HostileScanRadius;
        private float HostileScanInterval => GameMaster.Instance.Config.HostileScanInterval;
        #endregion

        private ICoroutineHandler coroutineHandler;
        private ProximityScanner proximityScanner;
        private RelationsTracker relationsTracker;
        private ITargetable currentTarget;

        public CombatProcessor(IMoveable moveable, ICoroutineHandler coroutineHandler, ProximityScanner proximityScanner, RelationsTracker relationsTracker)
        {
            this.coroutineHandler = coroutineHandler;
            this.proximityScanner = proximityScanner;
            this.relationsTracker = relationsTracker;
            this.RangedCombatDriver = new RangedCombatDriver(coroutineHandler, proximityScanner, moveable);
            this.RangedCombatDriver.TargetUnreachableEvent += this.OnTargetUnreachableEvent;
            this.MeleeCombatDriver = new MeleeCombatDriver(moveable);
        }

        public bool HasTarget => this.currentTarget != null;

        public bool IsManualTargetingOverrideActive { get; private set; }

        public RangedCombatDriver RangedCombatDriver { get; }       // this shouldn't be exposed, but need it for now to assign rangedWeapon... in the future, CombatProcessor should be interfacing with inventory

        public MeleeCombatDriver MeleeCombatDriver { get; }       // this shouldn't be exposed, but need it for now to assign meleeWeapon... in the future, CombatProcessor should be interfacing with inventory

        public void EngageHostileTargets()
        {
            if (this.coroutineHandler.LatestActiveCoroutine == null)
            {
                this.coroutineHandler.StartCoroutine(this.ScanForHostileTargetsRoutine());
            }
        }

        public void Cease()
        {
            if (this.coroutineHandler.LatestActiveCoroutine != null)
            {
                this.coroutineHandler.StopCoroutine(this.coroutineHandler.LatestActiveCoroutine);
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
            this.MeleeCombatDriver.CurrentTarget = null;
            LogUtility.Log(LogLevel.Info, "Target disengaged");
        }

        public void Dispose()
        {
            this.RangedCombatDriver.TargetUnreachableEvent -= this.OnTargetUnreachableEvent;
            this.RangedCombatDriver.Dispose();
            this.MeleeCombatDriver.Dispose();
        }

        private void EngageTargetInternal(IDamageable damageable, CombatEngagementMode engagementMode)
        {
            if (this.currentTarget == damageable)
            {
                return;
            }

            this.currentTarget = damageable;

            switch (engagementMode)
            {
                case CombatEngagementMode.Shoot:
                    this.RangedCombatDriver.CurrentTarget = damageable;
                    break;
                case CombatEngagementMode.Melee:
                    this.MeleeCombatDriver.CurrentTarget = damageable;
                    break;
                default:
                    LogUtility.Log(LogLevel.Info, $"'{Enum.GetName(typeof(CombatEngagementMode), engagementMode)}' combat processing not implemented yet.");
                    this.Cease();
                    break;
            }

            LogUtility.Log(LogLevel.Info, "Target engaged");
        }

        private IEnumerator ScanForHostileTargetsRoutine()
        {
            while (!this.IsManualTargetingOverrideActive)
            {
                ITargetable target = proximityScanner.GetNearestTargetByIdWithinRadius(relationsTracker.HostileIndividualsById, this.HostileScanRadius);

                if (target != null)
                {
                    this.OnHostileTargetAcquired(target as IDamageable);
                }
                
                yield return coroutineHandler.Wait(this.HostileScanInterval);
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