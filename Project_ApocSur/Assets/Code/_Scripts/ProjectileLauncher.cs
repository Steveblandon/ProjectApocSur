namespace Projapocsur.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Projapocsur.EditorAttributes;
    using Projapocsur.World;
    using UnityEngine;

    public class ProjectileLauncher : MonoBehaviour
    {
        [SerializeField]
        private int ammoCapacity;

        [SerializeField]
        private float reloadTime;

        [SerializeField]
        private float aimTime;

        [SerializeField]
        private float projectileSpeed;

        [SerializeField]
        [ReadOnly]
        private int ammo;

        private DamageInfo damageInfo;
        private Vector3 target;
        private bool isBusy;
        private Queue<Func<IEnumerator>> queuedRoutine = new Queue<Func<IEnumerator>>();

        void Start()
        {
            GameMaster.Instance.ObjectPool.AssureMinimumPoolSizeForPath(ResourcePathOf.Bullet, ammoCapacity);
        }

        public void FireProjectile(int distance)
        {
            if (ammo > 0)
            {
                this.target = this.transform.up * distance;
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
            if (this.ammo > 1)
            {
                Projectile projectile = GameMaster
                    .Instance
                    .ObjectPool
                    .GetOrInstantiate(ResourcePathOf.Bullet)
                    .GetComponent<Projectile>()
                    .Init(this.damageInfo, this.transform.position, this.transform.rotation);

                yield return new WaitUntil(() => projectile.enabled);

                projectile.PropelTowards(this.transform.parent.transform.up, projectileSpeed);

                this.ammo--;
            }
        }

        private IEnumerator ReloadRoutine()
        {
            Debug.Log($"{this.name} reloading...");
            yield return new WaitForSeconds(reloadTime);
            ammo = ammoCapacity;
            Debug.Log($"{this.name} reloaded ({this.ammo}/{this.ammoCapacity})");
        }
    }
}
