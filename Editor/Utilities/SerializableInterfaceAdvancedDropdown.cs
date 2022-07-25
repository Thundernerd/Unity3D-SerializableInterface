using System;
using System.Linq;
using System.Reflection;
using TNRD.Builders;
using TNRD.Items;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace TNRD.Utilities
{
    internal sealed class SerializableInterfaceAdvancedDropdown : AdvancedDropdown
    {
        private readonly Type interfaceType;
        private readonly MethodInfo sortChildrenMethod;
        private readonly bool canSort;
        private readonly Scene? relevantScene;
        private readonly SerializedProperty property;

        public delegate void ItemSelectedDelegate(SerializedProperty property, ReferenceMode mode, object reference);

        public event ItemSelectedDelegate ItemSelectedEvent; // Suffixed with Event because of the override

        /// <inheritdoc />
        public SerializableInterfaceAdvancedDropdown(
            AdvancedDropdownState state,
            Type interfaceType,
            Scene? relevantScene,
            SerializedProperty property
        )
            : base(state)
        {
            Assert.IsNotNull(interfaceType);

            sortChildrenMethod = typeof(AdvancedDropdownItem)
                .GetMethod("SortChildren", BindingFlags.Instance | BindingFlags.NonPublic);
            canSort = sortChildrenMethod != null;

            minimumSize = new Vector2(0, 300);
            this.interfaceType = interfaceType;
            this.relevantScene = relevantScene;
            this.property = property;
        }

        /// <inheritdoc />
        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItemWrapper item = new AdvancedDropdownItemWrapper(interfaceType.Name)
                .AddChild(new AssetsItemBuilder(interfaceType).Build())
                .AddChild(new ClassesItemBuilder(interfaceType).Build())
                .AddChild(new SceneItemBuilder(interfaceType, relevantScene).Build());

            foreach (AdvancedDropdownItem dropdownItem in item.children)
            {
                dropdownItem.AddChild(new NoneDropdownItem());
            }

            if (canSort)
            {
                sortChildrenMethod.Invoke(item,
                    new object[]
                    {
                        (Comparison<AdvancedDropdownItem>)Sort, true
                    });
            }

            return item;
        }

        private int Sort(AdvancedDropdownItem a, AdvancedDropdownItem b)
        {
            // For aesthetic reasons. Always puts the None first
            if (a is NoneDropdownItem)
                return -1;
            if (b is NoneDropdownItem)
                return 1;

            int childrenA = a.children.Count();
            int childrenB = b.children.Count();

            if (childrenA > 0 && childrenB > 0)
                return a.CompareTo(b);
            if (childrenA == 0 && childrenB == 0)
                return a.CompareTo(b);
            if (childrenA > 0 && childrenB == 0)
                return -1;
            return 1;
        }

        /// <inheritdoc />
        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is IDropdownItem dropdownItem)
            {
                ItemSelectedEvent?.Invoke(property, dropdownItem.Mode, dropdownItem.GetValue());
            }
        }
    }
}
