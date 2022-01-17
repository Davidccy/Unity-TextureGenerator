using System.IO;
using UnityEngine;
using UnityEditor;

public class GenerationGrid : EditorWindow {
    [MenuItem("TextureGenerator/Generation")]
    public static void OpenWindow() {
        EditorWindow window = GetWindow<GenerationGrid>();
        window.titleContent = new GUIContent("Ground Texture Generator");
        window.maxSize = new Vector2(500, 500);
        window.minSize = new Vector2(200, 200);
        window.Show();
    }

    #region Internal Fields
    private Color _color1 = Color.red;
    private Color _color2 = Color.green;
    private int[] _lengthOptions = new int[] { 64, 128, 256, 512, 1024, 2048 };
    private int _selectedLength = -1;
    private int _chunkLength = 32;
    private bool _statusChanged = false;

    private GUIStyle _guiStyleTitle = null;
    private Texture2D _previewTexture = null;
    #endregion

    #region Editor Window Hooks
    public void OnGUI() {
        // Init
        if (_selectedLength == -1) {
            _selectedLength = _lengthOptions[0];
            _guiStyleTitle = new GUIStyle() { fontStyle = FontStyle.Bold };
            _previewTexture = null;
            _statusChanged = true;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Color
        EditorGUILayout.LabelField("Select Color:", _guiStyleTitle);
        Color color1 = EditorGUILayout.ColorField("Color 1", _color1);
        if (_color1 != color1) {
            _color1 = color1;
            _statusChanged = true;
        }

        Color color2 = EditorGUILayout.ColorField("Color 2", _color2);
        if (_color2 != color2) {
            _color2 = color2;
            _statusChanged = true;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Length
        EditorGUILayout.LabelField("Select Texture Length:", _guiStyleTitle);
        EditorGUILayout.LabelField(string.Format("Length: {0}", _selectedLength));
        if (EditorGUILayout.DropdownButton(new GUIContent("Choose texture length"), FocusType.Passive)) {
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
        EditorGUILayout.LabelField("Preview", _guiStyleTitle);
        if (_statusChanged) {
            _statusChanged = false;

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
            GUILayout.Box(_previewTexture, GUILayout.Width(200), GUILayout.Height(200));
        }        

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Output
        if (GUILayout.Button("Generate !!")) {
            byte[] bytes = _previewTexture.EncodeToPNG();
            File.WriteAllBytes(string.Format("{0}/{1}", Utility.OUTPUT_PATH_ROOT, "NewGrid.png"), bytes);
            AssetDatabase.Refresh();
        }
    }
    #endregion

    #region Internal Methods
    private void OnDropdownValueSelected(object obj) {
        int length = (int) obj;
        if (_selectedLength != length) {
            _selectedLength = length;
            _statusChanged = true;
        }
    }
    #endregion
}
