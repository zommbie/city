using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public abstract class CUIWidgetTabPageMovableBase : CUIWidgetBase
{
	[System.Serializable]
	public class STabPageInfo
	{
		public CUIWidgetTabPageMovableItemBase  TabPage;
		public CUIButtonRadioBase				TabButton;
	}

	[SerializeField]
	private AnimationCurve MoveCurve;
	[SerializeField]
	private float MoveTime = 1f;
	
	[SerializeField]
	private List<STabPageInfo> TabPageInfo = null;					public int p_TotalPage { get { return TabPageInfo.Count; } }

	private STabPageInfo m_pTabPageCurrent = null;
	private STabPageInfo m_pTabPageNext = null;
	
	private float m_fCurrentMoveTime = 0;
	private bool m_bTabPageMoving = false;							public bool IsTabPageMoving { get { return m_bTabPageMoving; } }
	private bool m_bMoveForward = true;								public bool IsMoveForward { get { return m_bMoveForward; } }
	//-----------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitialize(pParentFrame);

		PrivTabPageArrangeIndex();
		PrivTabPageSelectShowHide(null);
	}

	protected override void OnUIEntryInitializePost(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitializePost(pParentFrame);
		ProtTabPageReset();
	}

	private void Update()
	{
		float fDelta = Time.deltaTime;
		UpdateTabPage(fDelta);
		OnTabPageUpdate(fDelta);
	}

	//------------------------------------------------------------
	protected CUIWidgetTabPageMovableItemBase ProtTabPageReset(int iPageIndex = 0)
	{		
		m_fCurrentMoveTime = 0f;
		PrivTabPageArrangeIndex();
		PrivTabPageShow(iPageIndex);
		return m_pTabPageCurrent.TabPage;
	}

	protected CUIWidgetTabPageMovableItemBase ProtTabPageMoveStart(bool bForward = true) // 포워드는 오른쪽에서 왼쪽으로 이동 
	{
		if (TabPageInfo.Count <= 1) return null;
		if (m_bTabPageMoving) return null;
		
		if (bForward)
		{
			PrivTabPageShowNext(m_pTabPageCurrent.TabPage.GetTabPageItemIndex() + 1);
		}
		else
		{
			PrivTabPageShowPrev(m_pTabPageCurrent.TabPage.GetTabPageItemIndex() - 1);
		}

		m_bTabPageMoving = true;
		m_fCurrentMoveTime = 0;

		m_pTabPageCurrent.TabPage.InterTabPageItemMoveStart();
		m_pTabPageNext.TabPage.InterTabPageItemMoveStart();

		OnTabPageItemMoveStart(m_pTabPageCurrent.TabPage, m_pTabPageNext.TabPage);
		return m_pTabPageNext.TabPage;
	}

	protected STabPageInfo FindTabPageItem(int iIndex)
	{
		STabPageInfo pFindTabPageInfo = null;
		if (iIndex >= 0 && iIndex < TabPageInfo.Count)
		{
			pFindTabPageInfo = TabPageInfo[iIndex];
		}

		return pFindTabPageInfo;
	}

	protected void ProtTabPageAddInfo(STabPageInfo pAddTabPageInfo)
	{
        pAddTabPageInfo.TabPage.InterTabPageItemSetIndex(TabPageInfo.Count);
		PrivTabPageShowHide(pAddTabPageInfo, false);
        TabPageInfo.Add(pAddTabPageInfo);
	}

	//-------------------------------------------------------------
	private void PrivTabPageArrangeIndex()
	{
		for (int i = 0; i < TabPageInfo.Count; i++)
		{
			TabPageInfo[i].TabPage.InterTabPageItemSetIndex(i);
		}
	}

	private void PrivTabPageShow(int iPageIndex)
	{
		STabPageInfo pPageShow = FindTabPageShow(iPageIndex);

		if (pPageShow == null) return;

		PrivTabPageSelectShowHide(pPageShow);
		pPageShow.TabPage.SetUIPosition(0, 0);
		pPageShow.TabButton.DoButtonToggleOn(false);
		pPageShow.TabPage.InterTabPageItemMain();
		
		m_pTabPageCurrent = pPageShow;
	}

	private void PrivTabPageShowNext(int iPageIndex)
	{
		PrivTabPageSetPage(iPageIndex);

		float fWidth = m_pTabPageCurrent.TabPage.GetUIWidth();
		m_pTabPageNext.TabPage.SetUIPositionX(fWidth);

		m_bMoveForward = true;
	}

	private void PrivTabPageShowPrev(int iPageIndex)
	{
		PrivTabPageSetPage(iPageIndex);
		float fWidth = m_pTabPageCurrent.TabPage.GetUIWidth();
		m_pTabPageNext.TabPage.SetUIPositionX(-fWidth);

		m_bMoveForward = false;
	}

	private void PrivTabPageSetPage(int iPageIndex)
	{
		STabPageInfo pPage = FindTabPageShow(iPageIndex, true);
		pPage.TabPage.InterTabPageItemShow();
		pPage.TabButton.DoButtonToggleOn(false);
		m_pTabPageNext = pPage;
	}


	private void PrivTabPageShowHide(STabPageInfo pPageInfo, bool bShow)
	{
		if (pPageInfo == null) return;

		if (bShow)
		{
			pPageInfo.TabPage.InterTabPageItemShow();
			OnTabPageItemShow(pPageInfo.TabPage);
		}
		else
		{
			pPageInfo.TabPage.InterTabPageItemHide();
			OnTabPageItemHide(pPageInfo.TabPage);
		}
	}


	private STabPageInfo FindTabPageShow(int iPageIndex, bool bReturnIndex = false)
	{
		STabPageInfo pPageInfo = null;

		if (iPageIndex >= 0 && iPageIndex < TabPageInfo.Count)
		{
			pPageInfo = TabPageInfo[iPageIndex];
		}

		if (pPageInfo == null && bReturnIndex && TabPageInfo.Count > 0)
		{
			if (iPageIndex >= TabPageInfo.Count)
			{
				pPageInfo = TabPageInfo[0];
			}
			else if (iPageIndex < 0)
			{
				pPageInfo = TabPageInfo[TabPageInfo.Count - 1];
			}
		}

		return pPageInfo;
	}

	private void PrivTabPageSelectShowHide(STabPageInfo pPageInfoShow)
	{
		for (int i = 0; i < TabPageInfo.Count; i++)
		{
			if (TabPageInfo[i] == pPageInfoShow)
			{
				PrivTabPageShowHide(TabPageInfo[i], true);
			}
			else
			{
				PrivTabPageShowHide(TabPageInfo[i], false);
			}
		}
	}

	private void UpdateTabPage(float fDelta)
	{
		if (m_bTabPageMoving == false) return;

		m_fCurrentMoveTime += fDelta;
		if (m_fCurrentMoveTime >= MoveTime)
		{
			PrivTabPageMoveFinish();
		}
		else
		{
			PrivTabPageMove(m_fCurrentMoveTime, m_pTabPageCurrent.TabPage, m_bMoveForward);
		}		
	}

	private void PrivTabPageMoveFinish()
	{
		PrivTabPageSelectShowHide(m_pTabPageNext);

		CUIWidgetTabPageMovableItemBase pPageMain = m_pTabPageCurrent.TabPage;
		CUIWidgetTabPageMovableItemBase pPageNext = m_pTabPageNext.TabPage;

		pPageNext.SetUIPositionX(0);

		pPageMain.InterTabPageItemMoveEnd();
		pPageNext.InterTabPageItemMoveEnd();
		pPageNext.InterTabPageItemMain();

		m_bTabPageMoving = false;
		m_fCurrentMoveTime = 0f;
		
		m_pTabPageCurrent = m_pTabPageNext;
		OnTabPageItemMoveEnd(pPageMain, pPageNext);
	}

	private void PrivTabPageMove(float fMoveTime, CUIWidgetTabPageMovableItemBase pMovePage, bool bMoveForward) // true는 오른쪽에서 왼쪽으로 이동 
	{
		float fMoveRate = fMoveTime / MoveTime;
		float fCurveValue = MoveCurve.Evaluate(fMoveRate);
		float fPageWidth = pMovePage.GetUIWidth();
		float fMovePixel = fPageWidth * fCurveValue;
		float fPositionCurrentPage = 0;
		float fPositionNextPage = 0; 
		if (bMoveForward)
		{
			fPositionCurrentPage = -fMovePixel;
			fPositionNextPage = fPageWidth - fMovePixel;
		}
		else
		{
			fPositionCurrentPage = fMovePixel;
			fPositionNextPage = -fPageWidth + fMovePixel;
		}

		m_pTabPageCurrent.TabPage.SetUIPositionX(fPositionCurrentPage);
		m_pTabPageNext.TabPage.SetUIPositionX(fPositionNextPage);
	}

	//--------------------------------------------------------------------------------
	protected virtual void OnTabPageItemShow(CUIWidgetTabPageMovableItemBase pPageItem) { }
	protected virtual void OnTabPageItemHide(CUIWidgetTabPageMovableItemBase pPageItem) { }
	protected virtual void OnTabPageItemMoveStart(CUIWidgetTabPageMovableItemBase pPageMain, CUIWidgetTabPageMovableItemBase pPageNext) { }
	protected virtual void OnTabPageItemMoveEnd(CUIWidgetTabPageMovableItemBase pPageMain, CUIWidgetTabPageMovableItemBase pPageNext) { }
	protected virtual void OnTabPageUpdate(float fDelta) { }
}
