namespace Projapocsur.Engine.EditorAttributes
{
    using UnityEngine;

    /// <summary>
    /// This attribute can only be applied to fields because its
    /// associated PropertyDrawer only operates on fields (either
    /// public or tagged with the [SerializeField] attribute) in
    /// the target MonoBehaviour.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ButtonAttribute : PropertyAttribute
    {
        public static float kDefaultButtonWidth = 300;

        public readonly string MethodName;

        private float _buttonWidth = kDefaultButtonWidth;
        public float ButtonWidth
        {
            get { return this._buttonWidth; }
            set { this._buttonWidth = value; }
        }

        public ButtonAttribute(string MethodName)
        {
            this.MethodName = MethodName;
        }
    }
}
