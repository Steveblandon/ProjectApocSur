namespace Projapocsur.Engine
{
    using System;
    using System.Collections;
    using Projapocsur.World;
    using UnityEngine;

    public class MeleeWeapon : QueuedRoutineExecutor, IMeleeWeapon
    {
        public event Action AttackFinishedEvent;

        [SerializeField]
        private float attackSpeed;

        [SerializeField]
        private int effectiveRange;

        private DamageInfo damageInfo;
        private IDamageable currentTarget;

        public int EffectiveRange => this.effectiveRange;

        public void Attack(IDamageable target, Func<bool> abortCallback = null)
        {
            this.currentTarget = target;
            this.StartOrQueueRoutine(this.AttackRoutine, abortCallback);
        }

        private IEnumerator AttackRoutine(Func<bool> abortCallback = null)
        {
            Debug.Log($"Initiated attack against target '{this.currentTarget.UniqueID}'");
            IDamageable preWaitTarget = this.currentTarget;
            yield return new WaitForSeconds(this.attackSpeed);
            bool aborted = abortCallback != null ? abortCallback() : false;

            if (!aborted && this.currentTarget == preWaitTarget)
            {
                preWaitTarget.TakeDamage(this.damageInfo);
                Debug.Log($"Completed attack against target '{this.currentTarget.UniqueID}'");
                this.AttackFinishedEvent?.Invoke();
            }
        }
    }
}
