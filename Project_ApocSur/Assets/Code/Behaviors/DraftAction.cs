namespace Projapocsur.Behaviors
{
    using Projapocsur.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class DraftAction : MonoBehaviour, IPointerClickHandler
    {
        public static readonly string compName = nameof(DraftAction);

        public static DraftAction Instance { get; private set; }

        public bool IsOn { get; private set; }

        private ImageColorSwitch imageColorSwitch;

        public void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                this.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError($"{compName}: multiple instances detected, there should only be one component enabled. current: {Instance.name}, extra: {this.name}");
            }

            this.imageColorSwitch = Utils.GetSingleComponentInChildrenAndLogOnFailure<ImageColorSwitch>(this, compName);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (this.IsOn)
                {
                    this.TurnOff();
                    CharacterManager.Instance.UndraftSelectees();
                }
                else
                {
                    this.TurnOn();
                    CharacterManager.Instance.DraftSelectees();
                }
            }
        }

        public void TurnOn()
        {
            if (!this.IsOn)
            {
                if (imageColorSwitch != null)
                {
                    imageColorSwitch.TurnOn();
                }

                this.IsOn = true;
            }
        }

        public void TurnOff()
        {
            if (this.IsOn)
            {
                if (imageColorSwitch != null)
                {
                    imageColorSwitch.TurnOff();
                }

                this.IsOn = false;
            }
        }
    }
}