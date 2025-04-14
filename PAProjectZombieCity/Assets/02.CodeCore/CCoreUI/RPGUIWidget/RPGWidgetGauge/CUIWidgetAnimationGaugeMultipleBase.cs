using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIWidgetAnimationGaugeMultipleBase : CUIWidgetAnimationGaugeDestSectionBase
{
	private float m_fGaugeTotal = 0;
	private float m_fValueRemain = 0;
	private float m_fValueAdd = 0;
	private int m_iGaugeLevel = 0;
	//------------------------------------------------------------------------------------------------------
	protected override sealed void OnUIGaugeEnd(float fMoveLength, bool bForward)
	{
		base.OnUIGaugeEnd(fMoveLength, bForward);

		if (IsShow == false) return;

		if (m_fValueRemain != 0)
		{
			bool bNextForward = m_fValueRemain > 0 ? true : false;
			PrivGaugeMultipleContinue(m_fValueRemain, bNextForward);
		}
		else
		{
			OnUIGaugeMultipleEnd(m_iGaugeLevel, m_fValueAdd);
		}
	}

	protected sealed override void OnUIGaugeRefresh(float fValueCurrent, float fValueCurrentRate)
	{
		base.OnUIGaugeRefresh(fValueCurrent, fValueCurrentRate);
		OnUIGaugeMultipleRefresh(m_iGaugeLevel, fValueCurrent, fValueCurrentRate);
	}

	protected sealed override void OnUIGaugeReset(float fMax, float fStartValue)
	{
		base.OnUIGaugeReset(fMax, fStartValue);
		OnUIGaugeMultipleReset(m_iGaugeLevel, fMax, fStartValue);
	}

	//-------------------------------------------------------------------------------------------------------
	protected void ProtGaugeMultipleReset(int iGaugeLevel, float fCurrentValue)
	{
		m_iGaugeLevel = iGaugeLevel;
		m_fGaugeTotal = OnUIGaugeMultipleGetTotal(iGaugeLevel);
		base.ProtGaugeReset(m_fGaugeTotal, fCurrentValue);
	}

	protected void ProtGaugeMultipleAdd(float fAddValue, float fLengthDuration = 0, bool bForce = false)
	{
		m_fValueAdd = fAddValue;
		float fValueDest = GetUIGaugeValueDest() + fAddValue;
		if (fValueDest >= m_fGaugeTotal)
		{
			m_fValueRemain = fAddValue - (m_fGaugeTotal - GetUIGaugeValueDest());
			base.ProtGaugeStart(m_fGaugeTotal, fLengthDuration, bForce);
		}
		else if (fValueDest < 0)
		{
			m_fValueRemain = fValueDest;
			base.ProtGaugeStart(0, fLengthDuration, bForce);
		}
		else
		{
			m_fValueRemain = 0;
			base.ProtGaugeStart(fValueDest, fLengthDuration, bForce);
		}
	}


	//-------------------------------------------------------------------------------------------------------
	private void PrivGaugeMultipleContinue(float fValueRemain, bool bForward)
	{
		float fGaugeTotal = PrivGaugeMultipleRefreshGaugeLevel(bForward);
	
		if (fGaugeTotal != 0)
        {
			m_fGaugeTotal = fGaugeTotal;
            if (bForward)
            {
                base.ProtGaugeReset(m_fGaugeTotal, 0f);
            }
            else
            {
                base.ProtGaugeReset(m_fGaugeTotal, m_fGaugeTotal);
            }
			ProtGaugeMultipleAdd(fValueRemain);
		}
		else
        {
            if (bForward)
            {
                base.ProtGaugeReset(m_fGaugeTotal, m_fGaugeTotal);
            }
            else
            {
                base.ProtGaugeReset(m_fGaugeTotal, 0f);
            }
        }
	}

	private float PrivGaugeMultipleRefreshGaugeLevel(bool bForward)
	{
		if (bForward)
		{
			m_iGaugeLevel++;
		}
		else
		{
			m_iGaugeLevel--;
		}

		float fGaugeTotal = OnUIGaugeMultipleGetTotal(m_iGaugeLevel);
		OnUIGaugeMultipleNextGauge(m_iGaugeLevel, bForward);

		return fGaugeTotal; 
	}

	//----------------------------------------------------------------------------------------------------
	protected virtual void   OnUIGaugeMultipleRefresh(int iGaugeLevel, float fValueCurrent, float fValueCurrentRate) { }
	protected virtual void   OnUIGaugeMultipleEnd(int iGaugeLevel, float fMoveLength) { }
	protected virtual void   OnUIGaugeMultipleNextGauge(int iGaugeLevel, bool bForward) { }
	protected virtual void   OnUIGaugeMultipleReset(int iGaugeLevel, float fGaugeMax, float fValueCurrent) { }
	protected abstract float OnUIGaugeMultipleGetTotal(int iGaugeLevel);  // 0값이 리턴될 경우 이전 토탈 값을 사용한다.

	protected new void ProtGaugeReset(float fGaugeMax, float fCurrentValue) { }
	protected new void ProtGaugeStart(float fDestValue, float fLengthDuration = 0, bool bForce = false) { }
    protected new void ProtGaugeAdd(float fAddValue, float fLengthDuration = 0, bool bForce = false)    { }
}
