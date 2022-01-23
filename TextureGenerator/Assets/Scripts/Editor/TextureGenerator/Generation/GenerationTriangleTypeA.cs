using System.IO;
using UnityEngine;
using UnityEditor;

public class GenerationTriangleTypeA : EditorWindow {
    [MenuItem("TextureGenerator/Generation/TriangleTypeA")]
    public static void OpenWindow() {
        GenerationTriangleTypeA window = GetWindow<GenerationTriangleTypeA>();
        window.Show();
    }

    #region Internal Fields
    private bool _optionChanged = false;
    private Texture2D _previewTexture = null;

    // Color
    private Color _color1 = Color.red;
    private Color _color2 = Color.green;

    // Length
    private int _defaultLength = 128;
    private int _selectedLength = -1;
    private int[] _lengthArray = new int[] { 128, 256, 512, 1024 };
    #endregion

    #region Editor Window Hooks
    private void OnGUI() {
        // Init
        if (_selectedLength == -1) {
            _selectedLength = _defaultLength;
            _optionChanged = true;
        }

        // Color
        EditorGUILayout.LabelField("Color");
        Color color1 = EditorGUILayout.ColorField(_color1);
        if (color1 != _color1) {
            _color1 = color1;
            _optionChanged = true;
        }

        Color color2 = EditorGUILayout.ColorField(_color2);
        if (color2 != _color2) {
            _color2 = color2;
            _optionChanged = true;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Size
        EditorGUILayout.LabelField("Size");
        if (EditorGUILayout.DropdownButton(new GUIContent(_selectedLength.ToString()), FocusType.Keyboard)) {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < _lengthArray.Length; i++) {
                int length = _lengthArray[i];
                menu.AddItem(new GUIContent(length.ToString()), length == _selectedLength, OnValueSelected, length);
            }

            menu.ShowAsContext();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Preview
        EditorGUILayout.LabelField("Preview");
        if (_optionChanged) {
            if (_previewTexture == null) {
                _previewTexture = new Texture2D(128, 128, TextureFormat.ARGB32, false);
            }

            _optionChanged = false;

            Texture2D newTex = new Texture2D(128, 128, TextureFormat.ARGB32, false);
            for (int w = 0; w < 128; w++) {
                for (int h = 0; h < 128; h++) {
                    Color c = w > h ? _color1 : _color2;
                    newTex.SetPixel(w, h, c);
                }
            }

            byte[] byteData = newTex.EncodeToPNG();
            _previewTexture.LoadImage(byteData);
        }

        GUILayout.Box(_previewTexture, GUILayout.Width(_previewTexture.width), GUILayout.Height(_previewTexture.height));

        // Output
        if (GUILayout.Button("Generate !!")) {
            byte[] bytes = _previewTexture.EncodeToPNG();
            File.WriteAllBytes(string.Format("{0}/{1}", Utility.OUTPUT_PATH_ROOT, "NewTriangle.png"), bytes);
            AssetDatabase.Refresh();
        }
    }
    #endregion

    #region Internal Methods
    private void OnValueSelected(object value) {
        int length = (int) value;

        if (length == _selectedLength) {
            return;
        }

        _selectedLength = length;
        _optionChanged = true;
    }
    #endregion
}
