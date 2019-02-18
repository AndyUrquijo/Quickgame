using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        var readOnly = attribute as ReadOnlyAttribute;
        if (readOnly.PlayModeOnly && !Application.isPlaying)
            return 0;
        else
            return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        var readOnly = attribute as ReadOnlyAttribute;
        if (readOnly.PlayModeOnly && !Application.isPlaying)
            return;

        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
[CustomPropertyDrawer(typeof(PlayOnlyAttribute))]
public class PlayOnlyDrawer : ReadOnlyDrawer
{ }
#endif


public class ReadOnlyAttribute : PropertyAttribute
{
    public bool PlayModeOnly;

    public ReadOnlyAttribute(bool playModeOnly = false)
    {
        PlayModeOnly = playModeOnly;
    }
}

public class PlayOnlyAttribute : ReadOnlyAttribute
{
    public PlayOnlyAttribute()
    {
        PlayModeOnly = true;
    }
}
