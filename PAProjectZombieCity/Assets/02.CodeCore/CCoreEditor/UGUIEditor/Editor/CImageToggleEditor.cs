using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CImageToggle), true)]
[CanEditMultipleObjects]
public class CImageToggleEditor : ImageEditor
{
	private SerializedProperty m_pSerializeSpriteOff;
	private SerializedProperty m_pSerializeColorOff;
	//--------------------------------------------------------
	protected override void OnEnable()
	{
		base.OnEnable();
		m_pSerializeSpriteOff = serializedObject.FindProperty("SpriteOff");
		m_pSerializeColorOff = serializedObject.FindProperty("ColorOff");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		serializedObject.Update();
		EditorGUILayout.LabelField("[ImageToggle]");
		EditorGUILayout.PropertyField(m_pSerializeSpriteOff);
		EditorGUILayout.PropertyField(m_pSerializeColorOff);
		serializedObject.ApplyModifiedProperties();
	}
}