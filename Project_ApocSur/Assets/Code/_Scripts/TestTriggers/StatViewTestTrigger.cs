namespace Projapocsur.Scripts
{
    using Projapocsur.EditorAttributes;
    using Projapocsur.World;
    using UnityEngine;

    [RequireComponent(typeof(StatViewController))]
    public class StatViewTestTrigger : MonoBehaviour
    {
        private const string ClassName = nameof(StatViewTestTrigger);
        private const string attached = nameof(attached);
        private const string deattached = nameof(deattached);

        [SerializeField]
        private float statMinValue, statMaxValue, statValue;
        
        [SerializeField]
        [Button(nameof(AttachToStat), ButtonWidth = 200)]
        private bool attachToggle;

        [SerializeField]
        [ReadOnly]
        private string attachState;

        private Stat currentStat;

        private StatViewController statViewController;

        private bool isAttached;

        void Start()
        {
            this.statViewController = this.GetComponent<StatViewController>();
            this.attachState = deattached;
            this.enabled = false;
        }

        void Update()
        {
            if (this.currentStat != null)
            {
                this.currentStat.MaxValue = statMaxValue;
                this.currentStat.MinValue = statMinValue;
                this.currentStat.ApplyQuantity(this.statValue - this.currentStat.Value);
            }
        }

        private void AttachToStat()
        {
            if (!this.isAttached)
            {
                this.currentStat = new Stat(DefNameOf.Stat.HitPoints, statValue, statMaxValue, statMinValue);
                this.statViewController.Stat = this.currentStat;
                this.isAttached = true;
                this.attachState = attached;
            }
            else
            {
                this.currentStat = null;
                this.isAttached = false;
                this.attachState = deattached;
            }

            this.enabled = this.isAttached;
        }
    }
}
