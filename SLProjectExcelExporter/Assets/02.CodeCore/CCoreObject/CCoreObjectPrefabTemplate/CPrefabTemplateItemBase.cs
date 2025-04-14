using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CPrefabTemplateItemBase : CMonoBase
{
	private CPrefabTemplateBase m_pTemplateOwner = null;
	private bool m_bItemActivate = false;  public bool IsItemActivate { get { return m_bItemActivate; } }
	//--------------------------------------------------------
	internal void InterPrefabTemplateEnable(CPrefabTemplateBase pTemplateOwner)
	{
		m_pTemplateOwner = pTemplateOwner;
		m_bItemActivate = true;
		SetMonoActive(true);
		OnPrefabTemplateEnable(true);
	}

	internal void InterPrefabTemplateItemAllocated()
	{
		OnPrefabTemplateAllocated();
	}

	//---------------------------------------------------------
	protected void ProtPrefabTemplateReturn()
	{
		if (m_pTemplateOwner == null) return;

		if (m_bItemActivate == true)
		{
			m_bItemActivate = false;
			SetMonoActive(false);
			transform.SetParent(m_pTemplateOwner.transform, false);
			OnPrefabTemplateEnable(false);
		}
	}

	//--------------------------------------------------------
	protected virtual void OnPrefabTemplateEnable(bool bEnable) { }
	protected virtual void OnPrefabTemplateAllocated() { }
}
