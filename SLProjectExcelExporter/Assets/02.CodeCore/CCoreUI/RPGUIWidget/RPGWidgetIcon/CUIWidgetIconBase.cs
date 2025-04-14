using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// 모든 객체는 SetActive를 하지 않는다. Graphic.enable을 사용
public abstract class CUIWidgetIconBase : CUIWidgetBase
{	
	

	[System.Serializable]
	private class SIconGradeInfo
	{
		public int GradeNumber = 0;
		public List<CUIWidgetIconPart> GradePartList = new List<CUIWidgetIconPart>();
	}

	[SerializeField]
	private bool				 DragAndDrop = false;		public bool IsDragAndDrop { get { return DragAndDrop; } } // 체크 되면 CUIWidgetIconDragBase를 생성한다.

	[SerializeField]
	private CImageClickable		 IconBody = null;

	[SerializeField]
	private Text				 TextCount = null;


	[SerializeField]
	private List<SIconGradeInfo> GradeList = null;


	private uint m_hIconID = 0;   public uint GetIconID() { return m_hIconID; }
	private Dictionary<int, CUIWidgetIconPart> m_mapIconParts = new Dictionary<int, CUIWidgetIconPart>();
	//-------------------------------------------------------------------------
	protected override void OnUnityAwake()
	{
		base.OnUnityAwake();
	}

	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitialize(pParentFrame);
		PrivIconInitialize();
	}

	//---------------------------------------------------------------------------
	protected void ProtIconCount(int iCount, bool bForceCount = false)
	{
		if (bForceCount)
		{
			TextCount.enabled = true;
			TextCount.text = iCount.ToString();
		}
		else
		{
			if (iCount == 0 || iCount == 1)
			{
				TextCount.enabled = false;
			}
			else
			{
				TextCount.enabled = true;
				TextCount.text = iCount.ToString();
			}
		}
	}

	protected void ProtIconCountMinMax(int iMin, int iMax)
	{
		TextCount.enabled = true;
		if (iMax <= iMin)
		{
			TextCount.text = iMin.ToString();
		}
		else
		{
			TextCount.text = string.Format("{0} ~ {1}", iMin, iMax);
		}
	}

	protected void ProtIconGrade(int iGrade)
	{
		for (int i = 0; i < GradeList.Count; i++)
		{
			SIconGradeInfo pIconGradeInfo = GradeList[i];
			if (iGrade == pIconGradeInfo.GradeNumber)
			{
				for (int j = 0; j < pIconGradeInfo.GradePartList.Count; j++)
				{
					pIconGradeInfo.GradePartList[j].DoUIWidgetShowHide(true, false);
				}
			}
			else
			{
				for (int j = 0; j < pIconGradeInfo.GradePartList.Count; j++)
				{
					pIconGradeInfo.GradePartList[j].DoUIWidgetShowHide(false, false);
				}
			}
		}
	}

	protected void ProtIconSetting(uint hIconID, Sprite pIconSprite, int iGrade, int iCount, bool bForceCount = false)
	{
		ProtIconImage(hIconID, pIconSprite);
		ProtIconGrade(iGrade);
		ProtIconCount(iCount, bForceCount);
	}

	protected void ProtIconSettingMinMax(uint hIconID, Sprite pIconSprite, int iGrade, int iCountMin, int iCountMax)
	{
		ProtIconImage(hIconID, pIconSprite);
		ProtIconGrade(iGrade);
		ProtIconCountMinMax(iCountMin, iCountMax);
	}

	protected void ProtIconImage(uint hIconID, Sprite pIconSprite)
	{
		m_hIconID = hIconID;
		if (IconBody != null)
		{
			IconBody.sprite = pIconSprite;
		}
	}

	protected CUIWidgetIconPart ProtIconParts(int eIconPartsType, bool bShow)
	{
		CUIWidgetIconPart pIconParts = FindIconParts(eIconPartsType);
		if (pIconParts != null)
		{
			pIconParts.DoUIWidgetShowHide(bShow, false);
		}
		return pIconParts;
	}

	protected void ProtIconReset() // 파츠와 그레이드를 비활성화
	{
		for (int i = 0; i < GradeList.Count; i++)
		{
			for (int j = 0; j < GradeList[i].GradePartList.Count; j++)
			{
				GradeList[i].GradePartList[j].DoUIWidgetShowHide(false, false);
			}
		}

		Dictionary<int, CUIWidgetIconPart>.Enumerator it = m_mapIconParts.GetEnumerator();
		while(it.MoveNext())
		{
			it.Current.Value.DoUIWidgetShowHide(false, false);
		}
	}
	//------------------------------------------------------------------------
	protected void ProtIconPartsAdd(int eIconPartsType, CUIWidgetIconPart pPartsInstance)
	{
		m_mapIconParts[eIconPartsType] = pPartsInstance;
	}

	//-------------------------------------------------------------------------
	private void PrivIconInitialize()
	{
		if (DragAndDrop)
		{
			IconBody.SetImageInputEvent(HandleIconClick, true, HandleIconDragOn, HandleIconDragStart, HandleIconDragEnd);
		}
		else
		{
			IconBody.SetImageInputEvent(HandleIconClick);
		}
		ProtIconReset();
	}

	private CUIWidgetIconPart FindIconParts(int eIconPartsType)
	{
		CUIWidgetIconPart pFindPart = null;
		m_mapIconParts.TryGetValue(eIconPartsType, out pFindPart);
		return pFindPart;
	}

	//------------------------------------------------------------------------
	private void HandleIconClick(Vector2 vecPosition)
	{
		OnUIIconClick(vecPosition);
	}

	private void HandleIconDragStart(Vector2 vecPosition)
	{
		OnUIIconDragStart(vecPosition);
	}

	private void HandleIconDragOn(Vector2 vecPosition)
	{
		OnUIIconDragOn(vecPosition);
	}

	private void HandleIconDragEnd(Vector2 vecPosition)
	{
		OnUIIconDragEnd(vecPosition);
	}

	//------------------------------------------------------------------------
	protected virtual void OnUIIconClick(Vector2 vecPosition) { }
	protected virtual void OnUIIconDragStart(Vector2 vecPosition) { }
	protected virtual void OnUIIconDragOn(Vector2 vecPosition) { }
	protected virtual void OnUIIconDragEnd(Vector2 vecPosition) { }

}
