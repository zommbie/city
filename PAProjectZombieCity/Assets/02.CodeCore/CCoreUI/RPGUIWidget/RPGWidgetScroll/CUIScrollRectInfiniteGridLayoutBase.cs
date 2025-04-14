using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// 2020.08 Zerogames  정구삼
// [개요]1) 기존 ScrollRect를 서포트하여 인덱스 기반 무한 스크롤을 구현하였다.
//        2) 아이템 갯수와 상관 없이 뷰포트 크기만큼만 셀을 할당하고 스크롤 하므로 드로우콜과 마스킹 비용을 절감할수 있다. 
//        3) 가로 / 세로 스크롤을 지원한다.
// [셋팅] 1) GridLayoutGroup 컴포넌트를 사용 / mGrid.constraint == GridLayoutGroup.Constraint.FixedColumnCount : 가로세로 스크롤 제공 
// 2) ContentSizeFitter를 사용하면 안된다 (본 컴포넌트가 동적으로 변경해서 사용한다)
// 3) ScrollRect.content는 Stretch 로 셋팅 되어야 한다.(Viewport 크기로 맞춰야 한다) 
// 4) ScrollRect.content의 Pivot은 0 / 1 로 셋팅한다. (왼쪽 위가 0 좌표)
abstract public class CUIScrollRectInfiniteGridLayoutBase : CUIScrollRectSpringBase
{
	
	private int m_iViewPortCellWidth = 0;                 // 뷰포트 내부에 표시될 셀 수
	private int m_iViewPortCellHeight = 0;

	private int m_iGridCellWidthCount = 0;                // 전체 스크롤바의 가로세로 셀 수  
	private int m_iGridCellHeightCount = 0;

	private int m_iCellScrollSizeX = 0;                   // 스크롤시 Panding Size;
	private int m_iCellScrollSizeY = 0;

	private int m_iTotalCount = 0;                        // 출력될 전체 셀 갯수 	
	private int m_iTotalViewCount = 0;                    // 뷰포트에 출력될 셀 갯수 : 이 크기만큼 인스턴스가 할당된다.
	private Vector2Int m_iScrollIndex = Vector2Int.zero;  // 죄상단 기준 스크롤 기준점. 셀 단위이다. 
	private GridLayoutGroup m_pGridLayout = null;         // Scroll - contents 에 부착해야 한다.
	private List<CUIWidgetTemplateItemBase> m_listViewPortItem = new List<CUIWidgetTemplateItemBase>();

    //----------------------------------------------------------------
    protected override void OnUIEntryInitialize(CUIFrameBase _uIFrameParent)
	{
		base.OnUIEntryInitialize(_uIFrameParent);
		m_pGridLayout = m_pScrollRect.content.GetComponent<GridLayoutGroup>();
        m_pGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

		m_pScrollRect.movementType = ScrollRect.MovementType.Clamped;  // 나머지 스크롤은 계산에 맞지 않음	
		m_pScrollRect.content.pivot = new Vector2(0f, 1f); // 좌표 산출기준 좌상단
	}

	protected override void OnUIScrollValueChange(Vector2 vecChangeValue)
	{
		base.OnUIScrollValueChange(vecChangeValue);
		UpdateUIScrollInfinite();
	}

    private void OnRectTransformDimensionsChange()
    {
        if (m_pScrollRect != null)
        {
       //     m_pScrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_fTotalScrollHeightPixel);
        }
    }


    /// <summary>
    /// vecViewportOffset 은 좌 상단 기준 
    /// </summary>
    protected override CUIWidgetTemplateItemBase ProtUIScrollGetAnchorSpringItemByViewportOffset(Vector2 vecViewportOffset)
    {
        CUIWidgetTemplateItemBase pFindItem = null;
        Vector2 vecContentsPosition = GetUIPositionLeftTop(m_pContentTransform);
      
        for (int i = 0; i < m_listViewPortItem.Count; i++)
        {
            Vector2 vecLocalPosition = m_listViewPortItem[i].GetUIPositionLeftTop() + vecContentsPosition;            
            Vector2 vecSize = m_listViewPortItem[i].GetUISize();

            Vector2 vecStart = vecLocalPosition;
            Vector2 vecEnd = vecStart + vecSize;

            if (vecViewportOffset.x >= vecStart.x && vecViewportOffset.x < vecEnd.x)
            {
                if (vecViewportOffset.y >= vecStart.y && vecViewportOffset.y < vecEnd.y)
                {
                    pFindItem = m_listViewPortItem[i];
                    break;
                }
            }
        }

        return pFindItem;
    }

