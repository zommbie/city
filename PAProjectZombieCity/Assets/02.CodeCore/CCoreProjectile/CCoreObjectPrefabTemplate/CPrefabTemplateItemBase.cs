using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CPrefabTemplateItemBase : CMonoBase
{
	private CPrefabTemplateBase m_pTemplateOwner = null;
	private bool m_bItemActivate = false;  public bool IsTemplateItemActivated { get { return m_bItemActivate; } }
	//--------------------------------------------------------
	internal void InterPrefabTemplateEnable()
	{
        PrivPrefabTemplateItemEnable(true);      
	}

	internal void InterPrefabTemplateItemAllocated(CPrefabTemplateBase pTemplateOwner)
	{
        m_pTemplateOwner = pTemplateOwner;
        SetMonoActive(false);
		OnPrefabTemplateAllocated();
	}

	//---------------------------------------------------------
	protected void ProtPrefabTemplateReturn()
	{	
		if (m_bItemActivate == true)
		{
            PrivPrefabTemplateItemEnable(false);
            m_pTemplateOwner.InterPrefabTemplateItemReturn(this);
		}
	}

    //-----------------------------------------------------------
    private void PrivPrefabTemplateItemEnable(bool bEnable)
    {
        m_bItemActivate = bEnable;
        SetMonoActive(bEnable);
        OnPrefabTemplateEnable(bEnable);
    }

	//--------------------------------------------------------
	protected virtual void OnPrefabTemplateEnable(bool bEnable) { }
	protected virtual void OnPrefabTemplateAllocated() { }
}
