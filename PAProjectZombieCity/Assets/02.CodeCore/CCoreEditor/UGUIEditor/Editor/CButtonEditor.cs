using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CButton), true)]
[CanEditMultipleObjects]
public class CButtonEditor : ButtonEditor
{
    SerializedProperty m_ButtonSound;
    //-----------------------------------------
    protected override void OnEnable()
    {
        base.OnEnable();
        m_ButtonSound = serializedObject.FindProperty("ButtonSound");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.LabelField("[Custom]");
        EditorGUILayout.PropertyField(m_ButtonSound);
        serializedObject.ApplyModifiedProperties();
    }
}
