using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIWidgetTemplateItemBase : CUIWidgetBase
{
	private bool m_bItemActivate = false;     public bool IsItemActivate() { return m_bItemActivate; }
	private int m_iItemIndex = 0;
	private CUIWidgetTemplateBase m_pParent = null;

    //-----------------------------------------------------------
    protected override void OnUIEntryOwner(CUIEntryBase pWidgetOwner)
    {
        base.OnUIEntryOwner(pWidgetOwner);
		m_pParent = pWidgetOwner as CUIWidgetTemplateBase;
    }
    //------------------------------------------------------------
    public void DoUITemplateItemShow(bool bShow)
    {
		DoUIWidgetShowHide(bShow);
        OnUIWidgetTemplateItemShow(bShow);
    }

    public void DoUITemplateItemReturn() // 비용이 상당하므로 사용시 주의
	{
		m_pParent.DoUITemplateReturn(this);		
	}

	public void DoUITemplateItemRefreshIndex(int iIndex)
	{
		m_iItemIndex = iIndex;
		OnUIWidgetTemplateItemRefresh(iIndex);
	}

	public int GetUITemplateItemIndex()
	{
		return m_iItemIndex;
	}

	//----------------------------------------------------------
	internal void InterUITemplateItemReturn()
	{
		m_bItemActivate = false;
		OnUIWidgetTemplateItemReturn();
	}
	internal void InterUITemplateItemActivate()
	{
		m_bItemActivate = true;
		OnUIWidgetTemplateItemActivate();
	}

	//--------------------------------------------------------
	protected virtual void OnUIWidgetTemplateItemShow(bool bShow) { }
	protected virtual void OnUIWidgetTemplateItemRefresh(int iIndex) { }
	protected virtual void OnUIWidgetTemplateItemReturn() { }
	protected virtual void OnUIWidgetTemplateItemActivate() { }
}
