using System;
using TNRD.Utilities;
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
            HandleDragAndDrop(position);
        }

        private void HandleDragAndDrop(Rect position)
        {
            if (Event.current.type == EventType.DragPerform)
            {
                UnityReferenceProperty.objectReferenceValue = DragAndDrop.objectReferences[0];
                return;
            }

            if (Event.current.type != EventType.DragUpdated) 
                return;

            if (!position.Contains(Event.current.mousePosition))
                return;

            if (DragAndDrop.objectReferences.Length > 1)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                return;
            }

            Object objectReference = DragAndDrop.objectReferences[0];

            if (objectReference is GameObject gameObject)
            {
                Component component = gameObject.GetComponent(GenericType);
                DragAndDrop.visualMode = component != null
                    ? DragAndDropVisualMode.Link
                    : DragAndDropVisualMode.Rejected;
            }
            else
            {
                DragAndDrop.visualMode = GenericType.IsInstanceOfType(objectReference)
                    ? DragAndDropVisualMode.Link
                    : DragAndDropVisualMode.Rejected;
            }
        }

        protected override void OnPropertiesClicked()
        {
            PropertyEditorUtility.Show(UnityReferenceProperty.objectReferenceValue);
        }
    }
}
