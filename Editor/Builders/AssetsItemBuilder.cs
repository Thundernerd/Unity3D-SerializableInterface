using System;
using System.Collections.Generic;
using TNRD.Items;
using TNRD.Utilities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace TNRD.Builders
{
    internal sealed class AssetsItemBuilder
    {
        private readonly HashSet<string> addedItems;
        private readonly Dictionary<string, AdvancedDropdownItem> splitToItem;
        private readonly Type interfaceType;

        public AssetsItemBuilder(Type interfaceType)
        {
            Assert.IsNotNull(interfaceType);
            Assert.IsTrue(interfaceType.IsInterface);

            addedItems = new HashSet<string>();
            splitToItem = new Dictionary<string, AdvancedDropdownItem>();
            this.interfaceType = interfaceType;
        }

        public AdvancedDropdownItem Build()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Assets");
            splitToItem.Add("Assets/", root); // Needs the trailing slash to be recognized later on

            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (string assetPath in allAssetPaths)
            {
                Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                if (interfaceType.IsAssignableFrom(assetType))
                {
                    CreateItemForPath(root, assetPath);
                }
                else if (assetType == typeof(GameObject))
                {
                    GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (gameObject.GetComponent(interfaceType) != null)
                        CreateItemForPath(root, assetPath);
                }
            }

            return root;
        }

        private void CreateItemForPath(AdvancedDropdownItem root, string path)
        {
            if (addedItems.Contains(path))
                return;

            AdvancedDropdownItem parent = GetOrCreateParentItem(root, path);
            parent.AddChild(new AssetDropdownItem(path));
            addedItems.Add(path);
        }

        private AdvancedDropdownItem GetOrCreateParentItem(AdvancedDropdownItem root, string path)
        {
            string currentPath = string.Empty;
            string[] splits = path.Split('/');

            AdvancedDropdownItem item = root;

            for (int i = 0; i < splits.Length - 1; i++)
            {
                string split = splits[i];
                currentPath += split + '/';

                if (splitToItem.TryGetValue(currentPath, out AdvancedDropdownItem foundItem))
                {
                    item = foundItem;
                    continue;
                }

                AdvancedDropdownItem advancedDropdownItem = new AdvancedDropdownItem(split)
                {
                    icon = IconUtility.FolderIcon
                };
                item.AddChild(advancedDropdownItem);
                item = advancedDropdownItem;
                splitToItem.Add(currentPath, advancedDropdownItem);
            }

            return item;
        }
    }
}
