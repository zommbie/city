using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 숫자를 스프라이트 이미지로 변환해서 보여주는 기능
public abstract class CUIWidgetNumberSpriteBase : CUIWidgetNumberBase
{
	[System.Serializable]
	public class SNumberSprite
	{
		public Sprite NumberImage;
	}

	[SerializeField]
	private List<SNumberSprite> NumberList;

	[SerializeField]
	private List<CUIWidgetNumberSpriteItemBase> NumberPrefabList;

	//-----------------------------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitialize(pParentFrame);
	}

	protected override sealed void OnUIWidgetNumber(long iTargetNumber, bool bForce, List<int> pListDigit)
	{
		PrivNumberSpriteDisable();
		PrivNumberSpriteEnable(pListDigit);
		OnUIWidgetNumberSpriteAnimationStart(bForce, pListDigit);
	}
	//-----------------------------------------------------------------------------
	private void PrivNumberSpriteEnable(List<int> pListDigit)
	{
		for (int i = 0; i < pListDigit.Count; i++)
		{
			PrivNumberSpriteShow(i, pListDigit[i]);
		}
	}

	private void PrivNumberSpriteDisable()
	{
		for (int i = 0; i < NumberPrefabList.Count; i++)
		{
			NumberPrefabList[i].DoUIWidgetShowHide(false);
		}
	}

	private void PrivNumberSpriteShow(int iDigit, int iNumber)
	{
		CUIWidgetNumberSpriteItemBase pSpriteItem = FindNumberSpriteItem(iDigit);
		if (pSpriteItem != null)
		{
			SNumberSprite pNumberSprite = SearchNumberSpriteImage(iNumber);
			if (pNumberSprite != null)
			{
				pSpriteItem.DoNumberSpriteShow(pNumberSprite.NumberImage);
			}
		}
	}

	private CUIWidgetNumberSpriteItemBase FindNumberSpriteItem(int iDigit)
	{
		CUIWidgetNumberSpriteItemBase pSpriteItem = null;
		if (iDigit >= 0 && iDigit < NumberPrefabList.Count)
		{
			pSpriteItem = NumberPrefabList[iDigit];
		}
		return pSpriteItem;
	}

	private SNumberSprite SearchNumberSpriteImage(int iDigit)
	{
		SNumberSprite pNumberSprite = NumberList[iDigit];
		return pNumberSprite;
	}

	//-----------------------------------------------------------------------------
	protected virtual void OnUIWidgetNumberSpriteAnimationStart(bool bForce, List<int> pListDigit) { }
}
