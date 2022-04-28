using System;
using TNRD.Drawers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TNRD
{
    [CustomPropertyDrawer(typeof(SerializableInterface<>), true)]
    internal sealed class SerializableInterfacePropertyDrawer : PropertyDrawer
    {
        private SerializedProperty serializedProperty;
        private Type genericType;

        private IReferenceDrawer activeDrawer;

        /// <inheritdoc />
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        private void Initialize(SerializedProperty property)
        {
            if (serializedProperty != null)
                return;

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
            activeDrawer = GetReferenceDrawer(activeDrawer, property, label);
            activeDrawer.OnGUI(position);
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