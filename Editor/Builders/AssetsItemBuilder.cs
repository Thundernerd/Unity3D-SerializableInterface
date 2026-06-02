using System;
using System.Collections.Generic;
using System.Linq;
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

            return Collapse(root);
        }

        private AdvancedDropdownItem Collapse(AdvancedDropdownItem root)
        {
            AdvancedDropdownItem[] rootChildren = root.children.ToArray();

            if (root is IDropdownItem)
            {
                return root;
            }

            if (rootChildren.Length == 0)
            {
                return null;
            }
            AdvancedDropdownItem newRoot = new(root.name)
            {
                icon = root.icon,
                enabled = root.enabled,
                id = root.id,
            };

            while (rootChildren.Length == 1 && rootChildren[0] is {} singleChild and not IDropdownItem)
            {
                newRoot = new AdvancedDropdownItem(CollapseName($"{newRoot.name}/{singleChild.name}"))
                {
                    icon = singleChild.icon,
                    id = singleChild.id,
                };
                rootChildren = singleChild.children.ToArray();
            }
            bool addedChildren = false;
            foreach (var child in rootChildren)
            {
                AdvancedDropdownItem newChild = Collapse(child);
                if (newChild != null)
                {
                    newRoot.AddChild(newChild);
                    addedChildren = true;
                }
            }
            return addedChildren ? newRoot : null;
        }

        private const int MAX_NAME_LENGTH = 90;
        private const int HALF_LENGTH = (MAX_NAME_LENGTH - 4) / 2;
        private string CollapseName(string name)
        {
            if (name.Length > MAX_NAME_LENGTH)
            {
                return $"{name[..HALF_LENGTH]}...{name[^HALF_LENGTH..]}";
            }
            return name;
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
