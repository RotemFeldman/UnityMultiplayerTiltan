using System;
using UnityEngine;
using UnityEditor;

public class Vector3ContextProperties
{
    [InitializeOnLoadMethod]
    static void OnInit()
    {
        EditorApplication.contextualPropertyMenu += OnContextMenu;
    }

    private static void OnContextMenu(GenericMenu menu, SerializedProperty property)
    {
        if (property.propertyType != SerializedPropertyType.Vector3)
            return;
            
        menu.AddItem(new GUIContent("Zero"),false, () =>
        {
            property.vector3Value = Vector3.zero;
            property.serializedObject.ApplyModifiedProperties();
        });
    }
}
