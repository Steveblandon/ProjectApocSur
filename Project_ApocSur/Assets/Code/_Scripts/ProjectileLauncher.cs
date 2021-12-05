namespace Projapocsur.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Projapocsur.EditorAttributes;
    using Projapocsur.World;
    using UnityEngine;

    public class ProjectileLauncher : MonoBehaviour, IRangedWeapon
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
        private bool isBusy;
        private Queue<Func<IEnumerator>> queuedRoutine = new Queue<Func<IEnumerator>>();
        private Func<bool> FireProjectileCommandAborted;

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
                this.FireProjectileCommandAborted = abortCallback;
                this.StartOrQueueRoutine(this.ShootRoutine);
            }
            else
            {
                Debug.Log($"{this.name} tried to shoot, but there is no ammo left.");
            }
        }

        public void Reload()
        {
            this.StartOrQueueRoutine(this.ReloadRoutine);
        }

        private void StartOrQueueRoutine(Func<IEnumerator> coroutine)
        {
            this.queuedRoutine.Enqueue(coroutine);

            if (!this.isBusy)
            {
                this.StartCoroutine(this.RunQueuedRoutines());
            }
        }

        private IEnumerator RunQueuedRoutines()
        {
            this.isBusy = true;

            while (this.queuedRoutine.Count > 0)
            {
                yield return this.StartCoroutine(this.queuedRoutine.Dequeue()());
            }

            this.isBusy = false;
        }

        private IEnumerator ShootRoutine()
        {
            yield return new WaitForSeconds(aimTime);
            bool aborted = this.FireProjectileCommandAborted != null ? this.FireProjectileCommandAborted() : false;

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

        private IEnumerator ReloadRoutine()
        {
            Debug.Log($"{this.name} reloading...");
            yield return new WaitForSeconds(reloadTime);
            ammo = ammoCapacity;
            Debug.Log($"{this.name} reloaded ({this.ammo}/{this.ammoCapacity})");
            this.WeaponReloadedEvent?.Invoke();
        }
    }
}