    //-----------------------------------------------------------------
    protected void ProtUIScrollInfiniteInitialize(int iTotalCount, float fCellWidth = 0, float fCellHeight = 0, bool bScrollReset = true)
	{
		m_iTotalCount = iTotalCount;
        m_pScrollRect.velocity = Vector2.zero;

		Vector2 vecCellSize = m_pGridLayout.cellSize;
		if (fCellWidth != 0)
        {
			vecCellSize.x = fCellWidth;
        }
		if (fCellHeight != 0)
        {
			vecCellSize.y = fCellHeight;
        }

		m_pGridLayout.cellSize = vecCellSize;

        Vector2Int ViewportSize = Vector2Int.zero;
		Vector2 ContentsPosition = m_pContentTransform.anchoredPosition;
		ViewportSize.x = Mathf.Abs((int)m_pScrollRect.viewport.rect.width);
		ViewportSize.y = Mathf.Abs((int)m_pScrollRect.viewport.rect.height);
		
		PrivUIScrollInfiniteSize(m_pGridLayout.cellSize, m_pGridLayout.spacing, ViewportSize, iTotalCount);
		PrivUIScrollInfiniteRefreshAll();	

		if (bScrollReset == false)
		{
			ProtUIScrollInfiniteSetPosition(ContentsPosition);
		}
	}

	protected void ProtUIScrollInfiniteSetPosition(int _targetIndex)
    {		
		int PosX = _targetIndex % m_iViewPortCellWidth;
		int PosY = _targetIndex / m_iViewPortCellWidth;

		if (PosX + m_iViewPortCellWidth >= m_iGridCellWidthCount)
        {
			PosX = m_iGridCellWidthCount - m_iViewPortCellWidth;
        }

		if (PosY + m_iViewPortCellHeight >= m_iGridCellHeightCount)
        {
			PosY = m_iGridCellHeightCount - m_iViewPortCellHeight + 2;
        }

		int PosXContent = PosX * m_iCellScrollSizeX;
		int PosYContent = PosY * m_iCellScrollSizeY;

		m_pScrollRect.content.anchoredPosition = new Vector2(PosXContent, PosYContent);
		UpdateUIScrollInfinite();
	}
	/// <summary>
	/// // 미니맵등 외부에서 스크롤 좌표를 입력할때
	/// </summary>
	protected void ProtUIScrollInfiniteSetPosition(Vector2 vecPosition) 
	{
		float positionX = Mathf.Clamp(vecPosition.x, -m_pScrollRect.content.sizeDelta.x, 0);
		float positionY = Mathf.Clamp(vecPosition.y, 0, m_pScrollRect.content.sizeDelta.y);

		m_pScrollRect.content.anchoredPosition = new Vector2(positionX, positionY);
		UpdateUIScrollInfinite();
	}


	//------------------------------------------------------------------
	private void UpdateUIScrollInfinite()
	{
		Vector2Int ScrollIndexCurrent = ExtractUIScrollInfiniteStartIndex();
		Vector2Int ScrollIndex = m_iScrollIndex - ScrollIndexCurrent;
		bool refresh = false;
		if (ScrollIndex.x != 0)
		{
			refresh = true;
			PrivUIScrollInfiniteRefreshWidth(ScrollIndexCurrent, ScrollIndex.x);
		}

		if (ScrollIndex.y != 0)
		{
			refresh = true;
			PrivUIScrollInfiniteRefreshHeight(ScrollIndexCurrent, ScrollIndex.y);
		}

		if (refresh)
		{
			PrivUIScrollInfiniteInvalidate();
		}

		m_iScrollIndex = ScrollIndexCurrent;

	}

	private void PrivUIScrollInfiniteRefreshAll()
	{
		PrivUIScrollInfiniteDetachAllContentsChild();
		PrivUIScrollInfiniteMakeClipingScale();
		PrivUIScrollInfiniteRefreshIndex();
		PrivUIScrollInfiniteContents();
		ProtUIScrollSensitivityRatio();
	}

