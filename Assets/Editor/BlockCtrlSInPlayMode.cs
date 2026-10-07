using UnityEditor;
using UnityEngine;
using System.Reflection;

[InitializeOnLoad]
public static class BlockCtrlSInPlayMode
{
    static BlockCtrlSInPlayMode()
    {
        FieldInfo field = typeof(EditorApplication).GetField(
            "globalEventHandler",
            BindingFlags.Static | BindingFlags.NonPublic
        );

        if (field == null)
            return;

        EditorApplication.CallbackFunction currentHandler =
            (EditorApplication.CallbackFunction)field.GetValue(null);

        currentHandler += HandleGlobalKey;

        field.SetValue(null, currentHandler);
    }

    private static void HandleGlobalKey()
    {
        if (!EditorApplication.isPlaying)
            return;

        Event currentEvent = Event.current;

        if (currentEvent == null)
            return;

        if (currentEvent.type == EventType.KeyDown &&
            currentEvent.keyCode == KeyCode.S &&
            currentEvent.control)
        {
            currentEvent.Use();
        }
    }
}