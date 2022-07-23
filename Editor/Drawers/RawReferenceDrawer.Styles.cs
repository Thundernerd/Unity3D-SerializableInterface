using UnityEditor;
using UnityEngine;

namespace TNRD.Drawers
{
    internal partial class RawReferenceDrawer
    {
        private static class Styles
        {
            public static Color LineColor
            {
                get
                {
                    return EditorGUIUtility.isProSkin
                        ? new Color(0.14f, 0.14f, 0.14f, 0.5f)
                        : new Color(0.67f, 0.67f, 0.67f);
                }
            }
        }
    }
}
