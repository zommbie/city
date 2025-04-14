using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 프레임 내부에서 동작하는 작동 단위로 형태에 따라 다양한 모습을 보여준다.
public abstract class CUIWidgetWindowBase : CUIWidgetBase
{
	public enum EWindowType
    {
		None,
		ToolTip,
		CloseChain,
    }
	[SerializeField]
	private EWindowType WindowType = EWindowType.None;

	protected int m_iCurrentChainOrder = 0;

	protected override void OnUIWidgetShowHide(bool bShow)
	{
		base.OnUIWidgetShowHide(bShow);

		if (WindowType == EWindowType.ToolTip)
        {
			if (bShow)
            {
			//	GetUIWidgetParent().InternalToolTipShow(this);
			}
        }
		else if (WindowType == EWindowType.CloseChain)
        {
		//	m_iCurrentChainOrder = GetUIWidgetParent().InternalWindowShowHide(this, bShow);
		}
	}
}
