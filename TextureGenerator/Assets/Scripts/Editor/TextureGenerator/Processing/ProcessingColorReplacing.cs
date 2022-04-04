using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProcessingColorReplacing : TextureGeneratorWindow<ProcessingColorReplacing> {
    [MenuItem("TextureGenerator/Processing/Replacing")]
    public static void OpenWindow() {
        ProcessingColorReplacing window = GetWindow<ProcessingColorReplacing>();
        window.Show();
    }

    #region Internal Fields
    private bool _targetTextureChanged = false;
    private bool _replacingOptionChanged = false;
    private bool _refreshFileReview = false;

    private Texture2D _targetTexture = null;
    private Texture2D _textureToReplace = null;
    private Texture2D _previewTexture = null;

    private List<byte[]> _texture2DByteDataList = null;

    // Replacing settings
    private Color _colorOld = Color.green;
    private Color _colorNew = Color.red;

    // Output file options
    private string _outputFolderPath = string.Empty;
    private string _outputPrefix = string.Empty;
    private string _outputWarningMsg = string.Empty;
    #endregion

    #region Editor Window Hooks
    protected override void OnInit() {
        _outputFolderPath = Utility.DEFAULT_OUTPUT_PATH;
    }

    protected override void OnGUIContent() {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Target texture selection
        DrawCommonTitle("Select Texture");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Target texture", GUILayout.Width(100));
        Texture2D tex = (Texture2D) EditorGUILayout.ObjectField(_targetTexture, typeof(Texture2D), true, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();
        if (tex != null) {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            DrawCommonTitle("Texture Info");
            EditorGUILayout.LabelField(string.Format("Width: {0} px,   Height: {1} px,   Format: {2}",
                tex.width, tex.height, tex.format.ToString()));
        }

        if (_targetTexture != tex) {
            _targetTexture = tex;
            _targetTextureChanged = true;
            _refreshFileReview = true;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Format converting
        if (_targetTextureChanged) {
            if (_targetTexture != null) {
                // TODO:
                // recheck readable and compressed
                if (!_targetTexture.isReadable) {
                    _textureToReplace = Utility.ConvertToReadableAndDecompressed(_targetTexture);
                }
            }
            else {
                _textureToReplace = null;
            }
        }

        if (_textureToReplace != null) {
            GUILayout.Box(_textureToReplace, GUILayout.Width(128), GUILayout.Height(128));
        }
        else {
            EditorGUILayout.LabelField("No content ...");
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Color replacing handlings
        if (_textureToReplace != null) {
            DrawCommonTitle("Replacing Options");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Color Old", GUILayout.Width(100));
            Color colorOld = EditorGUILayout.ColorField(_colorOld);
            EditorGUILayout.EndHorizontal();
            if (colorOld != _colorOld) {
                _colorOld = colorOld;
                _replacingOptionChanged = true;
                _refreshFileReview = true;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Color New", GUILayout.Width(100));
            Color colorNew = EditorGUILayout.ColorField(_colorNew);
            EditorGUILayout.EndHorizontal();
            if (colorNew != _colorNew) {
                _colorNew = colorNew;
                _replacingOptionChanged = true;
                _refreshFileReview = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        if (_targetTextureChanged || _replacingOptionChanged) {
            _targetTextureChanged = false;
            _replacingOptionChanged = false;

            if (_textureToReplace != null) {
                if (_previewTexture == null) {
                    _previewTexture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
                }

                int width = _textureToReplace.width;
                int height = _textureToReplace.height;
                Texture2D t = new Texture2D(width, height, TextureFormat.ARGB32, false);
                for (int w = 0; w < width; w++) {
                    for (int h = 0; h < height; h++) {
                        Color c = _textureToReplace.GetPixel(w, h);
                        if (IsSameColor(c, _colorOld)) {
                            c = _colorNew;
                        }

                        t.SetPixel(w, h, c);
                    }
                }

                byte[] bytes = t.EncodeToPNG();
                _previewTexture.LoadImage(bytes);
            }
            else {
                _previewTexture = null;
            }
        }

        // Preview
        if (_previewTexture != null) {
            DrawCommonTitle("Preview");

            GUILayout.Box(_previewTexture, GUILayout.Width(128), GUILayout.Height(128));
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //// Output
        if (_previewTexture != null) {
            DrawGenerationButton(() => {
                string path = EditorUtility.SaveFilePanel("Save File", Utility.DEFAULT_OUTPUT_PATH, Utility.DEFAULT_FILE_NAME_COLOR_REPLACING, "png");
                if (string.IsNullOrEmpty(path)) {
                    return;
                }

                byte[] bytes = _previewTexture.EncodeToPNG();
                File.WriteAllBytes(path, bytes);

                OnGenerationCompleted();
            });
        }
    }
    #endregion

    #region Internal Methods
    private bool IsSameColor(Color colorA, Color colorB) {
        return (colorA.r == colorB.r) && (colorA.g == colorB.g) && (colorA.b == colorB.b) && (colorA.a == colorB.a);
    }

    private bool ConvertToProjectFolderPath(string oriFolderPath, out string folderPath) {
        folderPath = string.Empty;
        if (oriFolderPath.IndexOf(Application.dataPath) == 0) {
            folderPath = oriFolderPath.Replace(Application.dataPath, "Assets");
            return true;
        }

        if (oriFolderPath.IndexOf("Assets") == 0) {
            folderPath = oriFolderPath;
            return true;
        }

        return false;
    }
    #endregion
}
