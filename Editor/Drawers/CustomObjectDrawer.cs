using UnityEditor;
using UnityEngine;

namespace TNRD.Drawers
{
    public partial class CustomObjectDrawer
    {
        public delegate void ButtonClickedDelegate(Rect position);

        public delegate void ClickedDelegate();

        public delegate void DeletePressedDelegate();

        public delegate void PropertiesClickedDelegate();

        private bool isSelected;

        private Event Event => Event.current;

        public event ButtonClickedDelegate ButtonClicked;
        public event ClickedDelegate Clicked;
        public event DeletePressedDelegate DeletePressed;
        public event PropertiesClickedDelegate PropertiesClicked;

        public void OnGUI(Rect position, GUIContent label, GUIContent content)
        {
            Rect positionWithoutThumb = new Rect(position);
            positionWithoutThumb.xMax -= 20;

            position = DrawPrefixLabel(position, label);
            DrawObjectField(position, content);
            DrawButton(position);

            HandleMouseDown(position, positionWithoutThumb);
            HandleKeyDown();
        }

        private Rect DrawPrefixLabel(Rect position, GUIContent label)
        {
            GUIStyle labelStyle = isSelected ? Styles.SelectedLabelStyle : Styles.RegularLabelStyle;
            Rect result = EditorGUI.PrefixLabel(position, label, labelStyle);
            return result;
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

        private void DrawButton(Rect position)
        {
            Rect buttonRect = new Rect(position);
            buttonRect.yMin += 1;
            buttonRect.yMax -= 1;
            buttonRect.xMin = buttonRect.xMax - 20;
            buttonRect.xMax -= 1;

            if (GUI.Button(buttonRect, string.Empty, "objectFieldButton"))
            {
                ButtonClicked?.Invoke(position);
            }
        }

        private void HandleMouseDown(Rect position, Rect positionWithoutThumb)
        {
            if (Event.type != EventType.MouseDown)
                return;

            if (Event.button == 0)
            {
                isSelected = positionWithoutThumb.Contains(Event.mousePosition);
                ForceRepaintEditors();
                Clicked?.Invoke();
            }
            else if (Event.button == 1 && positionWithoutThumb.Contains(Event.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Properties..."), false, () => { PropertiesClicked?.Invoke(); });
                menu.DropDown(position);
                Event.Use();
            }
        }

        private void HandleKeyDown()
        {
            if (!isSelected)
                return;

            if (Event.type == EventType.KeyDown && Event.keyCode == KeyCode.Delete)
            {
                DeletePressed?.Invoke();
            }
        }
    }
}