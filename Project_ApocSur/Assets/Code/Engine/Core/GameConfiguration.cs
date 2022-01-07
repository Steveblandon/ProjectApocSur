namespace Projapocsur.Engine
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameConfiguration", order = 1)]
    public class GameConfiguration : ScriptableObject
    {
        #region Private Fields
        #region Combat AI
        [Header("Combat AI")]
        [SerializeField]
        [Tooltip("auto-targeting range in Unity measurements (meters).")]
        private float hostileScanRadius;

        [SerializeField]
        [Tooltip("time in seconds to wait before a new scan.")]
        private float hostileScanInterval;

        [SerializeField]
        [Tooltip("time in seconds to wait before a new scan.")]
        private float targetInRangeScanInterval;

        [SerializeField]
        [Tooltip("number of times to scan for target within the held weapon's effective range. Once the max is reached, target is disengaged.")]
        private int maxTargetInRangeScans;

        [SerializeField]
        [Tooltip("Distance to be considered within touching distance between two objects. At zero, the objects touch, but don't clip onto each other.")]
        private float touchingDistance;
        #endregion
        #endregion

        #region Public Accessors
        #region Combat AI
        public float HostileScanRadius => this.hostileScanRadius;
        public float HostileScanInterval => this.hostileScanInterval;
        public float TargetInRangeScanInterval => this.targetInRangeScanInterval;
        public int MaxTargetInRangeScans => this.maxTargetInRangeScans;
        public float TouchingDistance => this.touchingDistance;
        #endregion
        #endregion
    }
}
