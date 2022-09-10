using UnityEditor;
using UnityEngine;

namespace TNRD.Utilities
{
    internal static class IconUtility
    {
#if UNITY_2021_2_OR_NEWER
        private static Texture2D folderIcon;

        public static Texture2D FolderIcon
        {
            get
            {
                if (folderIcon == null)
                    folderIcon = (Texture2D)EditorGUIUtility.IconContent("Folder Icon").image;

                return folderIcon;
            }
        }

        private static Texture2D scriptIcon;

        public static Texture2D ScriptIcon
        {
            get
            {
                if (scriptIcon == null)
                    scriptIcon = (Texture2D)EditorGUIUtility.IconContent("cs Script Icon").image;

                return scriptIcon;
            }
        }

        private static Texture2D gameObjectIcon;

        public static Texture2D GameObjectIcon
        {
            get
            {
                if (gameObjectIcon == null)
                    gameObjectIcon = (Texture2D)EditorGUIUtility.IconContent("GameObject Icon").image;

                return gameObjectIcon;
            }
        }


        public static Texture2D GetIconForObject(string path)
        {
            return (Texture2D)AssetDatabase.GetCachedIcon(path);
        }

        public static Texture2D GetIconForObject(Object obj)
        {
            return EditorGUIUtility.GetIconForObject(obj);
        }
#else
        // "Fix" for 2020 as the icons are 256x256 in size and the AdvancedDropDown fills the space as much as it can
        public static Texture2D FolderIcon => null;

        private static Texture2D scriptIcon;

        public static Texture2D ScriptIcon
        {
            get
            {
                if (scriptIcon == null)
                    scriptIcon = (Texture2D)EditorGUIUtility.IconContent("cs Script Icon").image;

                return scriptIcon;
            }
        }

        public static Texture2D GameObjectIcon => null;

        public static Texture2D GetIconForObject(string path)
        {
            return null;
        }

        public static Texture2D GetIconForObject(Object obj)
        {
            return null;
        }
#endif
    }
}
