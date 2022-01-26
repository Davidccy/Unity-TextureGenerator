using System;
using UnityEngine;
using UnityEditor;

public class TextureGeneratorWindow<T> : EditorWindow {
    #region Internal Fields
    private bool _initStyle = false;
    private Vector2 _scrollView = Vector2.zero;
    private GUIStyle _guiStyleTitle = new GUIStyle();
    #endregion

    #region Editor Window Hooks
    private void OnGUI() {
        // Init
        InitStyle();

        // Content scrollable
        _scrollView = EditorGUILayout.BeginScrollView(_scrollView);
        OnGUIContent();
        EditorGUILayout.EndScrollView();
    }
    #endregion

    #region Virtual Methods
    protected virtual void OnGUIContent() { 
        // Do nothing
    }
    #endregion

    #region Common Methods
    protected void DrawCommonTitle(string title) {
        EditorGUILayout.LabelField(title, _guiStyleTitle);
    }

    protected void DrawGenerationButton(Action cb) {
        if (GUILayout.Button("Generate !!")) {
            if (cb != null) {
                cb();
            }
        }
    }
    #endregion

    #region Internal Methods
    private void InitStyle() {
        if (_initStyle) {
            return;
        }
        _initStyle = true;

        _guiStyleTitle = new GUIStyle();
        _guiStyleTitle.normal.textColor = Color.white;
        _guiStyleTitle.fontStyle = FontStyle.Bold;
    }
    #endregion
}
