using System;
using System.Linq;
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

        protected SerializedProperty ReferenceModeProperty => Property.FindPropertyRelative("mode");
        protected SerializedProperty RawReferenceProperty => Property.FindPropertyRelative("rawReference");
        protected SerializedProperty UnityReferenceProperty => Property.FindPropertyRelative("unityReference");

        protected ReferenceDrawer()
        {
            CustomObjectDrawer = new CustomObjectDrawer();
            CustomObjectDrawer.ButtonClicked += OnButtonClicked;
            CustomObjectDrawer.Clicked += OnClicked;
            CustomObjectDrawer.DeletePressed += OnDeletePressed;
            CustomObjectDrawer.PropertiesClicked += OnPropertiesClicked;
        }

        protected void Initialize(SerializedProperty property, Type genericType)
        {
            Property = property;
            GenericType = genericType;
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

        protected void HandleDragAndDrop(Rect position)
        {
            if (Event.current.type == EventType.DragPerform)
            {
                HandleDragPerform();
            }
            else if (Event.current.type == EventType.DragUpdated)
            {
                HandleDragUpdated(position);
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

        private void HandleDragUpdated(Rect position)
        {
            if (!position.Contains(Event.current.mousePosition))
                return;

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

                if (scriptType.IsSubclassOf(typeof(UnityEngine.Object)))
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
                    RawReferenceProperty.managedReferenceValue =
                        Activator.CreateInstance(((MonoScript)DragAndDrop.objectReferences[0]).GetClass());
                    ReferenceModeProperty.enumValueIndex = (int)ReferenceMode.Raw;
                    break;
                case DragAndDropMode.Unity:
                    UnityReferenceProperty.objectReferenceValue = DragAndDrop.objectReferences[0];
                    ReferenceModeProperty.enumValueIndex = (int)ReferenceMode.Unity;
                    break;
                case DragAndDropMode.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
