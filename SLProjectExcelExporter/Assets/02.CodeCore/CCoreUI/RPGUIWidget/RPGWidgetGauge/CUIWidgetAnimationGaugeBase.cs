using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public abstract class CUIWidgetAnimationGaugeBase : CUIWidgetBase
{
	[SerializeField, Range(0.1f, 10f)]
	private float LengthDuration = 1f; // 처음부터 끝까지 움직이는 시간 . 별도 입력이 없으면 이 값을 대표 값으로 사용

	[SerializeField]
	private float MinValue = 0;

	[SerializeField]
	private AnimationCurve GaugeCurve = AnimationCurve.Linear(0, 0, 1f, 1f);  // 입력 구간을 움직이는데 필요한 이동값 곡선 1f = 전체 이동값

	private Slider	    m_pSlider = null;
	private float		m_fValueDest = 0;		 public float p_ValueDest    { get { return m_fValueDest; } }      // 실제 게이지 수치  
	private float		m_fValueCurrent = 0;	 public float p_ValueCurrent { get { return m_fValueCurrent; } }   // 현재 게이지 수치  	
	private float		m_fValueOrigin = 0;		 
	private float		m_fValueMax = 1f;
	private float		m_fCurrentTime = 0;
	private float		m_fDestTime = 0;       // 목표 도달 까지 시간 - 커브 적용을 위한 값
	private float		m_fDestLength = 0;     // 목표 까지 도달거리. 절대 값

	private bool		m_bGaugeForward = false;  public bool IsGaugeForward { get { return m_bGaugeForward; }} // 정방향 = 오른쪽  
	private bool		m_bGaugeActive = false;   public bool IsGaugeActive  { get { return m_bGaugeActive;  }}
	//----------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitialize(pParentFrame);
		m_pSlider = GetComponent<Slider>();
		m_pSlider.value = 1f;
		m_pSlider.interactable = false;
	}

	protected override void OnUIEntryUIFrameShowHide(bool bShow)	{ if (bShow == false) { ProtGaugeStop();}}
	protected override void OnUIWidgetShowHide(bool bShow) { if (bShow == false) { ProtGaugeStop();}}
	protected override void OnUIWidgetShowHideForce(bool bShow) { if (bShow == false) { ProtGaugeStop();}}

	private void Update()
	{
		if (m_bGaugeActive)
		{
			float fDelta = Time.deltaTime;
			UpdateGaugeValue(fDelta);			
		}
	}
	//----------------------------------------------------------
	protected virtual void ProtGaugeStart(float fDestValue, float fLengthDuration = 0, bool bForce = false)
	{
		if (fDestValue < 0) fDestValue = 0;
		if (fDestValue > m_fValueMax) fDestValue = m_fValueMax;
		if (fLengthDuration != 0) LengthDuration = fLengthDuration;

		PrivGaugeDirection(fDestValue);

        if (bForce || m_fValueCurrent == fDestValue)
		{
			PrivGaugeEnd(fDestValue);
		}
		else
		{
			PrivGaugeStart(fDestValue);
		}
	}

	protected virtual void ProtGaugeReset(float fGaugeMax, float fCurrentValue)
	{
		m_fValueMax = fGaugeMax == 0 ? 1f : fGaugeMax;
		m_fValueOrigin = fCurrentValue;
		m_fValueCurrent = fCurrentValue;
		m_fValueDest = fCurrentValue;
		m_bGaugeActive = false;

		PrivGaugeValueApplySlider(fCurrentValue);
		OnUIGaugeReset(fGaugeMax, fCurrentValue);
	}

	protected virtual void ProtGaugeStop()
	{
		if (m_bGaugeActive)
		{
			PrivGaugeEnd(m_fValueDest);
		}
	}
	//--------------------------------------------------------------
	public void SetGaugeFillColor(Color rColor)
	{
		CImage pImage = m_pSlider.fillRect.gameObject.GetComponent<CImage>();
		if (pImage)
		{
			pImage.color = rColor;
		}
	}

	public RectTransform GetGaugeFillRectTransform()
	{
		return m_pSlider.fillRect;
	}

	//-----------------------------------------------------------
	private void PrivGaugeStart(float fDestValue)
	{
		m_fValueOrigin = m_fValueDest;
		float fDestLength =  fDestValue - m_fValueOrigin;		
		m_fDestLength = Mathf.Abs(fDestLength);		
		m_fValueDest = fDestValue;
		m_fDestTime = m_fDestLength / (m_fValueMax / LengthDuration);
		m_fCurrentTime = 0;

		m_bGaugeActive = true;
	}

	private void UpdateGaugeValue(float fDelta)
	{
		m_fCurrentTime += fDelta;
		float fCurveTime = m_fCurrentTime / m_fDestTime;
		float fMoveValue = GaugeCurve.Evaluate(fCurveTime);
		fMoveValue *= m_fDestLength;

		UpdateGaugeValueCurrent(fMoveValue);
		OnUIGaugeUpdate(fDelta);
	}

	private void UpdateGaugeValueCurrent(float fMoveValue)
	{
		if (m_bGaugeForward)
		{
			m_fValueCurrent = m_fValueOrigin + fMoveValue;
			if (m_fValueCurrent >= m_fValueDest)
			{
				PrivGaugeEnd(m_fValueDest);
			}
			else
			{
				PrivGaugeValueApply(m_fValueCurrent);
			}
		}
		else
		{
			m_fValueCurrent = m_fValueOrigin - fMoveValue;
			if (m_fValueCurrent <= m_fValueDest)
			{
				PrivGaugeEnd(m_fValueDest);
			}
			else
			{
				PrivGaugeValueApply(m_fValueCurrent);
			}
		}
	}

	private void PrivGaugeEnd(float fValue)
	{
		float fMoveValue = fValue - m_fValueOrigin;
		m_fValueCurrent = fValue;
		m_fValueDest = fValue;
		m_fValueOrigin = fValue;
		m_bGaugeActive = false;

		PrivGaugeValueApplySlider(fValue);
		OnUIGaugeEnd(fMoveValue);
		OnUIGaugeRefresh(fValue);
	}

	private void PrivGaugeDirection(float fDestValue)
	{
		if (fDestValue < m_fValueDest)
		{
			m_bGaugeForward = false;
		}
		else if (fDestValue > m_fValueDest)
		{
			m_bGaugeForward = true;
		}
	}

	private void PrivGaugeValueApply(float fValueCurrent)
	{
		PrivGaugeValueApplySlider(fValueCurrent);
		OnUIGaugeRefresh(fValueCurrent);
	}

	private void PrivGaugeValueApplySlider(float fValue)
	{
        float fSliderValue = fValue / m_fValueMax;
        if (fSliderValue <= MinValue)
        {
            fSliderValue = MinValue;
        }
        m_pSlider.value = fSliderValue;
    }

    //---------------------------------------------------------------------------------------------------
    protected virtual void OnUIGaugeUpdate(float fDelta) { }
	protected virtual void OnUIGaugeRefresh(float fValueCurrent) { }
	protected virtual void OnUIGaugeEnd(float fMoveLength) { } // 원점 부터 실제로 이동한 값
	protected virtual void OnUIGaugeReset(float fMax, float fStartValue) { }
	protected virtual void OnUIGaugeStart(float fDestValue) { }

}
