using System;
using System.Collections.Generic;
using TNRD.Items;
using TNRD.Utilities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Assertions;

namespace TNRD.Builders
{
    internal sealed class ClassesItemBuilder
    {
        private readonly Dictionary<string, AdvancedDropdownItem> splitToItem;
        private readonly Type interfaceType;

        public ClassesItemBuilder(Type interfaceType)
        {
            Assert.IsNotNull(interfaceType);
            Assert.IsTrue(interfaceType.IsInterface);

            splitToItem = new Dictionary<string, AdvancedDropdownItem>();
            this.interfaceType = interfaceType;
        }

        public AdvancedDropdownItem Build()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Classes");

            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(interfaceType);
            foreach (Type type in types)
            {
                if (type.IsAbstract || type.IsInterface) continue;
                if (type.IsSubclassOf(typeof(UnityEngine.Object))) continue;

                AdvancedDropdownItem parent = GetOrCreateParentItem(type, root);
                parent.AddChild(new ClassDropdownItem(type));
            }

            return root;
        }

        private AdvancedDropdownItem GetOrCreateParentItem(Type type, AdvancedDropdownItem root)
        {
            Assert.IsNotNull(type);
            Assert.IsNotNull(root);
            Assert.IsNotNull(splitToItem);

            if (string.IsNullOrEmpty(type.Namespace))
                return root;

            string[] splits = type.Namespace.Split('.');

            AdvancedDropdownItem splitItem = root;

            string currentPath = string.Empty;
            foreach (string split in splits)
            {
                currentPath += split + '.';

                if (splitToItem.TryGetValue(currentPath, out AdvancedDropdownItem foundItem))
                {
                    splitItem = foundItem;
                    continue;
                }

                AdvancedDropdownItem newSplitItem = new AdvancedDropdownItem(split)
                {
                    icon = IconUtility.FolderIcon
                };
                splitToItem.Add(currentPath, newSplitItem);
                splitItem.AddChild(newSplitItem);
                splitItem = newSplitItem;
            }

            return splitItem;
        }
    }
}
