using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ���ο��� �����ϴ� �۵� ������ ���¿� ���� �پ��� ����� �����ش�.
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
