using System;
using System.Collections;
using TNRD.Drawers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TNRD
{
    [CustomPropertyDrawer(typeof(SerializableInterface<>), true)]
    internal sealed class SerializableInterfacePropertyDrawer : PropertyDrawer
    {
        private const string RAW_REFERENCE = "rawReference";
        private const string MODE = "mode";

        private SerializedProperty serializedProperty;
        private Type genericType;

        private IReferenceDrawer activeDrawer;

        private object previousReferenceValue;
        private string previousPropertyPath;

        /// <inheritdoc />
        public override bool CanCacheInspectorGUI(SerializedProperty property) => false;

        private void Initialize(SerializedProperty property)
        {
            if (serializedProperty == property)
                return;

            activeDrawer = null;
            serializedProperty = property;
            genericType = GetGenericArgument();

            Assert.IsNotNull(genericType, "Unable to find generic argument, are you doing some shady inheritance?");
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            activeDrawer = GetReferenceDrawer(activeDrawer, property, label);
            return activeDrawer.GetHeight();
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            AvoidDuplicateReferencesInArray(property);

            activeDrawer = GetReferenceDrawer(activeDrawer, property, label);
            activeDrawer.OnGUI(position);
            previousPropertyPath = property.propertyPath;
        }

        /// <summary>
        /// When a new instance of <see cref="SerializableInterface{T}"/> is added in an array using the inspector's gizmo,
        /// avoids having their <see cref="SerializableInterface{T}.rawReference"/> field reference the same instance.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        private void AvoidDuplicateReferencesInArray(SerializedProperty property)
        {
            if (!IsPropertyInArray(property)) return;
            if (previousPropertyPath == null) return;
            if (previousPropertyPath == property.propertyPath) return;

            var rawReferenceProperty = property.FindPropertyRelative(RAW_REFERENCE);
            var currentReferenceValue = rawReferenceProperty.managedReferenceValue;

            if (currentReferenceValue == null) return;

            if (previousReferenceValue == currentReferenceValue)
            {
                rawReferenceProperty.managedReferenceValue = CreateInstance(currentReferenceValue);
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

        private Type GetGenericArgument()
        {
            Type type = fieldInfo.FieldType;

            while (type != null)
            {
                if (type.IsArray)
                    type = type.GetElementType();

                if (type == null)
                    return null;

                if (type.IsGenericType)
                {
                    if (typeof(IEnumerable).IsAssignableFrom(type))
                    {
                        type = type.GetGenericArguments()[0];
                    }
                    else if (type.GetGenericTypeDefinition() == typeof(SerializableInterface<>))
                    {
                        return type.GetGenericArguments()[0];
                    }
                    else
                    {
                        type = type.BaseType;
                    }
                }
                else
                {
                    type = type.BaseType;
                }
            }

            return null;
        }

        private IReferenceDrawer GetReferenceDrawer(
            IReferenceDrawer original,
            SerializedProperty property,
            GUIContent label
        )
        {
            SerializedProperty modeProperty = serializedProperty.FindPropertyRelative(MODE);
            ReferenceMode referenceMode = (ReferenceMode)modeProperty.enumValueIndex;

            switch (referenceMode)
            {
                case ReferenceMode.Raw:
                    return original is RawReferenceDrawer
                        ? original
                        : new RawReferenceDrawer(property, label, genericType, fieldInfo);
                case ReferenceMode.Unity:
                    return original is UnityReferenceDrawer
                        ? original
                        : new UnityReferenceDrawer(property, label, genericType);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}