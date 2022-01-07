namespace Projapocsur.Engine
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.World;
    using UnityEngine;

    public class BodyOutlineManager : MonoBehaviour, IBodyPartsViewManager
    {
        public const string CompName = nameof(BodyOutlineManager);
        public event Action<BodyPart> OnSelectedBodyPartChangeEvent;

        /*
        * temporary ref set up; in the future when there are more than just the 'human' body type, the body def should be updated 
        * to have a prefab ref for this comp to instantiate and cache as needed.
        */
        [SerializeField]
        private GameObject bodyPartsManagerInternalObject;

        private IBodyPartsViewManager bodyPartsViewManagerInternal;

        void Start()
        {
            if (!this.DisableOnMissingReference(this.bodyPartsManagerInternalObject, nameof(this.bodyPartsManagerInternalObject), CompName))
            {
                this.bodyPartsViewManagerInternal = this.bodyPartsManagerInternalObject.GetComponent<IBodyPartsViewManager>();
            }

            this.DisableOnMissingReference(this.bodyPartsViewManagerInternal, nameof(this.bodyPartsViewManagerInternal), CompName);

            if (!this.isActiveAndEnabled)
            {
                return;
            }
            
            this.bodyPartsViewManagerInternal.OnSelectedBodyPartChangeEvent += this.OnSelectedBodyPartChangeEventHandler;
        }

        void OnDestroy()
        {
            this.bodyPartsViewManagerInternal.OnSelectedBodyPartChangeEvent -= this.OnSelectedBodyPartChangeEventHandler;
        }

        public BodyPart CurrentSelected => this.bodyPartsViewManagerInternal?.CurrentSelected;

        public void SetBodyParts(IReadOnlyCollection<BodyPart> bodyParts) => this.bodyPartsViewManagerInternal?.SetBodyParts(bodyParts);

        private void OnSelectedBodyPartChangeEventHandler(BodyPart bodyPart)
        {
            this.OnSelectedBodyPartChangeEvent?.Invoke(bodyPart);
        }
    }
}
