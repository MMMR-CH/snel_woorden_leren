using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class InspectorButtonAttribute : PropertyAttribute
{
    public string Label { get; }
    public InspectorButtonAttribute(string label = null)
    {
        Label = label;
    }
}
