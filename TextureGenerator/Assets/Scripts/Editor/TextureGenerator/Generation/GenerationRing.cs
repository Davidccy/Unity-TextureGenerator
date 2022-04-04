using System.IO;
using UnityEngine;
using UnityEditor;

public class GenerationRing : TextureGeneratorWindow<GenerationRing> {
    [MenuItem("TextureGenerator/Generation/Ring")]
    public static void OpenWindow() {
        GenerationRing window = GetWindow<GenerationRing>();
        window.Show();
    }

    #region Internal Fields
    private bool _optionChanged = false;
    private Texture2D _previewTexture = null;

    // Color
    private Color _colorCircleOutter = Color.red;
    private Color _colorCircleInner = Color.blue;
    private Color _colorBG = Color.green;

    // Length
    private int _defaultLength = 128;
    private int _selectedLength = -1;
    private int[] _lengthArray = new int[] { 128, 256, 512, 1024 };

    // Center point, radius
    private Vector2 _centerPos = Vector2.zero;
    private int _radiusOutter = 0;
    private int _radiusInner = 0;
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

        // Color - color circle
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Circle Outter", GUILayout.Width(100));
        Color colorCircleOutter = EditorGUILayout.ColorField(_colorCircleOutter);
        EditorGUILayout.EndHorizontal();
        if (_colorCircleOutter != colorCircleOutter) {
            _colorCircleOutter = colorCircleOutter;
            _optionChanged = true;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Circle Inner", GUILayout.Width(100));
        Color colorCircleInner = EditorGUILayout.ColorField(_colorCircleInner);
        EditorGUILayout.EndHorizontal();
        if (_colorCircleInner != colorCircleInner) {
            _colorCircleInner = colorCircleInner;
            _optionChanged = true;
        }

        // Color - color background
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Background", GUILayout.Width(100));
        Color colorBG = EditorGUILayout.ColorField(_colorBG);
        EditorGUILayout.EndHorizontal();
        if (_colorBG != colorBG) {
            _colorBG = colorBG;
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

        // Circle options
        DrawCommonTitle("Circle Settings");

        // Circle options - center point
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Center Point", GUILayout.Width(100));
        int x = EditorGUILayout.IntSlider((int) _centerPos.x, 0, _selectedLength, GUILayout.Width(200));
        int y = EditorGUILayout.IntSlider((int) _centerPos.y, 0, _selectedLength, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        x = Mathf.Clamp(x, 0, _selectedLength);
        y = Mathf.Clamp(y, 0, _selectedLength);
        if (x != _centerPos.x || y != _centerPos.y) {
            _centerPos = new Vector2(x, y);
            _optionChanged = true;
        }

        // Circle options - radius
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Radius Outter", GUILayout.Width(100));
        int radiusOutter = EditorGUILayout.IntSlider((int) _radiusOutter, 0, _selectedLength, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        _radiusOutter = Mathf.Clamp(_radiusOutter, 0, _selectedLength);
        if (_radiusOutter != radiusOutter) {
            _radiusOutter = radiusOutter;
            _optionChanged = true;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Radius Inner", GUILayout.Width(100));
        int radiusInner = EditorGUILayout.IntSlider((int) _radiusInner, 0, _selectedLength, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        _radiusInner = Mathf.Clamp(_radiusInner, 0, _selectedLength);
        if (_radiusInner != radiusInner) {
            _radiusInner = radiusInner;
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

            bool isInCircleOutter = false;
            bool isInCircleInner = false;
            Texture2D newTex = new Texture2D(_selectedLength, _selectedLength, TextureFormat.ARGB32, false);
            for (int w = 0; w < _selectedLength; w++) {
                for (int h = 0; h < _selectedLength; h++) {
                    Vector2 point = new Vector2(w, h);
                    isInCircleOutter = IsPointInCircle(point, _centerPos, _radiusOutter);
                    isInCircleInner = IsPointInCircle(point, _centerPos, _radiusInner);
                    Color c = isInCircleInner ? _colorCircleInner : isInCircleOutter ? _colorCircleOutter : _colorBG;
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
            string path = EditorUtility.SaveFilePanel("Save File", Utility.DEFAULT_OUTPUT_PATH, Utility.DEFAULT_FILE_NAME_RING, "png");
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

    private bool IsPointInCircle(Vector2 point, Vector2 circleCenter, int circleRadius) {
        return Vector2.Distance(point, circleCenter) < circleRadius;
    }
    #endregion
}
