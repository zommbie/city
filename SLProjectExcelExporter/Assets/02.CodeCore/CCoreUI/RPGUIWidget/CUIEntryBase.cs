using UnityEngine;

public abstract class CUIEntryBase : CMonoBase
{
	private CUIFrameBase m_pParentFrame;			public CUIFrameBase			GetUIEntryParentsUIFrame() { return m_pParentFrame; }
	private RectTransform m_pRectTransform = null;	protected RectTransform		GetUIEntryRectTransform() { return m_pRectTransform; }
	private CUIEntryBase m_pEntryOwner = null;		public CUIEntryBase			GetUIEntryOwner() { return m_pEntryOwner; }
	//------------------------------------------------------------------------
	internal void InterUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		m_pParentFrame = pParentFrame;
		m_pRectTransform = GetComponent<RectTransform>();
		OnUIEntryInitialize(pParentFrame);
	}

	internal void InterUIEntryInitializePost(CUIFrameBase pParentFrame) // 모든 위젯들이 초기화 된 이후 호출. 버튼의 디폴트 프리셋등 다른 위젯과 상호작용 하는 기능 삽입
	{
		OnUIEntryInitializePost(pParentFrame);
	}

	internal void InterUIEntryOwner(CUIEntryBase pEntryOwner)        // 위젯같의 상하관계가 있을 경우
	{
		m_pEntryOwner = pEntryOwner;
		OnUIEntryOwner(pEntryOwner);
	}

	internal void InterUIEntryUIFramwShowHide(bool bShow)
	{
		OnUIEntryUIFrameShowHide(bShow);
	}
		
	internal void InterUIEntryChangeOrder(int iOrder)
	{
		OnUIEntryChangeOrder(iOrder);
	}
	//-----------------------------------------------------------------------
	public object SendUIEntryMessage(int hMessageID, int iArg = 0, float fArg = 0, string strArg = null, params object [] aParams) // Inter 함수의 난립을 막기 위한 공용 수신 함수
	{
		return OnUIEntryMessage(hMessageID, iArg, fArg, strArg, aParams);
	}
	//-----------------------------------------------------------------------
	protected virtual void OnUIEntryInitialize(CUIFrameBase pParentFrame) { }
	protected virtual void OnUIEntryInitializePost(CUIFrameBase pParentFrame) { }
	protected virtual void OnUIEntryChangeOrder(int iOrder) { }
	protected virtual void OnUIEntryUIFrameShowHide(bool bShow) { }
	protected virtual void OnUIEntryOwner(CUIEntryBase pWidgetOwner) { }
	protected virtual object OnUIEntryMessage(int hMessageID, int iArg, float fArg, string strArg, params object[] aParams) { return null; }
	//-----------------------------------------------------------------------
	public void SetUIPositionX(float X)
	{
		m_pRectTransform.anchoredPosition = new Vector3(X, m_pRectTransform.anchoredPosition.y);
	}

	public void SetUIPositionY(float Y)
	{
		m_pRectTransform.anchoredPosition = new Vector3(m_pRectTransform.anchoredPosition.x, Y);
	}

	public void SetUIPosition(float X, float Y)
	{
		m_pRectTransform.anchoredPosition = new Vector2(X, Y);
	}

	public void SetUIPosition(Vector2 vecPosition)
	{
		m_pRectTransform.anchoredPosition = vecPosition;
	}

	public void SetUIPositionMoveX(float X)
	{
		m_pRectTransform.localPosition = new Vector2(m_pRectTransform.localPosition.x + X, m_pRectTransform.localPosition.y);
	}

	public void SetUIPositionMoveY(float Y)
	{
		m_pRectTransform.localPosition = new Vector2(m_pRectTransform.localPosition.x, m_pRectTransform.localPosition.y + Y);
	}

	public void SetUISiblingLowest()
	{
		m_pRectTransform.SetSiblingIndex(0);
	}

	public void SetUISiblingTopMost()
	{
		Transform pParent = transform.parent;
		if (pParent)
		{
			m_pRectTransform.SetSiblingIndex(pParent.childCount);
		}
	}

	public void SetUIWidth(float fWidth)
	{
		m_pRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fWidth);
	}

	public void SetUIHeight(float fHeight)
	{
		m_pRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fHeight);
	}

	public void SetUISize(Vector2 vecSize)
	{
		m_pRectTransform.sizeDelta = vecSize;
	}

	public void SetUIPivot(Vector2 vecPivot)
	{
		m_pRectTransform.pivot = vecPivot;
	}

	public void SetUIAnchor(EAnchorPresets allign, int offsetX = 0, int offsetY = 0)
	{
		m_pRectTransform.anchoredPosition = new Vector3(offsetX, offsetY, 0);

		switch (allign)
		{
			case (EAnchorPresets.TopLeft):
				{
					m_pRectTransform.anchorMin = new Vector2(0, 1);
					m_pRectTransform.anchorMax = new Vector2(0, 1);
					break;
				}
			case (EAnchorPresets.TopCenter):
				{
					m_pRectTransform.anchorMin = new Vector2(0.5f, 1);
					m_pRectTransform.anchorMax = new Vector2(0.5f, 1);
					break;
				}
			case (EAnchorPresets.TopRight):
				{
					m_pRectTransform.anchorMin = new Vector2(1, 1);
					m_pRectTransform.anchorMax = new Vector2(1, 1);
					break;
				}

			case (EAnchorPresets.MiddleLeft):
				{
					m_pRectTransform.anchorMin = new Vector2(0, 0.5f);
					m_pRectTransform.anchorMax = new Vector2(0, 0.5f);
					break;
				}
			case (EAnchorPresets.MiddleCenter):
				{
					m_pRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
					m_pRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
					break;
				}
			case (EAnchorPresets.MiddleRight):
				{
					m_pRectTransform.anchorMin = new Vector2(1, 0.5f);
					m_pRectTransform.anchorMax = new Vector2(1, 0.5f);
					break;
				}

			case (EAnchorPresets.BottomLeft):
				{
					m_pRectTransform.anchorMin = new Vector2(0, 0);
					m_pRectTransform.anchorMax = new Vector2(0, 0);
					break;
				}
			case (EAnchorPresets.BottonCenter):
				{
					m_pRectTransform.anchorMin = new Vector2(0.5f, 0);
					m_pRectTransform.anchorMax = new Vector2(0.5f, 0);
					break;
				}
			case (EAnchorPresets.BottomRight):
				{
					m_pRectTransform.anchorMin = new Vector2(1, 0);
					m_pRectTransform.anchorMax = new Vector2(1, 0);
					break;
				}

			case (EAnchorPresets.HorStretchTop):
				{
					m_pRectTransform.anchorMin = new Vector2(0, 1);
					m_pRectTransform.anchorMax = new Vector2(1, 1);
					break;
				}
			case (EAnchorPresets.HorStretchMiddle):
				{
					m_pRectTransform.anchorMin = new Vector2(0, 0.5f);
					m_pRectTransform.anchorMax = new Vector2(1, 0.5f);
					break;
				}
			case (EAnchorPresets.HorStretchBottom):
				{
					m_pRectTransform.anchorMin = new Vector2(0, 0);
					m_pRectTransform.anchorMax = new Vector2(1, 0);
					break;
				}

			case (EAnchorPresets.VertStretchLeft):
				{
					m_pRectTransform.anchorMin = new Vector2(0, 0);
					m_pRectTransform.anchorMax = new Vector2(0, 1);
					break;
				}
			case (EAnchorPresets.VertStretchCenter):
				{
					m_pRectTransform.anchorMin = new Vector2(0.5f, 0);
					m_pRectTransform.anchorMax = new Vector2(0.5f, 1);
					break;
				}
			case (EAnchorPresets.VertStretchRight):
				{
					m_pRectTransform.anchorMin = new Vector2(1, 0);
					m_pRectTransform.anchorMax = new Vector2(1, 1);
					break;
				}

			case (EAnchorPresets.StretchAll):
				{
					m_pRectTransform.anchorMin = new Vector2(0, 0);
					m_pRectTransform.anchorMax = new Vector2(1, 1);
					break;
				}
		}
	}

	public void SetUIPivot(EPivotPresets preset)
	{

		switch (preset)
		{
			case (EPivotPresets.TopLeft):
				{
					m_pRectTransform.pivot = new Vector2(0, 1);
					break;
				}
			case (EPivotPresets.TopCenter):
				{
					m_pRectTransform.pivot = new Vector2(0.5f, 1);
					break;
				}
			case (EPivotPresets.TopRight):
				{
					m_pRectTransform.pivot = new Vector2(1, 1);
					break;
				}

			case (EPivotPresets.MiddleLeft):
				{
					m_pRectTransform.pivot = new Vector2(0, 0.5f);
					break;
				}
			case (EPivotPresets.MiddleCenter):
				{
					m_pRectTransform.pivot = new Vector2(0.5f, 0.5f);
					break;
				}
			case (EPivotPresets.MiddleRight):
				{
					m_pRectTransform.pivot = new Vector2(1, 0.5f);
					break;
				}

			case (EPivotPresets.BottomLeft):
				{
					m_pRectTransform.pivot = new Vector2(0, 0);
					break;
				}
			case (EPivotPresets.BottomCenter):
				{
					m_pRectTransform.pivot = new Vector2(0.5f, 0);
					break;
				}
			case (EPivotPresets.BottomRight):
				{
					m_pRectTransform.pivot = new Vector2(1, 0);
					break;
				}
		}
	}

	//-------------------------------------------------------------------------
	public float GetUIWidth()
	{
		return m_pRectTransform.rect.width;
	}

	public float GetUIHeight()
	{
		return m_pRectTransform.rect.height;
	}

	public Vector2 GetUISize()
	{
		return new Vector2(m_pRectTransform.rect.width, m_pRectTransform.rect.height);
	}

	public Vector2 GetUIPosition()
	{
		return m_pRectTransform.anchoredPosition;
	}

	public Vector2 GetUIPositionCenter()
	{
		return GetUIPositionCenter(m_pRectTransform);
	}

	public static Vector2 GetUIPositionCenter(RectTransform pRectTransform)
	{
		return GetUIPositionFromPivot(pRectTransform, new Vector2(0.5f, 0.5f));
	}

	public Vector2 GetUIPositionLeftTop()
	{
		return GetUIPositionLeftTop(m_pRectTransform);
	}

	public static Vector2 GetUIPositionLeftTop(RectTransform pRectTransform)
	{
		return GetUIPositionFromPivot(pRectTransform, new Vector2(0f, 1f));
	}

	public Vector2 GetUIPositionFromPivot(Vector2 vecPivot)
	{
		return GetUIPositionFromPivot(m_pRectTransform, vecPivot);
	}

	public static Vector2 GetUIPositionFromPivot(RectTransform pRectTransform, Vector2 vecPivot)
	{
		Vector2 vecPosition = pRectTransform.anchoredPosition;
		vecPosition.x += (vecPivot.x - pRectTransform.pivot.x) * pRectTransform.rect.width;
		vecPosition.y += (vecPivot.y - pRectTransform.pivot.y) * pRectTransform.rect.height;
		return vecPosition;
	}

	public float GetUIPositionX()
	{
		return m_pRectTransform.anchoredPosition.x;
	}

	public float GetUIPositionLocalX()
	{
		return m_pRectTransform.localPosition.x;
	}

	public float GetUIPositionLocalY()
	{
		return m_pRectTransform.localPosition.y;
	}

	public float GetUIPositionY()
	{
		return m_pRectTransform.anchoredPosition.y;
	}

	public Rect GetUIRectWorldPosition()
	{
		Vector3[] aCorners = new Vector3[4];
		m_pRectTransform.GetWorldCorners(aCorners);
		Vector2 vecWorldScreen = RectTransformUtility.WorldToScreenPoint(CManagerUIFrameBase.Instance.GetUIManagerCamara(), aCorners[0]); // 이미지 좌하단 기준 (포인터 좌표가 좌하단이 0 / 0)
		return new Rect(vecWorldScreen, m_pRectTransform.rect.size);
	}
}



public enum EAnchorPresets
{
	TopLeft,
	TopCenter,
	TopRight,

	MiddleLeft,
	MiddleCenter,
	MiddleRight,

	BottomLeft,
	BottonCenter,
	BottomRight,
	BottomStretch,

	VertStretchLeft,
	VertStretchRight,
	VertStretchCenter,

	HorStretchTop,
	HorStretchMiddle,
	HorStretchBottom,

	StretchAll
}

public enum EPivotPresets
{
	TopLeft,
	TopCenter,
	TopRight,

	MiddleLeft,
	MiddleCenter,
	MiddleRight,

	BottomLeft,
	BottomCenter,
	BottomRight,
}

