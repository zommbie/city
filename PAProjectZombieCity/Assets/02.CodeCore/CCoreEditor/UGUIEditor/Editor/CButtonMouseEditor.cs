using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CButtonMouse), true)]
[CanEditMultipleObjects]
public class CButtonMouseEditor : ButtonEditor
{
    SerializedProperty m_OnPropertyPointDownLeft;
    SerializedProperty m_OnPropertyPointDownRight;

    SerializedProperty m_OnPropertyPointerUpLeft;
    SerializedProperty m_OnPropertyPointerUpRight;

    SerializedProperty m_OnPropertyButtonDragStart;
    SerializedProperty m_OnPropertyButtonDragging;
    SerializedProperty m_OnPropertyButtonDragDrop;

    SerializedProperty m_OnPropertyMouseMove;

    SerializedProperty m_OnPropertyMouseEnter;
    SerializedProperty m_OnPropertyMouseExit;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_OnPropertyPointDownLeft = serializedObject.FindProperty("m_OnPointDownLeft");
        m_OnPropertyPointDownRight = serializedObject.FindProperty("m_OnPointDownRight");

        m_OnPropertyPointerUpLeft = serializedObject.FindProperty("m_OnPointerUpLeft");
        m_OnPropertyPointerUpRight = serializedObject.FindProperty("m_OnPointerUpRight");

        m_OnPropertyButtonDragStart = serializedObject.FindProperty("m_OnButtonDragStart");
        m_OnPropertyButtonDragging = serializedObject.FindProperty("m_OnButtonDragging");
        m_OnPropertyButtonDragDrop = serializedObject.FindProperty("m_OnButtonDragDrop");

        m_OnPropertyMouseMove = serializedObject.FindProperty("m_OnMouseMove");

        m_OnPropertyMouseEnter = serializedObject.FindProperty("m_OnMouseEnter");
        m_OnPropertyMouseExit = serializedObject.FindProperty("m_OnMouseExit");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_OnPropertyPointDownLeft);
        EditorGUILayout.PropertyField(m_OnPropertyPointDownRight);

        EditorGUILayout.PropertyField(m_OnPropertyPointerUpLeft);
        EditorGUILayout.PropertyField(m_OnPropertyPointerUpRight);

        EditorGUILayout.PropertyField(m_OnPropertyButtonDragStart);
        EditorGUILayout.PropertyField(m_OnPropertyButtonDragging);
        EditorGUILayout.PropertyField(m_OnPropertyButtonDragDrop);

        EditorGUILayout.PropertyField(m_OnPropertyMouseMove);

        EditorGUILayout.PropertyField(m_OnPropertyMouseEnter);
        EditorGUILayout.PropertyField(m_OnPropertyMouseExit);

        serializedObject.ApplyModifiedProperties();
    }
}
