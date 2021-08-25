namespace Projapocsur.Behaviors
{
    using Projapocsur.Common;
    using UnityEngine;
    using EventType = Common.EventType;

    public class CharacterMovement : MonoBehaviour
    {
        public static readonly string compName = nameof(CharacterMovement);

        [SerializeField]
        private float speed = 1;
        private Vector3 target;
        private Character character;

        void Start()
        {
            target = this.transform.position;

            if (!this.TryGetComponent(out character))
            {
                Debug.LogWarning($"{compName}: no {nameof(Character)} detected on {this.name}.");
            }

            EventManager.Instance.RegisterListener(EventType.WO_NothingClicked_Right, this.UpdateTargetToMousePosition);
        }

        public void Update()
        {
            if (character != null
                && character.IsDrafted
                && target != default(Vector3) 
                && this.transform.position != target)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.fixedDeltaTime);
                this.transform.rotation = Quaternion.LookRotation(Vector3.forward, target - this.transform.position);
            }
            else
            {
                target = default(Vector3);
                this.enabled = false;
            }
        }

        private void UpdateTargetToMousePosition()
        {
            if (character != null && character.IsSelected && character.IsDrafted)
            {
                this.target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                this.target.z = this.transform.position.z;
                this.enabled = true;
                Debug.Log($"move click detected at {Input.mousePosition}, transformed to world space: {this.target}");
            }
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnregisterListener(EventType.WO_NothingClicked_Right, this.UpdateTargetToMousePosition);
        }
    }

}