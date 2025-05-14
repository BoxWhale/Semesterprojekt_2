using System;
using UnityEngine;
/// <summary>
/// When applied to a field, this attribute will only allow it to be drawn in the Inspector
/// if the boolean property (given by name) is true, and variable is public.
/// </summary>
public class ShowIfAttribute : PropertyAttribute
{
    public string BoolPropertyName { get; private set; }

    public ShowIfAttribute(string boolPropertyName)
    {
        BoolPropertyName = boolPropertyName;
    }
}