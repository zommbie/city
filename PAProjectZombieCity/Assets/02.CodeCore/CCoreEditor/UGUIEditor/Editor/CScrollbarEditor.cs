using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CScrollBar), true)]
[CanEditMultipleObjects]
public class CScrollbarEditor : ScrollbarEditor
{
    SerializedProperty m_fFixedSize;
    //-----------------------------------------
    protected override void OnEnable()
    {
        base.OnEnable();
        m_fFixedSize = serializedObject.FindProperty("FixedSize");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.LabelField("[Custom]");
        EditorGUILayout.PropertyField(m_fFixedSize);
        serializedObject.ApplyModifiedProperties();
    }
}
