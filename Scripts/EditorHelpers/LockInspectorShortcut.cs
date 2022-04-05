using System;
using System.Reflection;
using UnityEditor;

namespace EditorHelpers
{
    public class LockInspectorShortcut: EditorWindow
    {
            [MenuItem("Tools/Toggle Inspector Lock %l")] // Ctrl + L
            public static void ToggleInspectorLock()
            {
                EditorWindow inspectorToBeLocked = EditorWindow.focusedWindow; // "EditorWindow.focusedWindow" can be used instead
 
                Type projectBrowserType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.ProjectBrowser");
 
                Type inspectorWindowType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
 
                PropertyInfo propertyInfo;
                if (inspectorToBeLocked.GetType() == projectBrowserType)
                {
                    propertyInfo = projectBrowserType.GetProperty("isLocked", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                }
                else if (inspectorToBeLocked.GetType() == inspectorWindowType)
                {
                    propertyInfo = inspectorWindowType.GetProperty("isLocked");
                }
                else
                {
                    return;
                }
 
                bool value = (bool)propertyInfo.GetValue(inspectorToBeLocked, null);
                propertyInfo.SetValue(inspectorToBeLocked, !value, null);
                inspectorToBeLocked.Repaint();
                
            }
    }
}