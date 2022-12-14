namespace Projapocsur.Engine.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Reflection;
    using Projapocsur.Engine.EditorAttributes;

    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonPropertyDrawer : PropertyDrawer
    {
        private MethodInfo _eventMethodInfo = null;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            ButtonAttribute inspectorButtonAttribute = (ButtonAttribute)this.attribute;
            Rect buttonRect = new Rect(position.x + (position.width - inspectorButtonAttribute.ButtonWidth) * 0.5f, position.y, inspectorButtonAttribute.ButtonWidth, position.height);
            if (GUI.Button(buttonRect, label.text))
            {
                System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
                string eventName = inspectorButtonAttribute.MethodName;

                if (this._eventMethodInfo == null)
                    this._eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (this._eventMethodInfo != null)
                    this._eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
                else
                    Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
            }
        }
    }
}
