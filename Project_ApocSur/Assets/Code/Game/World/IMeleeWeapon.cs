namespace Projapocsur.World
{
    using System;

    public interface IMeleeWeapon
    {
        event Action AttackFinishedEvent;

        int EffectiveRange { get; }

        void Attack(IDamageable target, Func<bool> abortCallback = null);
    }
}
