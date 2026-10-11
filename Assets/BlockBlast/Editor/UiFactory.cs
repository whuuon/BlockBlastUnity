using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BlockBlast.Data;

namespace BlockBlast.Editor
{
    public sealed class UiFactory
    {
        private readonly GameTheme theme;
        public UiFactory(GameTheme theme) => this.theme = theme;

        public RectTransform Rect(string name, Transform parent, Vector2 position, Vector2 size)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform)gameObject.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = Vector2.one * 0.5f;
            rect.pivot = Vector2.one * 0.5f;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        public Image Image(string name, Transform parent, Sprite sprite, Vector2 position, Vector2 size)
        {
            Image image = Rect(name, parent, position, size).gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.raycastTarget = false;
            return image;
        }

        public Text Text(string name, Transform parent, string value, Vector2 position, Vector2 size,
            int fontSize, Color? tint = null)
        {
            Text text = Rect(name, parent, position, size).gameObject.AddComponent<Text>();
            text.font = theme.Font;
            text.text = value;
            text.fontSize = fontSize;
            text.color = tint ?? Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            text.supportRichText = true;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        public Button Button(string name, Transform parent, string title, Vector2 position, Vector2 size,
            UnityAction action, int fontSize = 28)
        {
            Image image = Image(name, parent, theme.Button, position, size);
            image.type = UnityEngine.UI.Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 4f;
            image.raycastTarget = true;
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(1f, 0.94f, 0.8f);
            colors.pressedColor = new Color(0.75f, 0.75f, 0.75f);
            button.colors = colors;
            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
            UnityEventTools.AddPersistentListener(button.onClick, action);
            Text label = Text("Label", image.transform, title, Vector2.zero, size - new Vector2(24, 12), fontSize,
                new Color(0.22f, 0.1f, 0.02f));
            label.fontStyle = FontStyle.Bold;
            return button;
        }

        public RectTransform Overlay(string name, Transform parent)
        {
            Image image = Image(name, parent, theme.Background, Vector2.zero, new Vector2(720, 1280));
            Stretch(image.rectTransform);
            image.raycastTarget = true;
            return image.rectTransform;
        }

        public static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        public static void Assign(Object target, string property, Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(property).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void AssignArray<T>(Object target, string property, T[] values) where T : Object
        {
            var serialized = new SerializedObject(target);
            SerializedProperty array = serialized.FindProperty(property);
            array.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++) array.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
