using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIWidgetAnimationGaugeMultipleBase : CUIWidgetAnimationGaugeBase
{
	private float m_fGaugeTotal = 0;
	private float m_fValueRemain = 0;
	private float m_fValueGain = 0;
	private int m_iGaugeLevel = 0;
	//------------------------------------------------------------------------------------------------------
	protected override sealed void OnUIGaugeEnd(float fMoveLength)
	{
		if (IsShow == false) return;

		if (m_fValueRemain != 0)
		{
			bool bNextForward = m_fValueRemain > 0 ? true : false;
			PrivGaugeMultipleContinue(m_fValueRemain, bNextForward);
		}
		else
		{
			OnUIGaugeMultipleEnd(m_iGaugeLevel, m_fValueGain);
		}
	}

	protected sealed override void OnUIGaugeRefresh(float fValueCurrent)
	{
		OnUIGaugeMultipleRefresh(m_iGaugeLevel, fValueCurrent);
	}

	protected sealed override void OnUIGaugeReset(float fMax, float fStartValue)
	{
		OnUIGaugeMultipleReset(m_iGaugeLevel, fMax, fStartValue);
	}

	//-------------------------------------------------------------------------------------------------------
	protected void ProtGaugeMultipleReset(int iGaugeLevel, float fCurrentValue)
	{
		m_iGaugeLevel = iGaugeLevel;
		m_fGaugeTotal = OnUIGaugeMultipleGetTotal(iGaugeLevel);
		ProtGaugeReset(m_fGaugeTotal, fCurrentValue);
	}

	protected void ProtGaugeMultipleStart(float fGainValue, float fLengthDuration = 0, bool bForce = false)
	{
		m_fValueGain = fGainValue;
		float fValueDest = p_ValueDest + fGainValue;
		if (fValueDest >= m_fGaugeTotal)
		{
			m_fValueRemain = fGainValue - (m_fGaugeTotal - p_ValueDest);
			ProtGaugeStart(m_fGaugeTotal, fLengthDuration, bForce);
		}
		else if (fValueDest < 0)
		{
			m_fValueRemain = fValueDest;
			ProtGaugeStart(0, fLengthDuration, bForce);
		}
		else
		{
			m_fValueRemain = 0;
			ProtGaugeStart(fValueDest, fLengthDuration, bForce);
		}
	}
	//-------------------------------------------------------------------------------------------------------
	private void PrivGaugeMultipleContinue(float fValueRemain, bool bForward)
	{
		PrivGaugeMultipleRefreshGaugeLevel(bForward);
	
		if (bForward)
		{
			ProtGaugeReset(m_fGaugeTotal, 0f);
		}
		else
		{
			ProtGaugeReset(m_fGaugeTotal, m_fGaugeTotal);
		}


		ProtGaugeMultipleStart(fValueRemain);
	}

	private void PrivGaugeMultipleRefreshGaugeLevel(bool bForward)
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
		if (fGaugeTotal != 0)
		{
			m_fGaugeTotal = fGaugeTotal;
		}

		OnUIGaugeMultipleNextGauge(m_iGaugeLevel, bForward);
	}

	//----------------------------------------------------------------------------------------------------
	protected virtual void   OnUIGaugeMultipleRefresh(int iGaugeLevel, float fValueCurrent) { }
	protected virtual void   OnUIGaugeMultipleEnd(int iGaugeLevel, float fMoveLength) { }
	protected virtual void   OnUIGaugeMultipleNextGauge(int iGaugeLevel, bool bForward) { }
	protected virtual void   OnUIGaugeMultipleReset(int iGaugeLevel, float fGaugeMax, float fValueCurrent) { }
	protected abstract float OnUIGaugeMultipleGetTotal(int iGaugeLevel);  // 0값이 리턴될 경우 이전 토탈 값을 사용한다.

	protected override sealed void ProtGaugeReset(float fGaugeMax, float fCurrentValue) { base.ProtGaugeReset(fGaugeMax, fCurrentValue);}
	protected override sealed void ProtGaugeStart(float fDestValue, float fLengthDuration = 0, bool bForce = false) { base.ProtGaugeStart(fDestValue, fLengthDuration, bForce);}
}
