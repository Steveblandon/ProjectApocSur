namespace Projapocsur.World
{
    public interface IDamageable : ITargetable
    {
        void TakeDamage(DamageInfo damageInfo);
    }
}