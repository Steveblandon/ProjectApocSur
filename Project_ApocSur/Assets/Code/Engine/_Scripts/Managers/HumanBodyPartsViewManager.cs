namespace Projapocsur.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Projapocsur.World;
    using UnityEngine;

    public class HumanBodyPartsViewManager : MonoBehaviour, IBodyPartsViewManager
    {
        public const string CompName = nameof(HumanBodyPartsViewManager);
        public event Action<BodyPart> OnSelectedBodyPartChangeEvent;

        [SerializeField]
        private SelectableUI head;

        [SerializeField]
        private SelectableUI torso;

        [SerializeField]
        private SelectableUI[] legs;

        [SerializeField]
        private SelectableUI[] arms;

        private Dictionary<Selectable, BodyPart> selectableToBodyPartMap;
        private Selectable currentSelectable;

        public BodyPart CurrentSelected { get => !this.selectableToBodyPartMap.IsNullOrEmpty() && this.currentSelectable != null ? this.selectableToBodyPartMap[this.currentSelectable] : null; }

        void OnDestroy()
        {
            this.ResetInternal();
        }

        public void SetBodyParts(IReadOnlyCollection<BodyPart> bodyParts)
        {
            this.ResetInternal();

            int legIndex = 0;
            int armIndex = 0;

            foreach (var bodyPart in bodyParts)
            {
                switch (bodyPart.Def.Name)
                {
                    case DefNameOf.BodyPart.Human_Head:
                        this.MapSelectableToBodyPart(head, bodyPart);
                        break;
                    case DefNameOf.BodyPart.Human_Torso:
                        this.MapSelectableToBodyPart(torso, bodyPart);
                        break;
                    case DefNameOf.BodyPart.Human_Arm:
                        this.MapSelectableToBodyPart(arms.ElementAtOrDefault(armIndex++), bodyPart);
                        break;
                    case DefNameOf.BodyPart.Human_Leg:
                        this.MapSelectableToBodyPart(legs.ElementAtOrDefault(legIndex++), bodyPart);
                        break;

                }
            }
        }

        private void ResetInternal()
        {
            this.currentSelectable = null;

            if (this.selectableToBodyPartMap != null)
            {
                foreach (var entry in this.selectableToBodyPartMap)
                {
                    entry.Key.OnSelectStateChangeEvent -= this.OnSelectEventHandler;
                }

                this.selectableToBodyPartMap.Clear();
            }

            this.selectableToBodyPartMap ??= new Dictionary<Selectable, BodyPart>();
        }

        private void MapSelectableToBodyPart(Selectable selectable, BodyPart bodyPart)
        {
            if (selectable == null)
            {
                Debug.LogWarning($"{CompName}: tried to map {nameof(bodyPart)}, but no {nameof(selectable)} was assigned.");
                return;
            }

            if (this.selectableToBodyPartMap.ContainsKey(selectable))
            {
                Debug.LogWarning($"second '{bodyPart.Def.Name}' detected, but only one was expected. skipped.");
            }
            else
            {
                this.selectableToBodyPartMap.Add(selectable, bodyPart);
                selectable.OnDeselect();
                selectable.OnSelectStateChangeEvent += this.OnSelectEventHandler;
            }
        }

        private void OnSelectEventHandler(Selectable selectable)
        {
            if (!selectable.IsSelected)
            {
                return;
            }

            if (this.currentSelectable != selectable)
            {
                this.currentSelectable?.OnDeselect();
                this.currentSelectable = selectable;
                this.OnSelectedBodyPartChangeEvent?.Invoke(this.selectableToBodyPartMap[selectable]);
            }
        }
    }
}
