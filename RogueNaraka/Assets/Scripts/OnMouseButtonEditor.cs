using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(OnMouseButton), true)]
public class OnMouseButtonEditor : Editor
{

    SerializedProperty _onDownProperty;
    SerializedProperty _onUpProperty;
    SerializedProperty _onEnterProperty;
    SerializedProperty _onExitProperty;

    public override void OnInspectorGUI()
    {
        _onDownProperty = serializedObject.FindProperty("_onDown");
        _onUpProperty = serializedObject.FindProperty("_onUp");
        _onEnterProperty = serializedObject.FindProperty("_onEnter");
        _onExitProperty = serializedObject.FindProperty("_onExit");

        base.OnInspectorGUI();
        EditorGUILayout.Space();

        serializedObject.Update();
        //EditorGUILayout.PropertyField(_onDownProperty);
        //EditorGUILayout.PropertyField(_onUpProperty);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
