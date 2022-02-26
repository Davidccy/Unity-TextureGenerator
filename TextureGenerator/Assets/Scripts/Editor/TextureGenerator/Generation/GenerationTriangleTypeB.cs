using System.IO;
using UnityEngine;
using UnityEditor;

public class GenerationTriangleTypeB : TextureGeneratorWindow<GenerationTriangleTypeB> {
    [MenuItem("TextureGenerator/Generation/TriangleTypeB")]
    public static void OpenWindow() {
        GenerationTriangleTypeB window = GetWindow<GenerationTriangleTypeB>();
        window.Show();
    }

    #region Internal Fields
    private bool _optionChanged = false;
    private Texture2D _previewTexture = null;

    // Color
    private Color _colorTriangle = Color.red;
    private Color _colorBG = Color.green;

    // Length
    private int _defaultLength = 128;
    private int _selectedLength = -1;

    private int[] _lengthArray = new int[] { 128, 256, 512, 1024 };

    // Point
    private Vector2 _point1Pos = Vector2.zero;
    private Vector2 _point2Pos = Vector2.zero;
    private Vector2 _point3Pos = Vector2.zero;
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

        // Color - color triangle
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Triangle", GUILayout.Width(100));
        Color colorTriangle = EditorGUILayout.ColorField(_colorTriangle);
        EditorGUILayout.EndHorizontal();
        if (_colorTriangle != colorTriangle) {
            _colorTriangle = colorTriangle;
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

        // Size
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

        // Points
        DrawCommonTitle("Point Options");

        // Points - point 1
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Point 1", GUILayout.Width(100));
        int x = EditorGUILayout.IntSlider((int) _point1Pos.x, 0, _selectedLength, GUILayout.Width(200));
        int y = EditorGUILayout.IntSlider((int) _point1Pos.y, 0, _selectedLength, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        x = Mathf.Clamp(x, 0, _selectedLength);
        y = Mathf.Clamp(y, 0, _selectedLength);
        if (x != _point1Pos.x || y != _point1Pos.y) {
            _point1Pos = new Vector2(x, y);
            _optionChanged = true;
        }

        // Points - point 2
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Point 2", GUILayout.Width(100));
        x = EditorGUILayout.IntSlider((int) _point2Pos.x, 0, _selectedLength, GUILayout.Width(200));
        y = EditorGUILayout.IntSlider((int) _point2Pos.y, 0, _selectedLength, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        x = Mathf.Clamp(x, 0, _selectedLength);
        y = Mathf.Clamp(y, 0, _selectedLength);
        if (x != _point2Pos.x || y != _point2Pos.y) {
            _point2Pos = new Vector2(x, y);
            _optionChanged = true;
        }

        // Points - point 3
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Point 3", GUILayout.Width(100));
        x = EditorGUILayout.IntSlider((int) _point3Pos.x, 0, _selectedLength, GUILayout.Width(200));
        y = EditorGUILayout.IntSlider((int) _point3Pos.y, 0, _selectedLength, GUILayout.Width(200));
        //x = EditorGUILayout.DelayedIntField((int) _point3Pos.x, GUILayout.Width(50));
        //y = EditorGUILayout.DelayedIntField((int) _point3Pos.y, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        x = Mathf.Clamp(x, 0, _selectedLength);
        y = Mathf.Clamp(y, 0, _selectedLength);
        if (x != _point3Pos.x || y != _point3Pos.y) {
            _point3Pos = new Vector2(x, y);
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
                    Color c = IsPointInTriangle(point, _point1Pos, _point2Pos, _point3Pos) ? _colorTriangle : _colorBG;
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
            string path = EditorUtility.SaveFilePanel("Save File", Utility.DEFAULT_OUTPUT_PATH, Utility.DEFAULT_FILE_NAME_TRIANGLE, "png");
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

    // Type A
    // https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
    private bool IsPointInTriangle(Vector2 point, Vector2 triP1, Vector2 triP2, Vector2 triP3) {
        float d1 = Sign(point, triP1, triP2);
        float d2 = Sign(point, triP2, triP3);
        float d3 = Sign(point, triP3, triP1);

        bool hasNegative = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPositive = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(hasNegative && hasPositive);
    }

    private float Sign(Vector2 p1, Vector2 p2, Vector2 p3) {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    // Type B
    // https://www.gamedev.net/forums/topic.asp?topic_id=295943
    // But this method not works
    //
    //private bool IsPointInTriangle(Vector2 point, Vector2 triP1, Vector2 triP2, Vector2 triP3) {
    //    float totalArea = TriangleArea(triP1, triP2, triP3);
    //    float area1 = TriangleArea(point, triP2, triP3);
    //    float area2 = TriangleArea(point, triP1, triP3);
    //    float area3 = TriangleArea(point, triP1, triP2);

    //    if ((area1 + area2 + area3) > totalArea) {
    //        return false;
    //    }

    //    return true;
    //}

    //private float TriangleArea(Vector2 p1, Vector2 p2, Vector2 p3) {
    //    float area = 0.0f;
    //    area = (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    //    return area / 2.0f;
    //}
    #endregion
}
