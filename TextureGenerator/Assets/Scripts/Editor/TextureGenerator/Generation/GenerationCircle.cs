﻿using System.IO;
using UnityEngine;
using UnityEditor;

public class GenerationCircle : TextureGeneratorWindow<GenerationCircle> {
    [MenuItem("TextureGenerator/Generation/Circle")]
    public static void OpenWindow() {
        GenerationCircle window = GetWindow<GenerationCircle>();
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

    // Center point, radius
    private Vector2 _centerPos = Vector2.zero;
    private int _radius = 0;
    #endregion

    #region Editor Window Hooks
    protected override void OnGUIContent() {
        // Init
        if (_selectedLength == -1) {
            _selectedLength = _defaultLength;
            _optionChanged = true;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Color
        DrawCommonTitle("Select Color");

        // Color - color 1
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Color 1", GUILayout.Width(70));
        Color color1 = EditorGUILayout.ColorField(_color1);
        EditorGUILayout.EndHorizontal();
        if (_color1 != color1) {
            _color1 = color1;
            _optionChanged = true;
        }

        // Color - color 2
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Color 2", GUILayout.Width(70));
        Color color2 = EditorGUILayout.ColorField(_color2);
        EditorGUILayout.EndHorizontal();
        if (_color2 != color2) {
            _color2 = color2;
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
        int x = EditorGUILayout.IntSlider((int) _centerPos.x, 0, _selectedLength, GUILayout.Width(160));
        int y = EditorGUILayout.IntSlider((int) _centerPos.y, 0, _selectedLength, GUILayout.Width(160));
        EditorGUILayout.EndHorizontal();
        x = Mathf.Clamp(x, 0, _selectedLength);
        y = Mathf.Clamp(y, 0, _selectedLength);
        if (x != _centerPos.x || y != _centerPos.y) {
            _centerPos = new Vector2(x, y);
            _optionChanged = true;
        }

        // Circle options - radius
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Radius", GUILayout.Width(100));
        int radius = EditorGUILayout.IntSlider((int) _radius, 0, _selectedLength, GUILayout.Width(160));
        EditorGUILayout.EndHorizontal();
        radius = Mathf.Clamp(radius, 0, _selectedLength);
        if (radius != _radius) {
            _radius = radius;
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
                    Color c = IsPointInCircle(point, _centerPos, _radius) ? _color1 : _color2;
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
            byte[] bytes = _previewTexture.EncodeToPNG();
            File.WriteAllBytes(string.Format("{0}/{1}", Utility.OUTPUT_PATH_ROOT, "NewCircle.png"), bytes);
            AssetDatabase.Refresh();
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
