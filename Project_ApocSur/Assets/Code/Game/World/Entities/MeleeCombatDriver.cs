namespace Projapocsur.World
{
    using System;
    using Projapocsur.Engine;

    public class MeleeCombatDriver : IDisposable
    {
        private IMoveable moveable;
        private IMeleeWeapon weapon;
        private IDamageable currentTarget;

        public MeleeCombatDriver(IMoveable moveable)
        {
            this.moveable = moveable;
        }

        /// <summary>
        /// once the inventory system is in place, this class will be able to interact with it to pull up a weapon, but until then it must be assigned.
        /// </summary>
        public IMeleeWeapon Weapon
        {
            get => this.weapon;
            set
            {
                if (this.weapon == value)
                {
                    return;
                }

                if (this.weapon != null)
                {
                    this.weapon.AttackFinishedEvent -= this.OnAttackFinishedEvent;
                }

                this.weapon = value;
                this.weapon.AttackFinishedEvent += this.OnAttackFinishedEvent;
            }
        }

        public IDamageable CurrentTarget
        {
            get => this.currentTarget;
            set
            {
                if (this.currentTarget == value)
                {
                    return;
                }

                this.moveable.OnDestinationReachedEvent -= this.OnTargetWithinRangeEvent;
                this.currentTarget = value;

                if (this.currentTarget != null)
                {
                    this.moveable.OnDestinationReachedEvent += this.OnTargetWithinRangeEvent;
                    this.moveable.FollowTarget(value, 3);
                }
                else
                {
                    this.moveable.CancelCurrentDirective();
                    this.moveable.StopLookingAtTarget();
                }
            }
        }

        public void Dispose()
        {
            if (this.weapon != null)
            {
                this.weapon.AttackFinishedEvent -= this.OnAttackFinishedEvent;
            }

            this.moveable.OnDestinationReachedEvent -= this.OnTargetWithinRangeEvent;
        }

        private void OnTargetWithinRangeEvent()
        {
            if (this.currentTarget != null)
            {
                this.AttackTarget();
            }
        }

        private void OnAttackFinishedEvent()
        {
            if (this.currentTarget != null && this.moveable.IsTargetWithinDistance)
            {
                this.AttackTarget();
            }
        }

        private void AttackTarget()
        {
            this.weapon.Attack(
                this.currentTarget,
                abortCallback: () => this.currentTarget == null || !this.moveable.IsTargetWithinDistance);
        }
    }
}