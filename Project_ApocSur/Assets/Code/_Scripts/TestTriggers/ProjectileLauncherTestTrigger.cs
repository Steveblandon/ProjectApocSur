namespace Projapocsur.Scripts
{
    using Projapocsur.EditorAttributes;
    using UnityEngine;

    [RequireComponent(typeof(ProjectileLauncher))]
    public class ProjectileLauncherTestTrigger : MonoBehaviour
    {
        [SerializeField]
        [Button(nameof(Fire))]
        private bool fire;

        [SerializeField]
        [Button(nameof(Reload))]
        private bool reload;

        [SerializeField]
        private int shootingDistance;

        private ProjectileLauncher launcher;

        void Start()
        {
            this.launcher = this.GetComponent<ProjectileLauncher>();
        }

        private void Fire()
        {
            this.launcher.FireProjectile(shootingDistance);
        }

        private void Reload()
        {
            this.launcher.Reload();
        }
    }
}
