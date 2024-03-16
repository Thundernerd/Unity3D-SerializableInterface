using System.Reflection;
using UnityEditor;

namespace TNRD.Utilities
{
    public static class ReflectedEditorGUI
    {
        private static readonly PropertyInfo indentLevelProperty =
            typeof(EditorGUI).GetProperty("indent", BindingFlags.Static | BindingFlags.NonPublic);
        
        private static readonly FieldInfo indentWidthField =
            typeof(EditorGUI).GetField("kIndentPerLevel", BindingFlags.Static | BindingFlags.NonPublic);

        static ReflectedEditorGUI()
        {
            indentWidth = (float)indentWidthField.GetValue(null);
        }
        
        public static float indent => (float)indentLevelProperty.GetValue(null);

        public static float indentWidth { get; private set; }
    }
}
