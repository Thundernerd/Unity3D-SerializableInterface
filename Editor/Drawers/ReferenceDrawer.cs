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

        protected SerializedProperty ReferenceModeProperty => Property.FindPropertyRelative("mode");
        protected SerializedProperty RawReferenceProperty => Property.FindPropertyRelative("rawReference");
        protected SerializedProperty UnityReferenceProperty => Property.FindPropertyRelative("unityReference");

        protected FieldInfo FieldInfo { get; private set; }

        protected ReferenceMode ModeValue
        {
            get => (ReferenceMode)ReferenceModeProperty.enumValueIndex;
            set => ReferenceModeProperty.enumValueIndex = (int)value;
        }

        protected object RawReferenceValue
        {
            get
            {
#if UNITY_2021_1_OR_NEWER
                return RawReferenceProperty.managedReferenceValue;
#else
                ISerializableInterface instance =
                    (ISerializableInterface)FieldInfo.GetValue(Property.serializedObject.targetObject);
                return instance.GetRawReference();
#endif
            }
            set
            {
#if UNITY_2021_1_OR_NEWER
                RawReferenceProperty.managedReferenceValue = value;
#else
                FieldInfo.SetValue(Property.serializedObject.targetObject, value);
#endif
            }
        }

        protected object PropertyValue
        {
            get
            {
                return ModeValue switch
                {
                    ReferenceMode.Raw => RawReferenceValue,
                    ReferenceMode.Unity => UnityReferenceProperty.objectReferenceValue,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set
            {
                switch (ModeValue)
                {
                    case ReferenceMode.Raw:
                        RawReferenceValue = value;
                        UnityReferenceProperty.objectReferenceValue = null;
                        break;
                    case ReferenceMode.Unity:
                        UnityReferenceProperty.objectReferenceValue = GetUnityObject((Object)value);
                        RawReferenceValue = null;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Property.serializedObject.ApplyModifiedProperties();
            }
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
            PingObject();
        }

        private void OnDeletePressed()
        {
            ModeValue = default;
            PropertyValue = null;
        }

        private void OnItemSelected(ReferenceMode mode, object reference)
        {
            ModeValue = mode;
            PropertyValue = reference;
        }

        protected abstract void OnPropertiesClicked();

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
            if(objectReference is GameObject gameObject)
                return gameObject.GetComponent(GenericType);
            return objectReference;
        }

        protected abstract void PingObject();
    }
}
