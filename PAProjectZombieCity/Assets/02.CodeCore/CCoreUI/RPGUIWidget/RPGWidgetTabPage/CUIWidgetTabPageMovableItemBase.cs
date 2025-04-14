using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIWidgetTabPageMovableItemBase : CUIWidgetBase
{

	private int m_iPageItemIndex = 0;   public int GetTabPageItemIndex() { return m_iPageItemIndex; }
	//-------------------------------------------------------------------
	internal void InterTabPageItemSetIndex(int iPageItemIndex)
	{
		m_iPageItemIndex = iPageItemIndex;
	}

	internal void InterTabPageItemShow()
	{
		DoUIWidgetShowHide(true);
		OnTabPageItemShow();
	}

	internal void InterTabPageItemHide()
	{
		DoUIWidgetShowHide(false);
		OnTabPageItemHide();
	}
	internal void InterTabPageItemMain()
	{
		OnTabPageItemMain();
	}

	internal void InterTabPageItemMoveStart()
	{
		OnTabPageItemMoveStart();
	}

	internal void InterTabPageItemMoveEnd()
	{
		OnTabPageItemMoveEnd();
	}

	//---------------------------------------------------------------------
	protected virtual void OnTabPageItemShow() { }
	protected virtual void OnTabPageItemHide() { }
	protected virtual void OnTabPageItemMoveStart() { }
	protected virtual void OnTabPageItemMoveEnd() { }
	protected virtual void OnTabPageItemMain() { }
}
