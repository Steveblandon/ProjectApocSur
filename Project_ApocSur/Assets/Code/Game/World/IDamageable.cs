namespace Projapocsur.World
{
    using Projapocsur.Engine;

    public interface IDamageable : ITargetable
    {
        void TakeDamage(DamageInfo damageInfo);
    }
}