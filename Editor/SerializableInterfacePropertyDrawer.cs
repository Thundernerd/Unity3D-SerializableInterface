using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TNRD
{
    [CustomPropertyDrawer(typeof(SerializableInterface<>), true)]
    internal sealed partial class SerializableInterfacePropertyDrawer : PropertyDrawer
    {
        private SerializedProperty serializedProperty;
        private bool labelSelected;
        private bool objectPickerSelected;

        private bool IsSelected => labelSelected || objectPickerSelected;

        private SerializedProperty RawReferenceProperty => serializedProperty.FindPropertyRelative("rawReference");
        private SerializedProperty UnityReferenceProperty => serializedProperty.FindPropertyRelative("unityReference");
        private SerializedProperty ReferenceModeProperty => serializedProperty.FindPropertyRelative("mode");

        private ReferenceMode ReferenceMode => (ReferenceMode)ReferenceModeProperty.enumValueIndex;

        private object RawReferenceValue
        {
            get
            {
#if UNITY_2021_1_OR_NEWER
                return RawReferenceProperty.managedReferenceValue;
#else
                ISerializableInterface instance =
                    (ISerializableInterface)fieldInfo.GetValue(serializedProperty.serializedObject.targetObject);
                return instance.GetRawReference();
#endif
            }
        }

        /// <inheritdoc />
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            serializedProperty = property;
            switch (ReferenceMode)
            {
                case ReferenceMode.Raw:
                    return EditorGUIUtility.singleLineHeight +
                           EditorGUIUtility.standardVerticalSpacing +
                           EditorGUI.GetPropertyHeight(RawReferenceProperty, true);
                case ReferenceMode.Unity:
                    return EditorGUIUtility.singleLineHeight;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            serializedProperty = property;
            Type genericArgument = GetGenericArgument();
            Assert.IsNotNull(genericArgument, "Unable to find generic argument, are you doing some shady inheritance?");

            switch (ReferenceMode)
            {
                case ReferenceMode.Raw:
                    DrawRawReferenceMode(position, property, label, genericArgument);
                    break;
                case ReferenceMode.Unity:
                    DrawUnityReferenceMode(position, property, label, genericArgument);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            HandleDeleteButton();
        }

        private Type GetGenericArgument()
        {
            Type type = fieldInfo.FieldType;

            while (type != null)
            {
                if (type.IsGenericType)
                {
                    if (type.GetGenericTypeDefinition() == typeof(SerializableInterface<>))
                    {
                        return type.GetGenericArguments()[0];
                    }

                    type = type.BaseType;
                }
                else
                {
                    type = type.BaseType;
                }
            }

            return null;
        }

        private void DrawRawReferenceMode(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            Type genericArgument
        )
        {
            Rect objectFieldRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            objectFieldRect = DrawPrefixLabel(objectFieldRect, label);
            DrawRawReference(objectFieldRect);
            DrawButton(objectFieldRect, property, genericArgument);

            Rect objectDrawerRect = new Rect(position);
            objectDrawerRect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(objectDrawerRect,
                RawReferenceProperty,
                new GUIContent(RawReferenceValue.GetType().Name),
                true);
        }

        private void DrawRawReference(Rect position)
        {
            Type type = RawReferenceValue.GetType();
            string typeName = type.Name;
            IEnumerable<MonoScript> scripts = AssetDatabase.FindAssets($"t:Script {typeName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<MonoScript>);
            MonoScript monoScript = scripts.FirstOrDefault(x => x.GetClass() == type);

            DrawObjectField(position, new GUIContent(typeName, IconUtility.ScriptIcon), monoScript);
        }

        private void DrawUnityReferenceMode(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            Type genericArgument
        )
        {
            position = DrawPrefixLabel(position, label);
            DrawUnityReference(position);
            DrawButton(position, property, genericArgument);
        }


        private void DrawUnityReference(Rect position)
        {
            Object unityReference = UnityReferenceProperty.objectReferenceValue;
            Type referenceType = unityReference == null ? typeof(Object) : unityReference.GetType();
            DrawObjectField(position, EditorGUIUtility.ObjectContent(unityReference, referenceType), unityReference);
        }

        private Rect DrawPrefixLabel(Rect position, GUIContent label)
        {
            GUIStyle labelStyle = IsSelected ? Styles.SelectedLabelStyle : Styles.RegularLabelStyle;
            Rect result = EditorGUI.PrefixLabel(position, label, labelStyle);

            if (Event.current.type == EventType.MouseDown)
            {
                Rect delta = new Rect(position)
                {
                    width = position.width - result.width
                };

                labelSelected = delta.Contains(Event.current.mousePosition);
                RepaintActiveInspector();
            }

            return result;
        }

        private void DrawObjectField(Rect position, GUIContent objectFieldContent, Object objectToShow)
        {
            Rect positionWithoutThumb = new Rect(position);
            positionWithoutThumb.xMax -= 20;

            Event evt = Event.current;
            if (evt.type == EventType.Repaint)
            {
                EditorStyles.objectField.Draw(position,
                    objectFieldContent,
                    position.Contains(evt.mousePosition),
                    false,
                    false,
                    IsSelected);
            }

            HandleObjectFieldMouseDown(position, objectToShow, evt, positionWithoutThumb);
        }

        private void HandleObjectFieldMouseDown(
            Rect position,
            Object objectToShow,
            Event evt,
            Rect positionWithoutThumb
        )
        {
            if (evt.type != EventType.MouseDown)
                return;

            if (evt.button == 0)
            {
                objectPickerSelected = positionWithoutThumb.Contains(evt.mousePosition);
                PingObject();
                RepaintActiveInspector();
            }
            else if (evt.button == 1 && positionWithoutThumb.Contains(evt.mousePosition))
            {
                if (objectToShow != null)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Properties..."),
                        false,
                        () => { PropertyEditorUtility.Show(objectToShow); });
                    menu.DropDown(position);
                }

                evt.Use();
            }
        }

        private void PingObject()
        {
            if (!IsSelected)
                return;

            switch (ReferenceMode)
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

        private void RepaintActiveInspector()
        {
            // Forcing a repaint of the inspector for this object, not the prettiest but it works
            foreach (Editor activeEditor in ActiveEditorTracker.sharedTracker.activeEditors)
            {
                if (activeEditor.serializedObject == serializedProperty.serializedObject)
                {
                    activeEditor.Repaint();
                }
            }
        }

        private void DrawButton(Rect position, SerializedProperty property, Type genericArgument)
        {
            Rect buttonRect = new Rect(position);
            buttonRect.yMin += 1;
            buttonRect.yMax -= 1;
            buttonRect.xMin = buttonRect.xMax - 20;
            buttonRect.xMax -= 1;

            if (GUI.Button(buttonRect, string.Empty, "objectFieldButton"))
            {
                AdvancedDropdownState state = new AdvancedDropdownState();
                SerializableInterfaceAdvancedDropdown dropdown =
                    new SerializableInterfaceAdvancedDropdown(state, genericArgument, GetRelevantScene());
                dropdown.ItemSelectedEvent += (mode, reference) =>
                {
                    DropdownOnItemSelectedEvent(property, mode, reference);
                };
                dropdown.Show(position);
            }
        }

        private Scene? GetRelevantScene()
        {
            Object target = serializedProperty.serializedObject.targetObject;

            if (target is ScriptableObject)
                return null;
            if (target is Component component)
                return component.gameObject.scene;
            if (target is GameObject gameObject)
                return gameObject.scene;

            return null;
        }

        private void DropdownOnItemSelectedEvent(SerializedProperty property, ReferenceMode mode, object reference)
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

            property.serializedObject.ApplyModifiedProperties();
        }

        private void HandleDeleteButton()
        {
            if (!IsSelected)
                return;

            Event evt = Event.current;
            if (evt.type != EventType.KeyDown || evt.keyCode != KeyCode.Delete)
                return;

            // Setting this to Unity because we don't handle null cases for Raw mode
            ReferenceModeProperty.enumValueIndex = (int)ReferenceMode.Unity;
            RawReferenceProperty.managedReferenceValue = null;
            UnityReferenceProperty.objectReferenceValue = null;

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}