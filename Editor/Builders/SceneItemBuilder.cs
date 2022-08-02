using System;
using System.Linq;
using TNRD.Items;
using TNRD.Utilities;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace TNRD.Builders
{
    internal sealed class SceneItemBuilder
    {
        private readonly Type interfaceType;
        private readonly Scene? scene;

        public SceneItemBuilder(Type interfaceType, Scene? scene)
        {
            Assert.IsNotNull(interfaceType);

            this.interfaceType = interfaceType;
            this.scene = scene;
        }

        public AdvancedDropdownItem Build()
        {
            if (scene == null || !scene.Value.IsValid())
            {
                return new AdvancedDropdownItem("Scene")
                {
                    enabled = false
                };
            }

            AdvancedDropdownItem root = new AdvancedDropdownItemWrapper("Scene");

            GameObject[] rootGameObjects = scene.Value.GetRootGameObjects();

            foreach (GameObject rootGameObject in rootGameObjects)
            {
                CreateItemsRecursive(rootGameObject.transform, root);
            }

            return root;
        }

        private void CreateItemsRecursive(Transform transform, AdvancedDropdownItem parent)
        {
            AdvancedDropdownItem advancedDropdownItem = new AdvancedDropdownItem(transform.name)
            {
                icon = IconUtility.GameObjectIcon
            };

            Component[] components = transform.GetComponents(interfaceType);

            foreach (Component component in components)
            {
                advancedDropdownItem.AddChild(new SceneDropdownItem(component));
            }

            foreach (Transform child in transform)
            {
                CreateItemsRecursive(child, advancedDropdownItem);
            }

            if (advancedDropdownItem.children.Any())
                parent.AddChild(advancedDropdownItem);
        }
    }
}
