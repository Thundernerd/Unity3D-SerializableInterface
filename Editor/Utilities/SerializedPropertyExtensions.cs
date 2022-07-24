using UnityEditor;

namespace TNRD.Utilities
{
    internal static class SerializedPropertyExtensions
    {
        public static SerializedProperty ReferenceModeProperty(this SerializedProperty property)
        {
            return property.FindPropertyRelative("mode");
        }

        public static SerializedProperty RawReferenceProperty(this SerializedProperty property)
        {
            return property.FindPropertyRelative("rawReference");
        }

        public static SerializedProperty UnityReferenceProperty(this SerializedProperty property)
        {
            return property.FindPropertyRelative("unityReference");
        }
    }
}
