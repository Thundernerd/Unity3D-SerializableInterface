using UnityEditor;
using UnityEngine;

namespace TNRD
{
    internal static class IconUtility
    {
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
    }
}