using System.IO;
using System;
using UnityEngine;
using UnityEditor;

public class GenerationRectangleGradient : TextureGeneratorWindow<GenerationRectangleGradient> {
    [MenuItem("TextureGenerator/Generation/Rectangle - Gradient")]
    public static void OpenWindow() {
        GenerationRectangleGradient window = GetWindow<GenerationRectangleGradient>();
        window.Show();
    }

    private enum GradientType { 
        FromCenterType1,
        FromCenterType2,
        FromCenterType3,
        FromCenterType4,
        ToCenterType1,
        ToCenterType2,
        ToCenterType3,
        ToCenterType4,
        ToUp,
        ToDown,
        ToLeft,
        ToRight,
    }

    #region Internal Fields
    private bool _optionChanged = false;
    private Texture2D _previewTexture = null;

    // Color
    private Gradient _colorGradient = new Gradient();
    private Gradient _colorGradientTemp = new Gradient();
    private Color _colorBackground = Color.green;
    private GradientType _selectedGType = GradientType.FromCenterType1;

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

        // Color - color rectangle gradient
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rectangle", GUILayout.Width(100));
        Gradient colorGradientTemp = EditorGUILayout.GradientField(_colorGradientTemp);
        EditorGUILayout.EndHorizontal();
        if (!_colorGradient.Equals(colorGradientTemp)) {
            _colorGradient.SetKeys(colorGradientTemp.colorKeys, colorGradientTemp.alphaKeys);
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

        // Color - gradient type
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Gradient type", GUILayout.Width(100));
        if (EditorGUILayout.DropdownButton(new GUIContent(_selectedGType.ToString()), FocusType.Keyboard)) {
            GenericMenu menu = new GenericMenu();
            string[] enumNames = Enum.GetNames(typeof(GradientType));
            for (int i = 0; i < enumNames.Length; i++) {
                string enumName = enumNames[i];
                menu.AddItem(new GUIContent(enumName), i == (int) _selectedGType, OnGradientTypeValueSelected, i);
            }

            menu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();

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
        int x = EditorGUILayout.IntSlider((int) _centerPos.x, 0, _selectedLength, GUILayout.Width(200));
        int y = EditorGUILayout.IntSlider((int) _centerPos.y, 0, _selectedLength, GUILayout.Width(200));
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
        int width = EditorGUILayout.IntSlider((int) _width, 0, _selectedLength, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        width = Mathf.Clamp(width, 0, _selectedLength);
        if (width != _width) {
            _width = width;
            _optionChanged = true;
        }

        // Rectangle options - height
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Height", GUILayout.Width(100));
        int height = EditorGUILayout.IntSlider((int) _height, 0, _selectedLength, GUILayout.Width(200));
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
                    Color c = IsPointInRectangle(point, _centerPos, _width, _height) ? GetGradientColor(point, _centerPos, _width, _height) : _colorBackground;
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

    private void OnGradientTypeValueSelected(object value) {
        int gType = (int) value;

        if (gType == (int) _selectedGType) {
            return;
        }

        _selectedGType = (GradientType) gType;
        _optionChanged = true;
    }

    private bool IsPointInRectangle(Vector2 point, Vector2 rectCenter, int rectWidth, int rectHeight) {
        return Mathf.Abs(point.x - rectCenter.x) <= rectWidth / 2.0f && Mathf.Abs(point.y - rectCenter.y) <= rectHeight / 2.0f;
    }

    private Color GetGradientColor(Vector2 point, Vector2 rectCenter, int rectWidth, int rectHeight) {
        float t = 0;
        if (_selectedGType == GradientType.FromCenterType1) {
            float minRectPosX = rectCenter.x - rectWidth / 2.0f;
            float maxRectPosX = rectCenter.x + rectWidth / 2.0f;
            float minDistancePointToSideX = Mathf.Min(Mathf.Abs(point.x - minRectPosX), Mathf.Abs(point.x - maxRectPosX));

            float minRectPosY = rectCenter.y - rectHeight / 2.0f;
            float maxRectPosY = rectCenter.y + rectHeight / 2.0f;
            float minDistancePointToSideY = Mathf.Min(Mathf.Abs(point.y - minRectPosY), Mathf.Abs(point.y - maxRectPosY));

            float minDistancePointToSide = Mathf.Min(minDistancePointToSideX, minDistancePointToSideY);
            float minDistanceCenterToSide = Mathf.Min(rectWidth / 2.0f, rectHeight / 2.0f);

            t = minDistanceCenterToSide != 0 ? Mathf.Clamp(minDistancePointToSide / minDistanceCenterToSide, 0, 1) : 0;
            t = 1 - t;
        }
        else if (_selectedGType == GradientType.FromCenterType2) {
            float distanceCenterToPointX = Mathf.Abs(point.x - rectCenter.x);
            float distanceRatioX = Mathf.Clamp01(distanceCenterToPointX / (rectWidth / 2.0f));

            float distanceCenterToPointY = Mathf.Abs(point.y - rectCenter.y);
            float distanceRatioY = Mathf.Clamp01(distanceCenterToPointY / (rectHeight / 2.0f));

            t = Mathf.Max(distanceRatioX, distanceRatioY);
        }
        else if (_selectedGType == GradientType.FromCenterType3) {
            float distanceCenterToPointX = Mathf.Abs(point.x - rectCenter.x);

            t = Mathf.Clamp01(distanceCenterToPointX / (rectWidth / 2.0f));
        }
        else if (_selectedGType == GradientType.FromCenterType4) {
            float distanceCenterToPointY = Mathf.Abs(point.y - rectCenter.y);

            t = Mathf.Clamp01(distanceCenterToPointY / (rectHeight / 2.0f));
        }
        else if (_selectedGType == GradientType.ToCenterType1) {
            float minRectPosX = rectCenter.x - rectWidth / 2.0f;
            float maxRectPosX = rectCenter.x + rectWidth / 2.0f;
            float minPointToSideX = Mathf.Min(Mathf.Abs(point.x - minRectPosX), Mathf.Abs(point.x - maxRectPosX));

            float minRectPosY = rectCenter.y - rectHeight / 2.0f;
            float maxRectPosY = rectCenter.y + rectHeight / 2.0f;
            float minPointToSideY = Mathf.Min(Mathf.Abs(point.y - minRectPosY), Mathf.Abs(point.y - maxRectPosY));

            float minPointToSide = Mathf.Min(minPointToSideX, minPointToSideY);
            float minCenterToSide = Mathf.Min(rectWidth / 2.0f, rectHeight / 2.0f);

            t = minCenterToSide != 0 ? Mathf.Clamp(minPointToSide / minCenterToSide, 0, 1) : 0;            
        }
        else if (_selectedGType == GradientType.ToCenterType2) {
            float distanceCenterToPointX = Mathf.Abs(point.x - rectCenter.x);
            float distanceRatioX = Mathf.Clamp01(distanceCenterToPointX / (rectWidth / 2.0f));

            float distanceCenterToPointY = Mathf.Abs(point.y - rectCenter.y);
            float distanceRatioY = Mathf.Clamp01(distanceCenterToPointY / (rectHeight / 2.0f));

            t = Mathf.Max(distanceRatioX, distanceRatioY);
            t = 1 - t;
        }
        else if (_selectedGType == GradientType.ToCenterType3) {
            float distanceCenterToPointX = Mathf.Abs(point.x - rectCenter.x);

            t = Mathf.Clamp01(distanceCenterToPointX / (rectWidth / 2.0f));
            t = 1 - t;
        }
        else if (_selectedGType == GradientType.ToCenterType4) {
            float distanceCenterToPointY = Mathf.Abs(point.y - rectCenter.y);

            t = Mathf.Clamp01(distanceCenterToPointY / (rectHeight / 2.0f));
            t = 1 - t;
        }
        else if (_selectedGType == GradientType.ToUp) {
            t = rectHeight != 0 ? (point.y - (rectCenter.y - rectHeight / 2.0f)) / rectHeight : 0;
        }
        else if (_selectedGType == GradientType.ToDown) {
            t = rectHeight != 0 ? ((rectCenter.y + rectHeight / 2.0f) - point.y) / rectHeight : 0;
        }
        else if (_selectedGType == GradientType.ToLeft) {
            t = rectWidth != 0 ? ((rectCenter.x + rectWidth / 2.0f) - point.x) / rectWidth : 0;
        }
        else if (_selectedGType == GradientType.ToRight) {
            t = rectWidth != 0 ? (point.x - (rectCenter.x - rectWidth / 2.0f)) / rectWidth : 0;
        }

        return _colorGradient.Evaluate(t);
    }
    #endregion
}
