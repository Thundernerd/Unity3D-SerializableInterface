using UnityEditor;
using UnityEngine;

namespace TNRD.Drawers
{
    public partial class CustomObjectDrawer
    {
        private static class Styles
        {
            private static GUIStyle regularLabelStyle;
            private static GUIStyle selectedLabelStyle;

            public static GUIStyle RegularLabelStyle => regularLabelStyle ??= new GUIStyle(EditorStyles.label);

            public static GUIStyle SelectedLabelStyle
            {
                get
                {
                    if (selectedLabelStyle == null)
                    {
                        selectedLabelStyle = new GUIStyle(EditorStyles.label)
                        {
                            normal =
                            {
                                textColor = EditorGUIUtility.isProSkin
                                    ? new Color32(128, 179, 253, 255)
                                    : new Color32(18, 73, 142, 255)
                            },
                            hover =
                            {
                                textColor = EditorGUIUtility.isProSkin
                                    ? new Color32(128, 179, 253, 255)
                                    : new Color32(18, 73, 142, 255)
                            }
                        };
                    }

                    return selectedLabelStyle;
                }
            }
        }
    }
}
