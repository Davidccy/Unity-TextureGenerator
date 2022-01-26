using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProcessingSplit : TextureGeneratorWindow<ProcessingSplit> {
    [MenuItem("TextureGenerator/Processing/Split")]
    public static void OpenWindow() {
        ProcessingSplit window = GetWindow<ProcessingSplit>();
        window.Show();
    }

    #region Internal Fields
    private bool _targetTextureChanged = false;
    private bool _splitOptionChanged = false;

    private Texture2D _targetTexture = null;
    private Texture2D _textureToSplit = null;

    private List<byte[]> _texture2DByteDataList = null;

    // Split options
    private int _splitCountX = 0;
    private int _splitCountY = 0;

    // Output order options
    private bool _orderReverseX = false;
    private bool _orderReverseY = false;
    #endregion

    #region Editor Window Hooks
    protected override void OnGUIContent() {
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
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Format converting
        if (_targetTextureChanged) {
            if (_targetTexture != null) {
                // TODO:
                // rechcheck readable and compressed
                if (!_targetTexture.isReadable) {
                    _textureToSplit = Utility.ConvertToReadableAndDecompressed(_targetTexture);
                }
            }
            else {
                _textureToSplit = null;
            }
        }

        if (_textureToSplit != null) {
            GUILayout.Box(_textureToSplit, GUILayout.Width(128), GUILayout.Height(128));
        }
        else {
            EditorGUILayout.LabelField("No content ...");
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Split handlings
        //
        // NOTE:
        //
        // Split 100 pixels into 4 parts => 25 px, 25 px, 25 px, 25 px
        // Split 103 pixels into 4 parts => 26 px, 26 px, 26 px, 25 px
        //                                  +1     +1     +1     +0
        // Fill the remain pixels to front part
        //
        // If (width, height) = (103, 157),
        // and will be splitted into 3 * 4 = 12 pieces (sub textures), then it would be like ...
        //
        //      ┌───┬───┬───┐
        //  39  │  P9  │  P10 │  P11 │
        //      ├───┼───┼───┤
        //  39  │  P6  │  P7  │  P8  │
        //      ├───┼───┼───┤
        //  39  │  P3  │  P4  │  P5  │
        //      ├───┼───┼───┤
        //  40  │  P0  │  P1  │  P2  │
        //      └───┴───┴───┘
        //          35      34      34
        //
        // First piece (sub texture) start from left down 
        // since function 'Texture2D.GetPixel()' starts the pixel index from left down
        //

        if (_textureToSplit != null) {
            DrawCommonTitle("Split Options");
            int splitCountX = EditorGUILayout.IntSlider(_splitCountX, 1, 12, GUILayout.Width(200));
            if (splitCountX != _splitCountX) {
                _splitCountX = splitCountX;
                _splitOptionChanged = true;
            }
            int splitCountY = EditorGUILayout.IntSlider(_splitCountY, 1, 12, GUILayout.Width(200));
            if (splitCountY != _splitCountY) {
                _splitCountY = splitCountY;
                _splitOptionChanged = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        if (_targetTextureChanged || _splitOptionChanged) {
            _targetTextureChanged = false;
            _splitOptionChanged = false;

            if (_textureToSplit != null) {
                _texture2DByteDataList = new List<byte[]>();

                int avgPixelX = _textureToSplit.width / _splitCountX;
                int avgPixelY = _textureToSplit.height / _splitCountY;
                int remainedPixelX = _textureToSplit.width % _splitCountX;
                int remainedPixelY = _textureToSplit.height % _splitCountY;

                for (int j = 0; j < _splitCountY; j++) {
                    for (int i = 0; i < _splitCountX; i++) {
                        int startPixelX = i * avgPixelX + Mathf.Min(i, remainedPixelX);
                        int startPixelY = j * avgPixelY + Mathf.Min(j, remainedPixelY);
                        int width = avgPixelX + (remainedPixelX > i ? 1 : 0);
                        int height = avgPixelY + (remainedPixelY > j ? 1 : 0);

                        Texture2D t = new Texture2D(width, height, TextureFormat.ARGB32, false);
                        for (int w = 0; w < width; w++) {
                            for (int h = 0; h < height; h++) {
                                Color c = _textureToSplit.GetPixel(startPixelX + w, startPixelY + h);
                                t.SetPixel(w, h, c);
                            }
                        }
                        _texture2DByteDataList.Add(t.EncodeToPNG());
                    }
                }
            }
            else {
                _texture2DByteDataList = null;
            }
        }

        // Output order settings
        //
        // NOTE:
        // Order reverse options
        //
        //                    0 1 2            2 1 0
        // Y reverse on       3 4 5            5 4 3
        //                    6 7 8            6 7 8
        //
        //                    6 7 8            6 7 8
        // Y reverse off      3 4 5            5 4 3  
        //                    0 1 2            2 1 0
        //
        //                 X reverse off    X reverse on
        //
        // 
        // Default: X reverse off, Y reverse off
        //

        if (_textureToSplit != null && _texture2DByteDataList != null) {
            DrawCommonTitle("Output Order Options");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("X order reverse", GUILayout.Width(100));
            bool orderReverseX = EditorGUILayout.Toggle(_orderReverseX);
            if (orderReverseX != _orderReverseX) {
                _orderReverseX = orderReverseX;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Y order reverse", GUILayout.Width(100));
            bool orderReverseY = EditorGUILayout.Toggle(_orderReverseY);
            if (orderReverseY != _orderReverseY) {
                _orderReverseY = orderReverseY;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        // Preview
        if (_texture2DByteDataList != null) {
            DrawCommonTitle("Preview");

            Texture2D ttPreview = new Texture2D(0, 0, TextureFormat.ARGB32, false);

            for (int y = _splitCountY - 1; y >= 0; y--) {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < _splitCountX; x++) {
                    int subTextureIndex = y * _splitCountX + x;

                    int columnIndex = _orderReverseX ? _splitCountX - 1 - x : x;
                    int rowIndex = _orderReverseY ? _splitCountX - 1 - y : y;
                    int outputIndex = rowIndex * _splitCountX + columnIndex;

                    EditorGUILayout.LabelField(string.Format("No.{0:00}", outputIndex), GUILayout.Width(40));
                    ttPreview.LoadImage(_texture2DByteDataList[subTextureIndex]);
                    GUILayout.Box(ttPreview, GUILayout.Width(ttPreview.width), GUILayout.Height(ttPreview.height));
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Output
        DrawGenerationButton(() => {
            for (int y = _splitCountY - 1; y >= 0; y--) {
                for (int x = 0; x < _splitCountX; x++) {
                    int subTextureIndex = y * _splitCountX + x;

                    int columnIndex = _orderReverseX ? _splitCountX - 1 - x : x;
                    int rowIndex = _orderReverseY ? _splitCountX - 1 - y : y;
                    int outputIndex = rowIndex * _splitCountX + columnIndex;

                    string path = string.Format("{0}/Split{1}.jpg", Utility.OUTPUT_PATH_ROOT, outputIndex);
                    File.WriteAllBytes(path, _texture2DByteDataList[subTextureIndex]);
                }
            }
            AssetDatabase.Refresh();
        });
    }
    #endregion
}
