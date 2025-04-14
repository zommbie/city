using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 상호 배타적인 페이지를 온 오프 할 수 있는 기능 제공 

public abstract  class CUIWidgetSinglePageBase : CUIWidgetBase
{
	[SerializeField]
	private List<CUIWidgetSinglePageItemBase> PageInstance = null;
	[SerializeField]
	private bool FirstItemShow = true;

	//-----------------------------------------------------------------
	protected override void OnUIEntryInitializePost(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitializePost(pParentFrame);
		for (int i = 0; i < PageInstance.Count; i++)
		{
			if (FirstItemShow == true && i == 0)
			{
				PageInstance[i].DoUIWidgetShowHide(true);
			}
			else
			{
				PageInstance[i].DoUIWidgetShowHide(false);
			}
		}
	}

	//-------------------------------------------------------------------
	protected void ProtSinglePageShow(int iIndex)
	{

	}

	//-------------------------------------------------------------------
	private CUIWidgetSinglePageItemBase FindSinglePage(int iIndex)
	{
		CUIWidgetSinglePageItemBase pFindItem = null;

		return pFindItem;
	}

}
