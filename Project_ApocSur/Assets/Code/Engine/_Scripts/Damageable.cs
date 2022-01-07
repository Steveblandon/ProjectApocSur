namespace Projapocsur.Engine
{
    using Projapocsur.World;
    using UnityEngine;

    [RequireComponent(typeof(SpriteRenderer))]
    public class Damageable : MonoBehaviour, IDamageable
    {
        private SpriteRenderer spriteRenderer;

        public Vector3 Position => this.transform.position;

        public Vector3 Size => this.spriteRenderer.bounds.size;

        public string UniqueID { get; set; }

        public string FactionID { get; set; }

        void Start()
        {
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            Debug.Log($"'{this.name}' took damage");
        }
    }
}
