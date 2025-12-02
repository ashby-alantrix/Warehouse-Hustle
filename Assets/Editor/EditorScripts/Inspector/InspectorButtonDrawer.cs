#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
public class InspectorButtonDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var inspectorButtonAttribute = (InspectorButtonAttribute)attribute;

        if (GUI.Button(position, label.text))
        {
            var target = property.serializedObject.targetObject;
            var method = target.GetType().GetMethod(inspectorButtonAttribute.MethodName);

            if (method != null)
                method.Invoke(target, null);
        }
    }
}
#endif
