using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

// 소규모 프리팹 풀로 정적 로드된 객체를 관리 하는 기능을 가지고 있다.
public abstract class CPrefabTemplateBase : CMonoBase
{
	protected class SPrefabTemplateInfo
	{
		public int hResourceID;
		public CPrefabTemplateItemBase	    PrefabOrigin;
		public List<CPrefabTemplateItemBase> CloneInstance = new List<CPrefabTemplateItemBase>();
	}

	private Dictionary<int, SPrefabTemplateInfo> m_mapTemplateResource = new Dictionary<int, SPrefabTemplateInfo>();

	//---------------------------------------------------------------------------------
	protected void ProtPrefabTemplateAdd(int hTemplateID, CPrefabTemplateItemBase pPrefabOrigin, int iCloneCount = 0)
	{
		if (m_mapTemplateResource.ContainsKey(hTemplateID) == false)
		{
			PrivPrefabTemplateAdd(hTemplateID, pPrefabOrigin, iCloneCount);
		}
	}

	protected TEMPLATE ProtPrefabTemplateCloneInstance<TEMPLATE>(int hTemplateID, Transform pParent) where TEMPLATE : CPrefabTemplateItemBase
	{
		TEMPLATE pCloneInstance = null;
		if (m_mapTemplateResource.ContainsKey(hTemplateID))
		{
			pCloneInstance = PrivPrefabTemplateFindInstance(m_mapTemplateResource[hTemplateID]) as TEMPLATE;
			pCloneInstance.InterPrefabTemplateEnable(this);
			pCloneInstance.transform.SetParent(pParent, false);
		}
		return pCloneInstance;
	}


	//-----------------------------------------------------------------------------------
	private void PrivPrefabTemplateAdd(int hTemplateID, CPrefabTemplateItemBase pPrefabOrigin, int iCloneCount)
	{
		SPrefabTemplateInfo pTemplateInfo = new SPrefabTemplateInfo();
		pTemplateInfo.hResourceID = hTemplateID;
		pTemplateInfo.PrefabOrigin = pPrefabOrigin;
		m_mapTemplateResource[hTemplateID] = pTemplateInfo;

		for (int i = 0; i < iCloneCount; i++)
		{
			PrivPrefabTemplateMakeInstance(pTemplateInfo);
		}

		pPrefabOrigin.SetMonoActive(false);
	}

	private CPrefabTemplateItemBase PrivPrefabTemplateMakeInstance(SPrefabTemplateInfo pTemplateInfo)
	{
		CPrefabTemplateItemBase pCloneInstance = Instantiate(pTemplateInfo.PrefabOrigin, transform, false);
		pCloneInstance.SetMonoActive(false);
		pCloneInstance.InterPrefabTemplateItemAllocated();
		RemoveCloneObjectName(pCloneInstance.gameObject);
		pTemplateInfo.CloneInstance.Add(pCloneInstance);
		return pCloneInstance;
	}

	private CPrefabTemplateItemBase PrivPrefabTemplateFindInstance(SPrefabTemplateInfo pTemplateInfo)
	{
		CPrefabTemplateItemBase pFindInstance = null;

		for (int i = 0; i < pTemplateInfo.CloneInstance.Count; i++)
		{
			if (pTemplateInfo.CloneInstance[i].IsItemActivate == false)
			{
				pFindInstance = pTemplateInfo.CloneInstance[i];
				break;
			}
		}
		
		if (pFindInstance == null)
		{
			pFindInstance = PrivPrefabTemplateMakeInstance(pTemplateInfo);
		}

		return pFindInstance;
	}

}
