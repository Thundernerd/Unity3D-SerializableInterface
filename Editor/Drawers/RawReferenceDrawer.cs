using System;
using System.Reflection;
using TNRD.Utilities;
using UnityEditor;
using UnityEngine;

namespace TNRD.Drawers
{
    internal partial class RawReferenceDrawer : ReferenceDrawer, IReferenceDrawer
    {
        private GUIContent label;
        private FieldInfo fieldInfo;

        private object previousReferenceValue;
        private string previousPropertyPath;

        public void Initialize(SerializedProperty property, Type genericType, GUIContent label, FieldInfo fieldInfo)
        {
            Initialize(property, genericType, fieldInfo);
            this.label = label;
        }

        /// <inheritdoc />
        public float GetHeight()
        {
            if (RawReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUI.GetPropertyHeight(RawReferenceProperty, true);
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

            HandleDragAndDrop(objectFieldRect);

            if (rawReferenceValue == null)
                return;
            
            DrawLine(position);
            DrawProperty(position);
            
            previousPropertyPath = Property.propertyPath;
        }

        private void DrawProperty(Rect position)
        {
            Rect objectDrawerRect = new Rect(position);
            
            EditorGUI.PropertyField(objectDrawerRect,
                RawReferenceProperty,
                GUIContent.none,
                true);
        }

        private void DrawLine(Rect position)
        {
            Rect line = new Rect(position)
            {
                width = 4,
                yMin = position.yMin + EditorGUIUtility.singleLineHeight,
                x = position.x + 2f
            };
            EditorGUI.DrawRect(line, Styles.LineColor);
        }

        protected override void PingObject()
        {
            // No support for pinging raw objects for now (I guess this would ping the MonoScript?)
        }

        /// <inheritdoc />
        protected override void OnPropertiesClicked()
        {
            if (RawReferenceValue == null)
                return;
            
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
            if (!ShouldCheckProperty()) 
                return;

            object currentReferenceValue = RawReferenceValue;

            if (currentReferenceValue == null) 
                return;

            if (previousReferenceValue == currentReferenceValue)
            {
                currentReferenceValue = CreateInstance(currentReferenceValue);
                PropertyValue = currentReferenceValue;
            }

            previousReferenceValue = currentReferenceValue;
        }

        private bool ShouldCheckProperty()
        {
            return IsPropertyInArray(Property) && 
                   previousPropertyPath != null && 
                   previousPropertyPath != Property.propertyPath;
        }

        private static bool IsPropertyInArray(SerializedProperty prop)
        {
            return prop.propertyPath.Contains(".Array.data[");
        }

        private static object CreateInstance(object source)
        {
            object instance = Activator.CreateInstance(source.GetType());
            EditorUtility.CopySerializedManagedFieldsOnly(source, instance);
            return instance;
        }
    }
}
