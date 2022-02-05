using System;
using UnityEngine;
using UnityEditor;

public class TextureGeneratorWindow<T> : EditorWindow {
    #region Internal Fields
    private bool _init = false;
    private bool _initStyle = false;
    private Vector2 _scrollView = Vector2.zero;
    #endregion

    #region Protected Fields
    protected GUIStyle commonGUIStyleTitle = new GUIStyle();
    protected GUIStyle commonGUIStyleWarningMsg = new GUIStyle();
    #endregion

    #region Editor Window Hooks
    private void OnGUI() {
        // Init
        Init();
        InitStyle();

        // Content scrollable
        _scrollView = EditorGUILayout.BeginScrollView(_scrollView);
        OnGUIContent();
        EditorGUILayout.EndScrollView();
    }
    #endregion

    #region Virtual Methods
    protected virtual void OnInit() { 
        // Do nothing
    }

    protected virtual void OnGUIContent() { 
        // Do nothing
    }

    protected virtual void OnGenerationCompleted() {
        ShowNotificationWindow("Generation Completed");
        AssetDatabase.Refresh();
    }
    #endregion

    #region Common Methods
    protected void DrawCommonTitle(string title) {
        EditorGUILayout.LabelField(title, commonGUIStyleTitle);
    }

    protected void DrawCommonWarningMsg(string msg) {
        EditorGUILayout.LabelField(msg, commonGUIStyleWarningMsg);
    }

    protected void DrawGenerationButton(Action cb) {
        if (GUILayout.Button("Generate !!")) {
            if (cb != null) {
                cb();
            }
        }
    }

    protected void ShowNotificationWindow(string msg) {
        ShowNotification(new GUIContent(msg), 3.0f);
    }
    #endregion

    #region Internal Methods
    private void Init() {
        if (_init) {
            return;
        }

        _init = true;
        OnInit();
    }

    private void InitStyle() {
        if (_initStyle) {
            return;
        }
        _initStyle = true;

        commonGUIStyleTitle = new GUIStyle();
        commonGUIStyleTitle.normal.textColor = Color.white;
        commonGUIStyleTitle.fontStyle = FontStyle.Bold;

        commonGUIStyleWarningMsg = new GUIStyle();
        commonGUIStyleWarningMsg.normal.textColor = Color.red;
        commonGUIStyleWarningMsg.fontStyle = FontStyle.Bold;
    }
    #endregion
}
