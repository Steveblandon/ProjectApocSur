namespace Projapocsur.Scripts
{
    using Projapocsur.World;
    using UnityEngine;

    public class Damageable : MonoBehaviour, IDamageable
    {
        public Vector3 Position => this.transform.position;

        public string UniqueID { get; set; }

        public string FactionID { get; set; }

        public void TakeDamage(DamageInfo damageInfo)
        {
            Debug.Log($"'{this.name}' took damage");
        }
    }
}
