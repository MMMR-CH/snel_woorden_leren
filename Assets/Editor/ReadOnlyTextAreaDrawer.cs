using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyTextAreaAttribute))]
public class ReadOnlyTextAreaDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ReadOnlyTextAreaAttribute attr = (ReadOnlyTextAreaAttribute)attribute;

        // Estimate line count from number of lines in string, clamped to min/max lines
        int lineCount = Mathf.Clamp(property.stringValue.Split('\n').Length, attr.minLines, attr.maxLines);

        // Approximate height: line height * lines + padding for label
        return EditorGUIUtility.singleLineHeight * (lineCount + 1) + 6;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ReadOnlyTextAreaAttribute attr = (ReadOnlyTextAreaAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);

        // Label at the top line
        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label);

        // Text rect below label
        Rect textRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, position.height - EditorGUIUtility.singleLineHeight - 4);

        GUIStyle style = new GUIStyle(EditorStyles.textArea)
        {
            wordWrap = true,
            richText = true,
            clipping = TextClipping.Clip,
            fontSize = EditorStyles.label.fontSize
        };

        GUI.enabled = false;  // Make it read-only (greyed out)
        EditorGUI.SelectableLabel(textRect, property.stringValue, style);
        GUI.enabled = true;

        EditorGUI.EndProperty();
    }
}
