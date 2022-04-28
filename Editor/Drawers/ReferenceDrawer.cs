using System;
using TNRD.Utilities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TNRD.Drawers
{
    internal abstract class ReferenceDrawer
    {
        protected readonly SerializedProperty Property;
        protected readonly Type GenericType;
        protected readonly CustomObjectDrawer CustomObjectDrawer;

        protected SerializedProperty ReferenceModeProperty => Property.FindPropertyRelative("mode");
        protected SerializedProperty RawReferenceProperty => Property.FindPropertyRelative("rawReference");
        protected SerializedProperty UnityReferenceProperty => Property.FindPropertyRelative("unityReference");

        protected ReferenceDrawer(SerializedProperty property, Type genericType)
        {
            Property = property;
            GenericType = genericType;

            CustomObjectDrawer = new CustomObjectDrawer();
            CustomObjectDrawer.ButtonClicked += OnButtonClicked;
            CustomObjectDrawer.Clicked += OnClicked;
            CustomObjectDrawer.DeletePressed += OnDeletePressed;
            CustomObjectDrawer.PropertiesClicked += OnPropertiesClicked;
        }

        private void OnButtonClicked(Rect position)
        {
            AdvancedDropdownState state = new AdvancedDropdownState();
            SerializableInterfaceAdvancedDropdown dropdown =
                new SerializableInterfaceAdvancedDropdown(state, GenericType, GetRelevantScene());
            dropdown.ItemSelectedEvent += OnItemSelected;
            dropdown.Show(position);
        }

        private Scene? GetRelevantScene()
        {
            Object target = Property.serializedObject.targetObject;

            if (target is ScriptableObject)
                return null;
            if (target is Component component)
                return component.gameObject.scene;
            if (target is GameObject gameObject)
                return gameObject.scene;

            return null;
        }

        private void OnClicked()
        {
            switch ((ReferenceMode)ReferenceModeProperty.enumValueIndex)
            {
                case ReferenceMode.Raw:
                    // No support for pinging raw objects for now (I guess this would ping the MonoScript?)
                    break;
                case ReferenceMode.Unity:
                    EditorGUIUtility.PingObject(UnityReferenceProperty.objectReferenceValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDeletePressed()
        {
            RawReferenceProperty.managedReferenceValue = null;
            UnityReferenceProperty.objectReferenceValue = null;
            Property.serializedObject.ApplyModifiedProperties();
        }

        private void OnItemSelected(ReferenceMode mode, object reference)
        {
            ReferenceModeProperty.enumValueIndex = (int)mode;

            switch (mode)
            {
                case ReferenceMode.Raw:
                    RawReferenceProperty.managedReferenceValue = reference;
                    break;
                case ReferenceMode.Unity:
                    UnityReferenceProperty.objectReferenceValue = (Object)reference;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            Property.serializedObject.ApplyModifiedProperties();
        }

        protected abstract void OnPropertiesClicked();
    }
}