	private void PrivUIScrollInfiniteSize(Vector2 _cellSize, Vector3 _spacing, Vector2Int _viewPortSize, int _totalCount)
	{
        int SpacingCountX = m_pGridLayout.constraintCount - 1;
		if (SpacingCountX < 0) SpacingCountX = 0;

		int ContentsWidth = ((int)_cellSize.x * m_pGridLayout.constraintCount) + ((int)_spacing.x * (SpacingCountX)) - _viewPortSize.x;
		int ContentsHeightCount = _totalCount / m_pGridLayout.constraintCount;
		if (_totalCount % m_pGridLayout.constraintCount > 0)
		{
			ContentsHeightCount++;
		}

		int SpacingCountY = ContentsHeightCount - 1;
		if (SpacingCountY < 0) SpacingCountY = 0;

		int ContentsHeight = ((int)_cellSize.y * ContentsHeightCount) + ((int)_spacing.y * (SpacingCountY)) - _viewPortSize.y;

		int ViewPortCellWidth = (int)(_viewPortSize.x / _cellSize.x) + 1;    // 위 아래 양 옆에 한칸씩 여분을 만들어야 한다. 
		int ViewPortCellHeight = (int)(_viewPortSize.y / _cellSize.y) + 1;  

		int RestX = _viewPortSize.x - (int)(_spacing.x * ((int)(_viewPortSize.x / _cellSize.x) - 1)); 
		int RestY = _viewPortSize.y - (int)(_spacing.y * ((int)(_viewPortSize.y / _cellSize.y) - 1));

		RestX = RestX % (int)_cellSize.x;
		RestY = RestY % (int)_cellSize.y;

		if (RestX > 0) RestX = 1;
		if (RestY > 0) RestY = 1;

		m_iViewPortCellWidth = ViewPortCellWidth + RestX;
		m_iViewPortCellHeight = ViewPortCellHeight + RestY;

		m_iGridCellWidthCount = m_pGridLayout.constraintCount;
		m_iGridCellHeightCount = ContentsHeightCount;

		if (m_iViewPortCellWidth > m_iGridCellWidthCount) m_iViewPortCellWidth = m_iGridCellWidthCount;
		if (m_iViewPortCellHeight > m_iGridCellHeightCount) m_iViewPortCellHeight = m_iGridCellHeightCount;

		m_iCellScrollSizeX = (int)(m_pGridLayout.cellSize.x + m_pGridLayout.spacing.x);
		m_iCellScrollSizeY = (int)(m_pGridLayout.cellSize.y + m_pGridLayout.spacing.y);

		m_pScrollRect.content.sizeDelta = new Vector2(ContentsWidth, ContentsHeight);
	}

	private void PrivUIScrollInfiniteDetachAllContentsChild()
	{
		for (int i = 0; i < m_listViewPortItem.Count; i++)
		{
			DoUITemplateReturn(m_listViewPortItem[i]);
		}
		m_listViewPortItem.Clear();

		m_pGridLayout.padding.left = 0;
		m_pGridLayout.padding.right = 0;
		m_pGridLayout.padding.top = 0;
		m_pGridLayout.padding.bottom = 0;
	}

	private void PrivUIScrollInfiniteMakeClipingScale()
	{
		m_iTotalViewCount = m_iViewPortCellWidth * m_iViewPortCellHeight;
		m_pGridLayout.constraintCount = m_iViewPortCellWidth;
	}

	private Vector2Int ExtractUIScrollInfiniteStartIndex() // 앵커포지션 기반으로 상좌단 셀 인덱스를 추출 
	{
		Vector2Int StartIndex = Vector2Int.zero;
		StartIndex.x = (int)((m_pScrollRect.content.anchoredPosition.x) / (m_pGridLayout.cellSize.x + (m_pGridLayout.spacing.x)));
		StartIndex.y = (int)((m_pScrollRect.content.anchoredPosition.y) / (m_pGridLayout.cellSize.y + (m_pGridLayout.spacing.y)));

		StartIndex.x = Mathf.Abs(StartIndex.x);
		StartIndex.y = Mathf.Abs(StartIndex.y);

		if (StartIndex.x > m_iGridCellWidthCount - m_iViewPortCellWidth)
		{
			StartIndex.x = m_iGridCellWidthCount - m_iViewPortCellWidth;
		}

		if (StartIndex.y > m_iGridCellHeightCount - m_iViewPortCellHeight)
		{
			StartIndex.y = m_iGridCellHeightCount - m_iViewPortCellHeight;
		}

		int PaddingX = StartIndex.x * m_iCellScrollSizeX;
		int PaddingY = StartIndex.y * m_iCellScrollSizeY;

		m_pGridLayout.padding.left = PaddingX;
		m_pGridLayout.padding.top = PaddingY;

		return StartIndex;
	}

