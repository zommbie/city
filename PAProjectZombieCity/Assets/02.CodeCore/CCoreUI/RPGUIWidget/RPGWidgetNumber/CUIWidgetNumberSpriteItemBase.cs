using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 숫자 스프라이트의 각 자리를 구성하는 컴포넌트

[RequireComponent(typeof(CImage))]
public abstract class CUIWidgetNumberSpriteItemBase : CUIWidgetBase
{
	private CImage m_pNumberImage = null;
	//--------------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitialize(pParentFrame);
		m_pNumberImage = GetComponent<CImage>();
	}

	protected override void OnUIEntryInitializePost(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitializePost(pParentFrame);
	}

	//--------------------------------------------------------------------
	public void DoNumberSpriteShow(Sprite pNumberSprite)
	{
		m_pNumberImage.sprite = pNumberSprite;
		DoUIWidgetShowHide(true);
	}
}
