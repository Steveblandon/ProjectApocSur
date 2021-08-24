namespace Projapocsur.Behaviors
{
    using System.Collections;
    using System.Collections.Generic;
    using Projapocsur.Common;
    using UnityEngine;

    /// <summary>
    /// Temporary placeholder for initializing connections or triggers for which a robust system isn't in place yet.
    /// </summary>
    public class Temp_Jumpstarter : MonoBehaviour
    {
        public static readonly string className = nameof(Temp_Jumpstarter);

        [Tooltip("will be linked to portrait below, in the future this will be managed by an instance that deals with character creation.")]
        [SerializeField]
        private Character character;

        [Tooltip("will be linked to character above, in the future this will be managed by an instance that deals with character creation.")]
        [SerializeField]
        private CharacterPortrait portrait;

        public void Start()
        {
            if (this.character == null || this.portrait == null)
            {
                Debug.LogError($"{className}: missing character reference(s) in {this.gameObject.name}.");
            }
            else
            {
                CharacterManager.Instance.Add(this.character, this.portrait);
            }
        }
    }
}