	private void PrivUIScrollInfiniteRefreshIndex()
	{
		for (int h = 0; h < m_iViewPortCellHeight; h++)
		{
			for (int w = 0; w < m_iViewPortCellWidth; w++)
			{
				int IndexX = w;
				if (IndexX < m_iGridCellWidthCount)
				{
					int IndexY = h;
					if (IndexY < m_iGridCellHeightCount)
					{
						int IndexCell = (IndexY * m_iGridCellWidthCount) + IndexX;
						PrivUIScrollGridLayoutGroupCreatItem(IndexCell);
					}
				}
			}
		}
	}

	private void PrivUIScrollInfiniteContents()
	{
		m_pScrollRect.content.anchoredPosition = Vector2.zero;
		m_iScrollIndex = Vector2Int.zero;
	}

	private void PrivUIScrollInfiniteRefreshWidth(Vector2Int _startIndex, int _scrollX)
	{
		int ScrollXTotal = Mathf.Abs(_scrollX);
		Vector2Int ScrollStartIndex = _startIndex;

		for (int i = 0; i < ScrollXTotal; i++)
		{
			if (_scrollX < 0)
			{
				ScrollStartIndex.x = _startIndex.x - ((ScrollXTotal - 1) - i);
				PrivUIScrollInfiniteRefreshChildItem();
				PrivUIScrollInfiniteSwapRight(ScrollStartIndex);
			}
			else
			{
				ScrollStartIndex.x = _startIndex.x + ((ScrollXTotal - 1) - i);
				PrivUIScrollInfiniteRefreshChildItem();
				PrivUIScrollInfiniteSwapLeft(ScrollStartIndex);
			}
		}
	}

	private void PrivUIScrollInfiniteRefreshHeight(Vector2Int _startIndex, int _scrollY)
	{
		int ScrollYTotal = Mathf.Abs(_scrollY);
		Vector2Int ScrollStartIndex = _startIndex;

		for (int i = 0; i < ScrollYTotal; i++)
		{
			if (_scrollY < 0)
			{
				ScrollStartIndex.y = _startIndex.y - ((ScrollYTotal - 1) - i);
				PrivUIScrollInfiniteRefreshChildItem();
				PrivUIScrollInfiniteSwapBottom(ScrollStartIndex);
			}
			else
			{
				ScrollStartIndex.y = _startIndex.y + ((ScrollYTotal - 1) - i);
				PrivUIScrollInfiniteRefreshChildItem();
				PrivUIScrollInfiniteSwapTop(ScrollStartIndex);
			}
		}
	}

	private void PrivUIScrollInfiniteSwapRight(Vector2Int _startIndex)
	{
		int IndexOff = 0;
		int IndexOn = 0;

		for (int i = 0; i < m_iViewPortCellHeight; i++)
		{
			IndexOff = i * m_iViewPortCellWidth;
			IndexOn = IndexOff + m_iViewPortCellWidth - 1;

			CUIWidgetTemplateItemBase Item = m_listViewPortItem[IndexOff];
			int ItemSlotIndex = _startIndex.x + (m_iViewPortCellWidth - 1) + (i* m_iGridCellWidthCount) + (_startIndex.y  * m_iGridCellWidthCount);

			Item.transform.SetSiblingIndex(IndexOn);
			if (ItemSlotIndex >= m_iTotalCount)
			{
				Item.DoUITemplateItemShow(false);
			}
			else
			{
				PrivUIScrollInfiniteRefreshItem(ItemSlotIndex, Item);
			}
		}
	}

	private void PrivUIScrollInfiniteSwapLeft(Vector2Int _startIndex)
	{
		int IndexOff = 0;
		int IndexOn = 0;
		
		for (int i = 0; i < m_iViewPortCellHeight; i++)
		{
			IndexOn = i * m_iViewPortCellWidth;
			IndexOff = IndexOn + m_iViewPortCellWidth - 1;

			CUIWidgetTemplateItemBase Item = m_listViewPortItem[IndexOff];
			int ItemSlotIndex = _startIndex.x + ( i * m_iGridCellWidthCount) + (_startIndex.y * m_iGridCellWidthCount);

			Item.transform.SetSiblingIndex(IndexOn);

			if (ItemSlotIndex >= m_iTotalCount)
			{
				Item.DoUITemplateItemShow(false);
			}
			else
			{
				PrivUIScrollInfiniteRefreshItem(ItemSlotIndex, Item);
			}
		}
	}

