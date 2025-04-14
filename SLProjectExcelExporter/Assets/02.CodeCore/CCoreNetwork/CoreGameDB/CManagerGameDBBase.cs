using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CManagerGameDBBase : CManagerTemplateBase<CManagerGameDBBase>
{
	private Dictionary<string, CGameDBSheetBase> m_mapGameDBCategory = new Dictionary<string, CGameDBSheetBase>();
	//---------------------------------------------------------
	protected void ProtMgrGameDBSheetAdd(CGameDBSheetBase pSheet)
	{
		string strClassName = pSheet.GetType().Name;

		if (m_mapGameDBCategory.ContainsKey(strClassName))
		{
			Debug.LogWarningFormat("[GameDB] Duplicate SheetID : {0}", strClassName);
		}
		else
		{
			m_mapGameDBCategory[strClassName] = pSheet;
		}
	}

	protected TEMPLATE FindMgrGameDBSheet<TEMPLATE>() where TEMPLATE : CGameDBSheetBase
	{
		string strClassName = typeof(TEMPLATE).Name;
		TEMPLATE pFindSheet = null;
		if (m_mapGameDBCategory.ContainsKey(strClassName))
		{
			pFindSheet = m_mapGameDBCategory[strClassName] as TEMPLATE;
		}
		return pFindSheet;
	}

 

}
