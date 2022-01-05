namespace Projapocsur.Scripts
{
    using System;
    using System.Collections;
    using Projapocsur.EditorAttributes;
    using Projapocsur.World;
    using UnityEngine;

    public class ProjectileLauncher : QueuedRoutineExecutor, IRangedWeapon
    {
        public event Action WeaponFiredEvent;

        public event Action WeaponReloadedEvent;

        [SerializeField]
        private float accuracy;

        [SerializeField]
        private float spread;

        [SerializeField]
        private int effectiveRange;

        [SerializeField]
        private float reloadTime;

        [SerializeField]
        private float aimTime;

        [SerializeField]
        private float projectileSpeed;

        [SerializeField]
        private int ammoCapacity;

        [SerializeField]
        [ReadOnly]
        private int ammo;

        private DamageInfo damageInfo;

        public int Ammo => this.ammo;

        public int EffectiveRange => this.effectiveRange;

        void Start()
        {
            GameMaster.Instance.ObjectPool.AssureMinimumPoolSizeForPath(ResourcePathOf.Bullet, ammoCapacity);
        }

        public void FireProjectile(Func<bool> abortCallback = null)
        {
            if (ammo > 0)
            {
                this.StartOrQueueRoutine(this.ShootRoutine, abortCallback);
            }
            else
            {
                Debug.Log($"{this.name} tried to shoot, but there is no ammo left.");
            }
        }

        public void Reload(Func<bool> abortCallback = null)
        {
            this.StartOrQueueRoutine(this.ReloadRoutine, abortCallback);
        }

        private IEnumerator ShootRoutine(Func<bool> abortCallback)
        {
            yield return new WaitForSeconds(aimTime);
            bool aborted = abortCallback != null ? abortCallback() : false;

            if (this.ammo > 0 && !aborted)
            {
                Projectile projectile = GameMaster
                    .Instance
                    .ObjectPool
                    .GetOrInstantiate(ResourcePathOf.Bullet)
                    .GetComponent<Projectile>()
                    .Init(this.damageInfo, this.transform.position, this.transform.rotation);

                float rotation = 0;
                if (RandomNumberGenerator.Roll() > accuracy)
                {
                    float spreadHalf = spread / 2;
                    rotation = RandomNumberGenerator.Roll(-spreadHalf, spreadHalf);
                }

                yield return new WaitUntil(() => projectile.enabled);
                projectile.transform.Rotate(0, 0, rotation);
                projectile.PropelForward(distance: effectiveRange, speed: projectileSpeed);
                this.ammo--;
                this.WeaponFiredEvent?.Invoke();
            }
        }

        private IEnumerator ReloadRoutine(Func<bool> abortCallback)
        {
            Debug.Log($"{this.name} reloading...");
            yield return new WaitForSeconds(reloadTime);
            bool aborted = abortCallback != null ? abortCallback() : false;

            if (!aborted)
            {
                ammo = ammoCapacity;
                Debug.Log($"{this.name} reloaded ({this.ammo}/{this.ammoCapacity})");
                this.WeaponReloadedEvent?.Invoke();
            }
        }
    }
}
