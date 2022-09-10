using System;
using System.Linq;
using System.Reflection;
using TNRD.Utilities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TNRD.Drawers
{
    internal abstract class ReferenceDrawer
    {
        private enum DragAndDropMode
        {
            None,
            Raw,
            Unity
        }

        private DragAndDropMode dragAndDropMode;

        protected readonly CustomObjectDrawer CustomObjectDrawer;

        protected SerializedProperty Property { get; private set; }
        protected Type GenericType { get; private set; }

        protected SerializedProperty ReferenceModeProperty => Property.ReferenceModeProperty();
        protected SerializedProperty RawReferenceProperty => Property.RawReferenceProperty();
        protected SerializedProperty UnityReferenceProperty => Property.UnityReferenceProperty();

        protected FieldInfo FieldInfo { get; private set; }

        protected ReferenceMode ModeValue
        {
            get => GetModeValue(Property);
            set => SetModeValue(Property, value);
        }

        protected object RawReferenceValue
        {
            get => GetRawReferenceValue(Property);
            set => SetRawReferenceValue(Property, value);
        }

        protected object PropertyValue
        {
            get => GetPropertyValue(Property);
            set => SetPropertyValue(Property, value);
        }

        protected ReferenceDrawer()
        {
            CustomObjectDrawer = new CustomObjectDrawer();
            CustomObjectDrawer.ButtonClicked += OnButtonClicked;
            CustomObjectDrawer.Clicked += OnClicked;
            CustomObjectDrawer.DeletePressed += OnDeletePressed;
            CustomObjectDrawer.PropertiesClicked += OnPropertiesClicked;
        }

        protected void Initialize(SerializedProperty property, Type genericType, FieldInfo fieldInfo)
        {
            Property = property;
            GenericType = genericType;
            FieldInfo = fieldInfo;
        }

        private void OnButtonClicked(Rect position, SerializedProperty property)
        {
            AdvancedDropdownState state = new AdvancedDropdownState();
            SerializableInterfaceAdvancedDropdown dropdown =
                new SerializableInterfaceAdvancedDropdown(state, GenericType, GetRelevantScene(property), property);
            dropdown.ItemSelectedEvent += OnItemSelected;
            dropdown.Show(position);
        }

        private static Scene? GetRelevantScene(SerializedProperty property)
        {
            Object target = property.serializedObject.targetObject;

            if (target is ScriptableObject)
                return null;
            if (target is Component component)
                return component.gameObject.scene;
            if (target is GameObject gameObject)
                return gameObject.scene;

            return null;
        }

        private void OnClicked(SerializedProperty property)
        {
            PingObject(property);
        }

        private void OnDeletePressed(SerializedProperty property)
        {
            SetModeValue(property, default);
            SetPropertyValue(property, null);
        }

        private void OnItemSelected(SerializedProperty property, ReferenceMode mode, object reference)
        {
            SetModeValue(property, mode);
            SetPropertyValue(property, reference);
        }

        protected abstract void OnPropertiesClicked(SerializedProperty property);

        protected void HandleDragAndDrop(Rect position)
        {
            if (!position.Contains(Event.current.mousePosition))
                return;

            if (Event.current.type == EventType.DragPerform)
            {
                HandleDragUpdated();
                HandleDragPerform();
            }
            else if (Event.current.type == EventType.DragUpdated)
            {
                HandleDragUpdated();
            }
        }

        private void SetDragAndDropMode(bool success, DragAndDropMode? successMode = null)
        {
            if (success)
            {
                Assert.IsTrue(successMode.HasValue);
                dragAndDropMode = successMode.Value;
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            }
            else
            {
                dragAndDropMode = DragAndDropMode.None;
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }

        private void HandleDragUpdated()
        {
            if (DragAndDrop.objectReferences.Length > 1)
            {
                SetDragAndDropMode(false);
                return;
            }

            Object objectReference = DragAndDrop.objectReferences[0];

            if (objectReference is GameObject gameObject)
            {
                Component component = gameObject.GetComponent(GenericType);
                SetDragAndDropMode(component != null, DragAndDropMode.Unity);
            }
            else if (objectReference is MonoScript monoScript)
            {
                Type scriptType = monoScript.GetClass();

                if (scriptType.IsSubclassOf(typeof(Object)))
                {
                    SetDragAndDropMode(false);
                    return;
                }

                if (!GenericType.IsAssignableFrom(scriptType))
                {
                    SetDragAndDropMode(false);
                    return;
                }

                bool isValidDrop = scriptType.GetConstructors().Any(x => x.GetParameters().Length == 0);
                SetDragAndDropMode(isValidDrop, DragAndDropMode.Raw);
            }
            else
            {
                SetDragAndDropMode(GenericType.IsInstanceOfType(objectReference), DragAndDropMode.Unity);
            }
        }

        private void HandleDragPerform()
        {
            switch (dragAndDropMode)
            {
                case DragAndDropMode.Raw:
                    ModeValue = ReferenceMode.Raw;
                    PropertyValue = Activator.CreateInstance(((MonoScript)DragAndDrop.objectReferences[0]).GetClass());
                    break;
                case DragAndDropMode.Unity:
                    ModeValue = ReferenceMode.Unity;
                    PropertyValue = DragAndDrop.objectReferences[0];
                    break;
                case DragAndDropMode.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Object GetUnityObject(Object objectReference)
        {
            if (objectReference is GameObject gameObject)
                return gameObject.GetComponent(GenericType);
            return objectReference;
        }

        protected abstract void PingObject(SerializedProperty property);

        protected ReferenceMode GetModeValue(SerializedProperty property)
        {
            return (ReferenceMode)property.ReferenceModeProperty().enumValueIndex;
        }

        protected void SetModeValue(SerializedProperty property, ReferenceMode mode)
        {
            property.ReferenceModeProperty().enumValueIndex = (int)mode;
        }

        protected object GetRawReferenceValue(SerializedProperty property)
        {
#if UNITY_2021_2_OR_NEWER
            return property.RawReferenceProperty().managedReferenceValue;
#else
            ISerializableInterface target = (ISerializableInterface) SerializedPropertyUtilities.GetValue(property);
            return target.GetRawReference();
#endif
        }

        protected void SetRawReferenceValue(SerializedProperty property, object value)
        {
#if UNITY_2021_1_OR_NEWER
            property.RawReferenceProperty().managedReferenceValue = value;
#else
            FieldInfo.SetValue(property.serializedObject.targetObject, value);
#endif
        }

        protected Object GetUnityReferenceValue(SerializedProperty property)
        {
            return property.UnityReferenceProperty().objectReferenceValue;
        }

        protected void SetUnityReferenceValue(SerializedProperty property, object value)
        {
            property.UnityReferenceProperty().objectReferenceValue = GetUnityObject((Object)value);
        }

        protected object GetPropertyValue(SerializedProperty property)
        {
            return GetModeValue(property) switch
            {
                ReferenceMode.Raw => GetRawReferenceValue(property),
                ReferenceMode.Unity => GetUnityReferenceValue(property),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected void SetPropertyValue(SerializedProperty property, object value)
        {
            switch (GetModeValue(property))
            {
                case ReferenceMode.Unity:
                    SetUnityReferenceValue(property, value);
                    SetRawReferenceValue(property, null);
                    break;
                case ReferenceMode.Raw:
                    SetRawReferenceValue(property, value);
                    SetUnityReferenceValue(property, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
