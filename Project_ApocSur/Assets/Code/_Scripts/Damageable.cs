namespace Projapocsur.Scripts
{
    using Projapocsur.World;
    using UnityEngine;

    public class Damageable : MonoBehaviour, IDamageable, ITargetable
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            Debug.Log($"'{this.name}' took damage");
        }
    }
}
