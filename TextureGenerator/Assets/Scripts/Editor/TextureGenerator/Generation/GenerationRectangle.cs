using System.IO;
using UnityEngine;
using UnityEditor;

public class GenerationRectangle : TextureGeneratorWindow<GenerationRectangle> {
    [MenuItem("TextureGenerator/Generation/Rectangle")]
    public static void OpenWindow() {
        GenerationRectangle window = GetWindow<GenerationRectangle>();
        window.Show();
    }

    #region Internal Fields
    private bool _optionChanged = false;
    private Texture2D _previewTexture = null;

    // Color
    private Color _colorRect = Color.red;
    private Color _colorBackground = Color.green;

    // Length
    private int _defaultLength = 128;
    private int _selectedLength = -1;
    private int[] _lengthArray = new int[] { 128, 256, 512, 1024 };

    // Center point, side length
    private Vector2 _centerPos = Vector2.zero;
    private int _width = 0;
    private int _height = 0;
    #endregion

    #region Override Methods
    protected override void OnInit() {
        _selectedLength = _defaultLength;
        _optionChanged = true;
    }

    protected override void OnGUIContent() {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Color
        DrawCommonTitle("Select Color");

        // Color - color rectangle
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rectangle", GUILayout.Width(100));
        Color colorRect = EditorGUILayout.ColorField(_colorRect);
        EditorGUILayout.EndHorizontal();
        if (_colorRect != colorRect) {
            _colorRect = colorRect;
            _optionChanged = true;
        }

        // Color - color background
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Background", GUILayout.Width(100));
        Color colorBG = EditorGUILayout.ColorField(_colorBackground);
        EditorGUILayout.EndHorizontal();
        if (_colorBackground != colorBG) {
            _colorBackground = colorBG;
            _optionChanged = true;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Length
        DrawCommonTitle("Select Texture Length");
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

        // Rectangle options
        DrawCommonTitle("Rectangle Settings");

        // Rectangle options - center point
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Center Point", GUILayout.Width(100));
        int x = EditorGUILayout.IntSlider((int) _centerPos.x, 0, _selectedLength, GUILayout.Width(160));
        int y = EditorGUILayout.IntSlider((int) _centerPos.y, 0, _selectedLength, GUILayout.Width(160));
        EditorGUILayout.EndHorizontal();
        x = Mathf.Clamp(x, 0, _selectedLength);
        y = Mathf.Clamp(y, 0, _selectedLength);
        if (x != _centerPos.x || y != _centerPos.y) {
            _centerPos = new Vector2(x, y);
            _optionChanged = true;
        }

        // Rectangle options - width
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Width", GUILayout.Width(100));
        int width = EditorGUILayout.IntSlider((int) _width, 0, _selectedLength, GUILayout.Width(160));
        EditorGUILayout.EndHorizontal();
        width = Mathf.Clamp(width, 0, _selectedLength);
        if (width != _width) {
            _width = width;
            _optionChanged = true;
        }

        // Rectangle options - height
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Height", GUILayout.Width(100));
        int height = EditorGUILayout.IntSlider((int) _height, 0, _selectedLength, GUILayout.Width(160));
        EditorGUILayout.EndHorizontal();
        height = Mathf.Clamp(height, 0, _selectedLength);
        if (height != _height) {
            _height = height;
            _optionChanged = true;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Preview
        DrawCommonTitle("Preview");
        if (_optionChanged) {
            if (_previewTexture == null) {
                _previewTexture = new Texture2D(_selectedLength, _selectedLength, TextureFormat.ARGB32, false);
            }

            _optionChanged = false;

            Texture2D newTex = new Texture2D(_selectedLength, _selectedLength, TextureFormat.ARGB32, false);
            for (int w = 0; w < _selectedLength; w++) {
                for (int h = 0; h < _selectedLength; h++) {
                    Vector2 point = new Vector2(w, h);
                    Color c = IsPointInRectangle(point, _centerPos, _width, _height) ? _colorRect : _colorBackground;
                    newTex.SetPixel(w, h, c);
                }
            }

            byte[] byteData = newTex.EncodeToPNG();
            _previewTexture.LoadImage(byteData);
        }

        GUILayout.Box(_previewTexture, GUILayout.Width(128), GUILayout.Height(128));

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Output
        DrawGenerationButton(() => {
            string path = EditorUtility.SaveFilePanel("Save File", Utility.DEFAULT_OUTPUT_PATH, Utility.DEFAULT_FILE_NAME_RECTANGLE, "png");
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            byte[] bytes = _previewTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            OnGenerationCompleted();
        });
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

    private bool IsPointInRectangle(Vector2 point, Vector2 rectCenter, int rectWidth, int rectHeight) {
        return Mathf.Abs(point.x - rectCenter.x) <= rectWidth / 2.0f && Mathf.Abs(point.y - rectCenter.y) <= rectHeight / 2.0f;
    }
    #endregion
}
