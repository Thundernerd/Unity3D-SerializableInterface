#if UNITY_2022_3_20 || UNITY_2022_3_21
#define UNITY_2022_3_20_OR_NEWER
using TNRD.Utilities;
#endif
using TNRD.Utilities;
using UnityEditor;
using UnityEngine;

namespace TNRD.Drawers
{
    public partial class CustomObjectDrawer
    {
        public delegate void ButtonClickedDelegate(Rect position, SerializedProperty property);

        public delegate void ClickedDelegate(SerializedProperty property);

        public delegate void DeletePressedDelegate(SerializedProperty property);

        public delegate void PropertiesClickedDelegate(SerializedProperty property);

        private bool isSelected;

        private Event Event => Event.current;

        public event ButtonClickedDelegate ButtonClicked;
        public event ClickedDelegate Clicked;
        public event DeletePressedDelegate DeletePressed;
        public event PropertiesClickedDelegate PropertiesClicked;

        public void OnGUI(Rect position, GUIContent label, GUIContent content, SerializedProperty property, bool hasChildren)
        {
            Rect positionWithoutThumb = new Rect(position);
            positionWithoutThumb.xMax -= 20;

            position = DrawPrefixLabel(position, label, hasChildren);
            DrawObjectField(position, content);
            DrawButton(position, property);

            HandleMouseDown(position, positionWithoutThumb, property);
            HandleKeyDown(property);
        }

        private Rect DrawPrefixLabel(Rect position, GUIContent label, bool hasChildren)
        {
#if UNITY_2022_3_20_OR_NEWER
            GUIStyle labelStyle = isSelected ? Styles.SelectedLabelStyle : Styles.RegularLabelStyle;
            
            if (hasChildren || EditorGUI.indentLevel >= 1)
            {
                using (new EditorGUI.IndentLevelScope(-1))
                {
                    position.xMin -= ReflectedEditorGUI.indent;
                }    
            }

            Rect result = EditorGUI.PrefixLabel(position, label, labelStyle);
            
            if (hasChildren || EditorGUI.indentLevel >= 1)
            {
                result.xMin -= ReflectedEditorGUI.indentWidth;
            }

            return result;
#elif UNITY_2022_2_OR_NEWER
            if (hasChildren || EditorGUI.indentLevel >= 1)
            {
                position.xMin += ReflectedEditorGUI.indentWidth;
            }

            GUIStyle labelStyle = isSelected ? Styles.SelectedLabelStyle : Styles.RegularLabelStyle;
            Rect result = EditorGUI.PrefixLabel(position, label, labelStyle);
            if (hasChildren || EditorGUI.indentLevel>=1)
                result.xMin -= ReflectedEditorGUI.indentWidth;
            return result;
#else
            GUIStyle labelStyle = isSelected ? Styles.SelectedLabelStyle : Styles.RegularLabelStyle;
            Rect result = EditorGUI.PrefixLabel(position, label, labelStyle);
            return result;
#endif
        }

        private void DrawObjectField(Rect position, GUIContent objectFieldContent)
        {
            Rect positionWithoutThumb = new Rect(position);
            positionWithoutThumb.xMax -= 20;

            if (Event.type == EventType.Repaint)
            {
                EditorStyles.objectField.Draw(position,
                    objectFieldContent,
                    position.Contains(Event.mousePosition),
                    false,
                    false,
                    isSelected);
            }
        }

        private void ForceRepaintEditors()
        {
            foreach (Editor activeEditor in ActiveEditorTracker.sharedTracker.activeEditors)
            {
                activeEditor.Repaint();
            }
        }

        private void DrawButton(Rect position, SerializedProperty property)
        {
            Rect buttonRect = new Rect(position);
            buttonRect.yMin += 1;
            buttonRect.yMax -= 1;
            buttonRect.xMin = buttonRect.xMax - 20;
            buttonRect.xMax -= 1;

            if (GUI.Button(buttonRect, string.Empty, "objectFieldButton"))
            {
                ButtonClicked?.Invoke(position, property);
            }
        }

        private void HandleMouseDown(Rect position, Rect positionWithoutThumb, SerializedProperty property)
        {
            if (Event.type != EventType.MouseDown)
                return;

            if (Event.button == 0)
            {
                isSelected = positionWithoutThumb.Contains(Event.mousePosition);
                ForceRepaintEditors();
                if (isSelected)
                {
                    Clicked?.Invoke(property);
                }
            }
            else if (Event.button == 1 && positionWithoutThumb.Contains(Event.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Clear"), false, () => { DeletePressed?.Invoke(property); });
                menu.AddItem(new GUIContent("Properties..."), false, () => { PropertiesClicked?.Invoke(property); });
                menu.DropDown(position);
                Event.Use();
            }
        }

        private void HandleKeyDown(SerializedProperty property)
        {
            if (!isSelected)
                return;

            if (Event.type == EventType.KeyDown && Event.keyCode == KeyCode.Delete)
            {
                DeletePressed?.Invoke(property);
            }
        }
    }
}
