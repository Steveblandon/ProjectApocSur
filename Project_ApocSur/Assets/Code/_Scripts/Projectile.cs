namespace Projapocsur.Scripts
{
    using Projapocsur.World;
    using UnityEngine;

    [RequireComponent(typeof(CapsuleCollider2D))]
    [RequireComponent(typeof(Moveable))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float destinationOffsetMultiplier;

        private Moveable moveable;
        private DamageInfo damageInfo;

        void Start()
        {
            this.moveable = this.GetComponent<Moveable>();
            this.moveable.OnStoppedMovingEvent += this.OnDestinationReachedEventHandler;
        }

        void OnDestroy()
        {
            if (this.moveable != null)
            {
                this.moveable.OnStoppedMovingEvent -= this.OnDestinationReachedEventHandler;
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable))
            {
                Debug.Log($"{this.gameObject.name} hit {collision.gameObject.name}");
                damageable.TakeDamage(this.damageInfo);
                this.gameObject.SetActive(false);
            }
        }

        public Projectile Init(DamageInfo damageInfo, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            this.damageInfo = damageInfo;
            this.transform.parent = parent;
            this.transform.localPosition = position;
            this.transform.localRotation = rotation;
            return this;
        }

        public void PropelTowards(Vector3 direction, float speed)
        {
            if (destinationOffsetMultiplier > 0f)
            {
                direction *= destinationOffsetMultiplier;
            }
            
            this.moveable.MoveInDirection(direction, speed: speed);
        }

        private void OnDestinationReachedEventHandler()
        {
            this.gameObject.SetActive(false);
        }
    }
}
