using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CButtonPointDown), true)]
[CanEditMultipleObjects]
public class CButtonPointDownEditor : ButtonEditor
{
    SerializedProperty m_OnPropertyPointerDown;

	protected override void OnEnable()
	{
		base.OnEnable();
		m_OnPropertyPointerDown = serializedObject.FindProperty("PointDown");
		
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorGUILayout.Space();

		serializedObject.Update();
		EditorGUILayout.PropertyField(m_OnPropertyPointerDown);
		serializedObject.ApplyModifiedProperties();
	}
}
