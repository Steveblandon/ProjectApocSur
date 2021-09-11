namespace Projapocsur.ScriptableObjects
{
    using Projapocsur.Common;
    using UnityEngine;

    [CreateAssetMenu]
    public class DefsDatabaseViewer : XMLViewer
    {
        public static readonly new string CompName = nameof(DefsDatabaseViewer);

        [Header("Result")]
        [Space(30)]
        [Tooltip("The deserialized content")]
        [SerializeField]
        private DefinitionFinder.Defs root;

        protected override void Save() => this.SaveData(root);

        protected override void Load() => this.LoadData(out root);
    }
}
