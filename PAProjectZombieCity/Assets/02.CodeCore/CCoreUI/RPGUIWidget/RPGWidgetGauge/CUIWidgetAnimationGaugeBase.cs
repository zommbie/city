using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public abstract class CUIWidgetAnimationGaugeBase : CUIWidgetBase
{
	[SerializeField, Range(0.5f, 10f)]
	private float LengthDuration = 5f; //  0에서 Max까지 움직이는 시간 . 별도 입력이 없으면 이 값을 대표 값으로 사용 
	
	[SerializeField]
	private bool  RefreshOnStart = false;   // 작동 중 입력시 바로 현재 값을 갱신한다. 

	[SerializeField]
	private AnimationCurve GaugeCurve = AnimationCurve.Linear(0, 0, 1f, 1f);  // 입력 구간을 움직이는데 필요한 이동값 곡선 1f = 전체 이동값

	private Slider	    m_pSlider = null;
	private float		m_fValueDest = 0;		 public float GetUIGaugeValueDest()    { return m_fValueDest;}      // 실제 게이지 수치  
	private float		m_fValueCurrent = 0;	 public float GetUIGaugeValueCurrent() { return m_fValueCurrent;}   // 현재 게이지 수치
	private float       m_fValueDestRate = 0;	 public float GetUIGaugeValueDestRate() { return m_fValueDestRate; }
	private float       m_fValueCurrentRate = 0; public float GetUIGaugeValueCurrentRate() { return m_fValueCurrentRate; }	
	private float		m_fValueOrigin = 0;		 
	private float		m_fValueMax = 1f;
	private float		m_fCurrentTime = 0;
	private float		m_fDestTime = 0;       // 목표 도달 까지 시간 - 커브 적용을 위한 값
	private float		m_fDestLength = 0;     // 목표 까지 도달거리. 절대 값
	private float       m_fTotalDuration = 0;

	private bool		m_bGaugeForward = false;  public bool IsGaugeForward { get { return m_bGaugeForward; }} // 정방향 = 오른쪽  
	private bool		m_bGaugeActive = false;   public bool IsGaugeActive  { get { return m_bGaugeActive;  }}
	//----------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitialize(pParentFrame);
		m_pSlider = GetComponent<Slider>();
		m_pSlider.interactable = false;        
	}

	protected override void OnUIEntryUIFrameShowHide(bool bShow)	{ if (bShow == false) { ProtGaugeStop();}}
	protected override void OnUIWidgetShowHide(bool bShow) { if (bShow == false) { ProtGaugeStop();}}
	protected override void OnUIWidgetShowHideForce(bool bShow) { if (bShow == false) { ProtGaugeStop();}}

	private void Update()
	{
        float fDelta = Time.deltaTime;
        if (m_bGaugeActive)
		{
			UpdateGaugeValue(fDelta);			
		}

		OnUIUnityUpdate(fDelta);
	}
	//----------------------------------------------------------
	protected virtual void ProtGaugeStart(float fDestValue, float fLengthDuration = 0, bool bForce = false)
	{
		if (fDestValue < 0) fDestValue = 0;
		if (fDestValue > m_fValueMax) fDestValue = m_fValueMax;
		if (fLengthDuration != 0)
        {
			m_fTotalDuration = fLengthDuration;
		}
		else
        {
			m_fTotalDuration = LengthDuration;
        }

		
        if (bForce || m_fValueCurrent == fDestValue)
		{
			PrivGaugeEnd(fDestValue);
		}
		else
		{
			PrivGaugeStart(fDestValue);
		}
	}

	protected virtual void ProtGaugeAdd(float fAddValue, float fLengthDuration = 0, bool bForce = false)
    {
		ProtGaugeStart(m_fValueDest + fAddValue, fLengthDuration, bForce);
    }

	protected virtual void ProtGaugeReset(float fGaugeMax, float fCurrentValue)
	{
		m_fValueMax = fGaugeMax == 0 ? 1f : fGaugeMax;
		m_fValueOrigin = fCurrentValue;
		m_fCurrentTime = 0;
		PrivGaugeValueCurrent(fCurrentValue);
		PrivGaugeValueDest(fCurrentValue);
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
		if (RefreshOnStart)
        {
            PrivGaugeValueCurrent(m_fValueDest);
        }
        m_fValueOrigin = m_fValueCurrent;

        float fDestLength =  fDestValue - m_fValueCurrent;		
		m_fDestLength = Mathf.Abs(fDestLength);	
		
		PrivGaugeValueDest(fDestValue);

		m_fDestTime = m_fDestLength / (m_fValueMax / m_fTotalDuration);
		m_fCurrentTime = 0f;
		m_bGaugeActive = true;

		OnUIGaugeStart(m_fValueDest, m_fValueDestRate, m_bGaugeForward);
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
			PrivGaugeValueCurrent(m_fValueOrigin + fMoveValue);
			if (m_fValueCurrent >= m_fValueDest)
			{
				PrivGaugeEnd(m_fValueDest);
			}
		}
		else
		{
            PrivGaugeValueCurrent(m_fValueOrigin - fMoveValue);
            if (m_fValueCurrent <= m_fValueDest)
			{
				PrivGaugeEnd(m_fValueDest);
			}
		}
	}

	private void PrivGaugeEnd(float fValue)
	{
		float fMoveValue = fValue - m_fValueOrigin;
		PrivGaugeValueCurrent(fValue);
        m_fValueDest = fValue;
        m_fValueDestRate = m_fValueDest / m_fValueMax;
        m_fValueOrigin = fValue;
		m_bGaugeActive = false;
		m_fCurrentTime = 0;		
		OnUIGaugeEnd(fMoveValue, m_bGaugeForward);
		OnUIGaugeRefresh(m_fValueCurrent, m_fValueCurrentRate);
	}

	private void PrivGaugeValueApply(float fValueCurrent)
	{
		PrivGaugeValueApplySlider(fValueCurrent);
		OnUIGaugeRefresh(m_fValueCurrent, m_fValueCurrentRate);
	}

	private void PrivGaugeValueApplySlider(float fValue)
	{
        float fSliderValue = fValue / m_fValueMax;
		if (fSliderValue < 0)
        {
			fSliderValue = 0f;
        }
     
		if (fSliderValue > 1f)
        {
			fSliderValue = 1f;
        }

		m_pSlider.value = fSliderValue;
    }

	private void PrivGaugeValueCurrent(float fValueCurrent)
    {
		m_fValueCurrent = fValueCurrent;
		m_fValueCurrentRate = m_fValueCurrent / m_fValueMax;
		PrivGaugeValueApply(fValueCurrent);
	}

	private void PrivGaugeValueDest(float fValueDest)
    {
		if (m_fValueCurrent <= fValueDest)
        {
			m_bGaugeForward = true;
        }
		else
        {
			m_bGaugeForward = false;
        }

		m_fValueDest = fValueDest;		
        m_fValueDestRate = m_fValueDest / m_fValueMax;
    }

    //---------------------------------------------------------------------------------------------------
    protected virtual void OnUIGaugeUpdate(float fDelta) { }
	protected virtual void OnUIGaugeRefresh(float fValueCurrent, float fValueCurrentRate) { }
	protected virtual void OnUIUnityUpdate(float fDelta) { }

	protected virtual void OnUIGaugeEnd(float fMoveLength, bool bFoward) { } // 원점 부터 실제로 이동한 값
	protected virtual void OnUIGaugeReset(float fMax, float fStartValue) { }	
	protected virtual void OnUIGaugeStart(float fDestValue, float fGaugeDestRate , bool bFoward) { }

}
