using System.IO;
using UnityEngine;
using UnityEditor;

public class GenerationSquare : TextureGeneratorWindow<GenerationSquare> {
    [MenuItem("TextureGenerator/Generation/Square")]
    public static void OpenWindow() {
        GenerationSquare window = GetWindow<GenerationSquare>();
        window.Show();
    }

    #region Internal Fields
    private bool _optionChanged = false;
    private Texture2D _previewTexture = null;

    // Color
    private Color _colorSquare = Color.red;
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

        // Color - color square
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Square", GUILayout.Width(100));
        Color colorSquare = EditorGUILayout.ColorField(_colorSquare);
        EditorGUILayout.EndHorizontal();
        if (_colorSquare != colorSquare) {
            _colorSquare = colorSquare;
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

        // Square options
        DrawCommonTitle("Square Settings");

        // Square options - center point
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

        // Square options - width
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Width", GUILayout.Width(100));
        int width = EditorGUILayout.IntSlider((int) _width, 0, _selectedLength, GUILayout.Width(160));
        EditorGUILayout.EndHorizontal();
        width = Mathf.Clamp(width, 0, _selectedLength);
        if (width != _width) {
            _width = width;
            _optionChanged = true;
        }

        // Square options - height
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
                    Color c = IsPointInSquare(point, _centerPos, _width, _height) ? _colorSquare : _colorBackground;
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
            string path = EditorUtility.SaveFilePanel("Save File", Utility.DEFAULT_OUTPUT_PATH, Utility.DEFAULT_FILE_NAME_SQUARE, "png");
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

    private bool IsPointInSquare(Vector2 point, Vector2 squareCenter, int squareWidth, int squareHeight) {
        return Mathf.Abs(point.x - squareCenter.x) <= squareWidth && Mathf.Abs(point.y - squareCenter.y) <= squareHeight;
    }
    #endregion
}