	private void PrivUIScrollInfiniteSwapTop(Vector2Int _startIndex)
	{
		int IndexOff = 0;
		int IndexOn = 0;

		for (int i = 0; i < m_iViewPortCellWidth; i++)
		{
			IndexOn = i;
			IndexOff = IndexOn + ((m_iViewPortCellHeight - 1) * m_iViewPortCellWidth);

			CUIWidgetTemplateItemBase Item = m_listViewPortItem[IndexOff];
			int ItemSlotIndex = i + (_startIndex.y * m_iGridCellWidthCount) + _startIndex.x;
			Item.transform.SetSiblingIndex(IndexOn);
			if (ItemSlotIndex >= m_iTotalCount)
			{
				Item.DoUITemplateItemShow(false);			
			}
			else
			{
				PrivUIScrollInfiniteRefreshItem(ItemSlotIndex, Item);
			}
		}
	}

	private void PrivUIScrollInfiniteSwapBottom(Vector2Int _startIndex)
	{
		int IndexOff = 0;
		int IndexOn = 0;

		for (int i = 0; i < m_iViewPortCellWidth; i++)
		{
			IndexOff = i;
			IndexOn = IndexOff + (m_iViewPortCellHeight * m_iViewPortCellWidth);

			CUIWidgetTemplateItemBase Item = m_listViewPortItem[IndexOff];
			int ItemSlotIndex = (_startIndex.x + i ) + (m_iGridCellWidthCount * (m_iViewPortCellHeight + _startIndex.y - 1));

			Item.transform.SetSiblingIndex(IndexOn);

			if (ItemSlotIndex >= m_iTotalCount)
			{
				Item.DoUITemplateItemShow(false);
			}
			else
			{
				PrivUIScrollInfiniteRefreshItem(ItemSlotIndex, Item);
			}
		}
	}

	private void PrivUIScrollInfiniteRefreshChildItem()
	{
        m_listViewPortItem.Clear();

		int Total = m_pScrollRect.content.childCount;
		for (int i = 0; i < Total; i++)
        {
			m_listViewPortItem.Add(m_pScrollRect.content.GetChild(i).gameObject.GetComponent<CUIWidgetTemplateItemBase>());
        }
    }

	private void PrivUIScrollInfiniteInvalidate()
	{
		PrivUIScrollInfiniteArrange();
		LayoutRebuilder.MarkLayoutForRebuild(m_pScrollRect.content);
	}

	private void PrivUIScrollInfiniteRefreshItem(int _itemIndex, CUIWidgetTemplateItemBase _item)
	{
		if (_itemIndex >= m_iTotalCount || _itemIndex < 0) return;
		
		_item.DoUITemplateItemShow(true);
		_item.DoUITemplateItemRefreshIndex(_itemIndex);

		OnUIScrollInifiniteRefreshItem(_itemIndex, _item);
	}

	private void PrivUIScrollInfiniteArrange()
	{
		if (m_pScrollRect.content.childCount <= 0) return;

		int StartIndex = 0;
		for (int H = 0; H < m_iViewPortCellHeight; H++)
		{
			for (int W = 0; W < m_iViewPortCellWidth; W++)
			{
				int itemIndex = (H * m_iViewPortCellWidth) + W;
				CUIWidgetTemplateItemBase item = m_pScrollRect.content.GetChild(itemIndex).GetComponent<CUIWidgetTemplateItemBase>();
				int SlotIndex = item.GetUITemplateItemIndex();
				if (W == 0 && H == 0)
				{
					StartIndex = SlotIndex;  
					continue; 
				}

				int nextSlotIndex = StartIndex + W + (H * m_iGridCellWidthCount);
				if (SlotIndex != nextSlotIndex)
				{
					PrivUIScrollInfiniteRefreshItem(nextSlotIndex, item);
				}
			}
		}		
	}

	private void PrivUIScrollGridLayoutGroupCreatItem(int _index)
	{
		if (m_listViewPortItem.Count >= m_iTotalViewCount) return;

		CUIWidgetTemplateItemBase item = DoUITemplateRequestItem(m_pScrollRect.content);		
		m_listViewPortItem.Add(item);
		item.DoUITemplateItemRefreshIndex(_index);

		if (_index >= m_iTotalCount)
		{
			item.DoUITemplateItemShow(false);
		}
		else
        {
			PrivUIScrollInfiniteRefreshItem(_index, item);
		}
	}	


	//-----------------------------------------------------------------
	protected virtual void OnUIScrollInifiniteRefreshItem(int iIndex, CUIWidgetTemplateItemBase pNewItem) {}

}
