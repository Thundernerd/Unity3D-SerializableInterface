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
        private const string k_RawReference = "rawReference";

        private SerializedProperty serializedProperty;
        private object previousReferenceValue;
        private string lastPropertyPath;
        private Type genericType;

        private IReferenceDrawer activeDrawer;

        /// <inheritdoc />
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        private void Initialize(SerializedProperty property)
        {
            serializedProperty = property;
            activeDrawer = null;
            genericType = GetGenericArgument();

            Assert.IsNotNull(genericType, "Unable to find generic argument, are you doing some shady inheritance?");
        }

        /// <summary>
        /// Avoids an unexpected behaviour
        /// </summary>
        private void AvoidDuplicateReferences(SerializedProperty property)
        {
            if (!IsTargetObjectArray(property)) return; // This function 
            if (lastPropertyPath == null) return;
            if (lastPropertyPath == property.propertyPath) return; // only one element

            var rawReferenceProperty = property.FindPropertyRelative(k_RawReference); // Cache property
            var currentReferenceValue = rawReferenceProperty.managedReferenceValue;

            if (currentReferenceValue == null) return; // Value is null. Probably new or not set yet

            if (previousReferenceValue == currentReferenceValue)
            {
                // The best behaviour would be to create a shallow copy. The extension method from SolidUtilities works perfectly.
                // Using serialization or reflection might also work
                var instance = Activator.CreateInstance(currentReferenceValue.GetType());
                rawReferenceProperty.managedReferenceValue = instance;

                // propertyRelative.managedReferenceValue = currentReferenceValue.ShallowCopy();
            }

            previousReferenceValue = currentReferenceValue;
        }


        private static bool IsTargetObjectArray(SerializedProperty prop)
        {
            return prop.propertyPath.Contains(".Array.data[");
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
            AvoidDuplicateReferences(property);

            activeDrawer = GetReferenceDrawer(activeDrawer, property, label);
            activeDrawer.OnGUI(position);

            lastPropertyPath = property.propertyPath;
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
            SerializedProperty modeProperty = serializedProperty.FindPropertyRelative("mode");
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