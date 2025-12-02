using System;
using UnityEngine;

public class InspectorButtonAttribute : PropertyAttribute
{
    public string MethodName;

    public InspectorButtonAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
