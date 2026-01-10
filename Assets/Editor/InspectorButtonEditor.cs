using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// Make this editor work for BOTH MonoBehaviour and ScriptableObject
[CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
public class InspectorButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Only proceed for custom user components (MonoBehaviour or ScriptableObject)
        if (!(target is MonoBehaviour) && !(target is ScriptableObject))
            return;

        var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            var attrs = (InspectorButtonAttribute[])method.GetCustomAttributes(typeof(InspectorButtonAttribute), true);
            if (attrs.Length > 0)
            {
                var attr = attrs[0];
                string label = attr.Label ?? ObjectNames.NicifyVariableName(method.Name);

                if (GUILayout.Button(label))
                {
                    method.Invoke(target, null);
                }
            }
        }
    }
}
