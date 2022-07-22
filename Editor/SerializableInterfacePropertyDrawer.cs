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
        private readonly RawReferenceDrawer rawReferenceDrawer = new RawReferenceDrawer();
        private readonly UnityReferenceDrawer unityReferenceDrawer = new UnityReferenceDrawer();

        private SerializedProperty serializedProperty;
        private Type genericType;

        /// <inheritdoc />
        public override bool CanCacheInspectorGUI(SerializedProperty property) => false;

        private void Initialize(SerializedProperty property)
        {
            if (serializedProperty == property)
                return;

            serializedProperty = property;
            genericType = GetGenericArgument();
            Assert.IsNotNull(genericType, "Unable to find generic argument, are you doing some shady inheritance?");
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            return GetReferenceDrawer(property, label).GetHeight();
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            GetReferenceDrawer(property, label).OnGUI(position);
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

        private IReferenceDrawer GetReferenceDrawer(SerializedProperty property, GUIContent label)
        {
            SerializedProperty modeProperty = serializedProperty.FindPropertyRelative("mode");
            ReferenceMode referenceMode = (ReferenceMode)modeProperty.enumValueIndex;

            switch (referenceMode)
            {
                case ReferenceMode.Raw:
                    rawReferenceDrawer.Initialize(property, genericType, label, fieldInfo);
                    return rawReferenceDrawer;
                case ReferenceMode.Unity:
                    unityReferenceDrawer.Initialize(property, genericType, label, fieldInfo);
                    return unityReferenceDrawer;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
