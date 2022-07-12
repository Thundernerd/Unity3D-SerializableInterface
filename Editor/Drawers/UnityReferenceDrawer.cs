using System;
using System.Reflection;
using TNRD.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TNRD.Drawers
{
    internal class UnityReferenceDrawer : ReferenceDrawer, IReferenceDrawer
    {
        private readonly GUIContent label;

        public UnityReferenceDrawer(SerializedProperty property, GUIContent label, Type genericType, FieldInfo fieldInfo)
            : base(property, genericType, fieldInfo)
        {
            this.label = label;
        }

        public float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public void OnGUI(Rect position)
        {
            Object unityReference = UnityReferenceProperty.objectReferenceValue;
            Type referenceType = unityReference == null ? typeof(Object) : unityReference.GetType();
            GUIContent objectContent = EditorGUIUtility.ObjectContent(unityReference, referenceType);
            CustomObjectDrawer.OnGUI(position, label, objectContent);
            HandleDragAndDrop(position);
        }

        protected override void PingObject()
        {
            EditorGUIUtility.PingObject((Object)PropertyValue);
        }

        protected override void OnPropertiesClicked()
        {
            PropertyEditorUtility.Show(UnityReferenceProperty.objectReferenceValue);
        }
    }
}
