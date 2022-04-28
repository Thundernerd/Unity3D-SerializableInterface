using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TNRD.Drawers
{
    internal class UnityReferenceDrawer : ReferenceDrawer, IReferenceDrawer
    {
        private readonly GUIContent label;

        public UnityReferenceDrawer(SerializedProperty property, GUIContent label, Type genericType)
            : base(property, genericType)
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
        }

        protected override void OnPropertiesClicked()
        {
            PropertyEditorUtility.Show(UnityReferenceProperty.objectReferenceValue);
        }
    }
}