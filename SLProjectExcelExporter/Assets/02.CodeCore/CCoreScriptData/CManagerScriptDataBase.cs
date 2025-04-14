using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CManagerScriptDataBase : CManagerTemplateBase<CManagerScriptDataBase>
{
	private Dictionary<string, CScriptDataBase> m_mapScriptData = new Dictionary<string, CScriptDataBase>();

	//-----------------------------------------------------------------
	protected override void OnUnityAwake()
	{
		base.OnUnityAwake();
		PrivScriptDataInitilize();
	}

	//-----------------------------------------------------------------
	protected CScriptDataBase FindMgrScriptData(string strClassName)
	{
		CScriptDataBase pFindData = null;
		m_mapScriptData.TryGetValue(strClassName, out pFindData);
		return pFindData;
	}

	//------------------------------------------------------------------
	private void PrivScriptDataInitilize()
	{
		List<CScriptDataBase> listScriptData = new List<CScriptDataBase>();
		GetComponentsInChildren(true, listScriptData);

		for (int i = 0; i < listScriptData.Count; i++)
		{
			m_mapScriptData[listScriptData[i].GetType().Name] = listScriptData[i];
			listScriptData[i].InterScriptDataInitialize();			
		}
		OnScriptDataInitialize();
		GlobalManagerScriptDataLoaded();
	}
	
	//-------------------------------------------------------------------
	protected virtual void OnScriptDataInitialize() { }
}
