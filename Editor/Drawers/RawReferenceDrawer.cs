using System;
using System.Reflection;
using TNRD.Utilities;
using UnityEditor;
using UnityEngine;

namespace TNRD.Drawers
{
    internal class RawReferenceDrawer : ReferenceDrawer, IReferenceDrawer
    {
        private readonly GUIContent label;
        private readonly FieldInfo fieldInfo;

        private static object previousReferenceValue;
        private static string previousPropertyPath;

        private object RawReferenceValue
        {
            get
            {
#if UNITY_2021_1_OR_NEWER
                return RawReferenceProperty.managedReferenceValue;
#else
                ISerializableInterface instance =
                    (ISerializableInterface)fieldInfo.GetValue(Property.serializedObject.targetObject);
                return instance.GetRawReference();
#endif
            }

            set
            {
#if UNITY_2021_1_OR_NEWER
                RawReferenceProperty.managedReferenceValue = value;
#else
                fieldInfo.SetValue(Property.serializedObject.targetObject, value);
#endif
            }
        }

        /// <inheritdoc />
        public RawReferenceDrawer(SerializedProperty property, GUIContent label, Type genericType, FieldInfo fieldInfo)
            : base(property, genericType)
        {
            this.label = label;
            this.fieldInfo = fieldInfo;
        }

        /// <inheritdoc />
        public float GetHeight()
        {
            if (RawReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight +
                   EditorGUIUtility.standardVerticalSpacing +
                   EditorGUI.GetPropertyHeight(RawReferenceProperty, true);
        }

        /// <inheritdoc />
        public void OnGUI(Rect position)
        {
            AvoidDuplicateReferencesInArray();

            Rect objectFieldRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            object rawReferenceValue = RawReferenceValue;

            GUIContent content = rawReferenceValue == null
                ? EditorGUIUtility.ObjectContent((MonoScript)null, typeof(MonoScript))
                : new GUIContent(rawReferenceValue.GetType().Name, IconUtility.ScriptIcon);

            CustomObjectDrawer.OnGUI(objectFieldRect, label, content);
            if (rawReferenceValue == null)
                return;

            Rect objectDrawerRect = new Rect(position);
            objectDrawerRect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(objectDrawerRect,
                RawReferenceProperty,
                new GUIContent(rawReferenceValue.GetType().Name),
                true);

            HandleDragAndDrop(objectFieldRect);

            previousPropertyPath = Property.propertyPath;
        }

        /// <inheritdoc />
        protected override void OnPropertiesClicked()
        {
            Type type = RawReferenceValue.GetType();
            string typeName = type.Name;

            string[] guids = AssetDatabase.FindAssets($"t:Script {typeName}");
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                if (monoScript.GetClass() == type)
                {
                    PropertyEditorUtility.Show(monoScript);
                    return;
                }
            }
        }

        private void AvoidDuplicateReferencesInArray()
        {
            if (!IsPropertyInArray(Property)) return;
            if (previousPropertyPath == null) return;
            if (previousPropertyPath == Property.propertyPath) return;

            var rawReferenceProperty = Property.FindPropertyRelative("rawReference");
            var currentReferenceValue = RawReferenceValue;

            if (currentReferenceValue == null) return;

            if (previousReferenceValue == currentReferenceValue)
            {
                RawReferenceValue = CreateInstance(currentReferenceValue);
                rawReferenceProperty.serializedObject.ApplyModifiedProperties();
            }

            previousReferenceValue = currentReferenceValue;
        }

        private static bool IsPropertyInArray(SerializedProperty prop)
        {
            return prop.propertyPath.Contains(".Array.data[");
        }

        private static object CreateInstance(object source)
        {
            var instance = Activator.CreateInstance(source.GetType());
            EditorUtility.CopySerializedManagedFieldsOnly(source, instance);
            return instance;
        }
    }
}