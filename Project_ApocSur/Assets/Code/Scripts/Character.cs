namespace Projapocsur
{
    using UnityEngine;

    public class Character : MonoBehaviour
    {
        private bool isSelected = false;

        [SerializeField]
        private SelectablePortrait portrait;

        [SerializeField]
        private SpriteRenderer characterOutline;

        public SelectablePortrait Portrait
        {
            get { return this.portrait; }
            private set { this.portrait = value; }
        }

        public void OnEnable()
        {
            if (this.portrait != null)
            {
                this.portrait.Character = this;
            }
            else
            {
                Debug.LogWarning($"Missing portrait assignment for {this.name}:{this.GetInstanceID()}");
            }

            if (this.characterOutline == null)
            {
                Debug.LogWarning($"Missing outline assignment for {this.name}:{this.GetInstanceID()}");
            }
        }

        public void OnSelect()
        {
            if (!this.isSelected)
            {
                Debug.Log($"{this.name} selected.");
                this.isSelected = true;

                if (this.characterOutline != null)
                {
                    this.characterOutline.gameObject.SetActive(true);
                }
            }
        }

        public void OnDeselect()
        {
            if (this.isSelected)
            {
                Debug.Log($"{this.name} de-selected.");
                this.isSelected = false;

                if (this.characterOutline != null)
                {
                    this.characterOutline.gameObject.SetActive(false);
                }
            }
        }
    }

}