using System.IO;
using UnityEngine;
using UnityEditor;

public class GenerationGrid : TextureGeneratorWindow<GenerationGrid> {
    [MenuItem("TextureGenerator/Generation/Grid")]
    public static void OpenWindow() {
        EditorWindow window = GetWindow<GenerationGrid>();
        window.Show();
    }

    #region Internal Fields
    private bool _optionChanged = false;
    private Texture2D _previewTexture = null;

    // Color
    private Color _color1 = Color.red;
    private Color _color2 = Color.green;

    // Length
    private int[] _lengthOptions = new int[] { 128, 256, 512, 1024 };
    private int _selectedLength = -1;
    private int _chunkLength = 32;
    #endregion

    #region Editor Window Hooks
    protected override void OnGUIContent() {
        // Init
        if (_selectedLength == -1) {
            _selectedLength = _lengthOptions[0];
            _previewTexture = null;
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
        if (EditorGUILayout.DropdownButton(new GUIContent(_selectedLength.ToString()), FocusType.Passive)) {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < _lengthOptions.Length; i++) {
                int lengthOption = _lengthOptions[i];
                menu.AddItem(new GUIContent(lengthOption.ToString()), lengthOption == _selectedLength, OnDropdownValueSelected, lengthOption);
            }
            menu.ShowAsContext();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Preview
        DrawCommonTitle("Preview");
        if (_optionChanged) {
            _optionChanged = false;

            // Set preview texture
            Texture2D tex = new Texture2D(_selectedLength, _selectedLength, TextureFormat.RGBA32, false);
            for (int w = 0; w < _selectedLength; w++) {
                for (int h = 0; h < _selectedLength; h++) {
                    int chunkValue = (w / _chunkLength) + (h / _chunkLength);
                    Color c = chunkValue % 2 == 0 ? _color1 : _color2;
                    tex.SetPixel(w, h, c);
                }
            }

            byte[] bytes = tex.EncodeToPNG();

            if (_previewTexture == null) {
                _previewTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            }
            _previewTexture.LoadImage(bytes);
        }

        if (_previewTexture != null) {
            GUILayout.Box(_previewTexture, GUILayout.Width(128), GUILayout.Height(128));
        }        

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Output
        DrawGenerationButton(() => {
            string path = EditorUtility.SaveFilePanel("Save File", Utility.DEFAULT_OUTPUT_PATH, Utility.DEFAULT_FILE_NAME_GRID, "png");
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            byte[] bytes = _previewTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
        });
    }
    #endregion

    #region Internal Methods
    private void OnDropdownValueSelected(object obj) {
        int length = (int) obj;
        if (_selectedLength != length) {
            _selectedLength = length;
            _optionChanged = true;
        }
    }
    #endregion
}
