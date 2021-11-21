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

        private ProjectileLauncher launcher;

        void Start()
        {
            this.launcher = this.GetComponent<ProjectileLauncher>();
        }

        private void Fire()
        {
            this.launcher.FireProjectile();
        }

        private void Reload()
        {
            this.launcher.Reload();
        }
    }
}
