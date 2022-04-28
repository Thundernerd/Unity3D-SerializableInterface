using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace TNRD.Utilities
{
    internal static class PropertyEditorUtility
    {
        internal static void Show(Object obj)
        {
            Type propertyEditor = typeof(EditorWindow).Assembly.GetTypes()
                .FirstOrDefault(x => x.Name == "PropertyEditor");

            if (propertyEditor == null)
                return;

            MethodInfo openPropertyEditorMethod = propertyEditor.GetMethod("OpenPropertyEditor",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new Type[]
                {
                    typeof(UnityEngine.Object),
                    typeof(bool)
                },
                null);

            openPropertyEditorMethod.Invoke(null,
                new object[]
                {
                    obj, true
                });
        }
    }
}
