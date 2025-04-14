using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CUIContentsFollow), true)]
[CanEditMultipleObjects]
public class CUIContentsFollowEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		UpdateFollowContents();
	}

	//-----------------------------------------
	private void UpdateFollowContents()
	{
		CUIContentsFollow pIntance = serializedObject.targetObject as CUIContentsFollow;
		pIntance.DoUIContentsFollowUpdate();
	}

	//----------------------------------------
	

}
