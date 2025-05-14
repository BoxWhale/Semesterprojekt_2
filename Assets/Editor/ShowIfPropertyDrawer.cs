
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;
            SerializedProperty conditionProp = property.serializedObject.FindProperty(showIf.BoolPropertyName);

            // Check if the condition property exists and is a boolean
            if (conditionProp == null || conditionProp.propertyType != SerializedPropertyType.Boolean)
            {
                Debug.LogWarning($"ShowIfAttribute: Could not find a boolean property named '{showIf.BoolPropertyName}' in the same class.");
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            // Hide the field if the condition is false
            return conditionProp.boolValue ? EditorGUI.GetPropertyHeight(property, label, true) : 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;
            SerializedProperty conditionProp = property.serializedObject.FindProperty(showIf.BoolPropertyName);

            // Check if the condition property exists and is a boolean
            if (conditionProp == null || conditionProp.propertyType != SerializedPropertyType.Boolean)
            {
                Debug.LogWarning($"ShowIfAttribute: Could not find a boolean property named '{showIf.BoolPropertyName}' in the same class.");
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            // Only draw the field if the condition is true
            if (conditionProp.boolValue)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
#endif
