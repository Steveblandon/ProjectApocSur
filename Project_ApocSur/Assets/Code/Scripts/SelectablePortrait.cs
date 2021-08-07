namespace Projapocsur
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class SelectablePortrait : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        private Image frameOutline;

        public Character Character { get; set; }

        public void OnSelect(BaseEventData eventData)
        {
            Debug.Log($" {this.name} portrait selected");

            if (frameOutline != null)
            {
                frameOutline.color = Color.white;
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Debug.Log($" {this.name} portrait deselected");

            if (frameOutline != null)
            {
                frameOutline.color = Color.black;
            }
        }
    }

}