using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIWidgetNumberTextChartedBase : CUIWidgetNumberTextBase
{
	[SerializeField]
	private float Duration = 1f;
	[SerializeField]
	private long  MinValue = 0;
	[SerializeField]
	private AnimationCurve ChartCurve = AnimationCurve.Linear(0, 0, 1f, 1f);

	private long  m_iDestValue;
	private long  m_iChartedValue;
	private long  m_iCurrentValue;
	private long  m_iOriginValue;
	private float m_fCurrentTime = 0f;
	private bool  m_bUpdateChartNumber = false;
	//-------------------------------------------------------------
	protected override void OnUIWidgetNumber(long iNumber, bool bForce, List<int> pListDigit)
	{
		if (bForce)
		{
			base.OnUIWidgetNumber(iNumber, bForce, pListDigit);
		}
		else
		{
			if (iNumber <= MinValue)
			{
				base.OnUIWidgetNumber(iNumber, bForce, pListDigit);
			}
			else
			{
				PrivNumberTextChartedStart(iNumber);
			}
		}
	}

	protected override void OnUIWidgetNumberUpdate(float fDelta)
	{
		if (m_bUpdateChartNumber)
		{
			UpdateNumberTextCharted(fDelta);
		}
	}
	//----------------------------------------------------------------
	public void SetNumberTextChartDuration(float fDuration)
	{
		Duration = fDuration;
	}

	//----------------------------------------------------------------
	private void PrivNumberTextChartedStart(long iNumber)
	{
		m_iCurrentValue = 0;
		m_iOriginValue = m_iDestValue;
		m_iChartedValue = iNumber - m_iDestValue;
		m_fCurrentTime = 0f;
		m_iDestValue = iNumber;
		m_bUpdateChartNumber = true;
		OnNumberTextChartedStart();
	}

	private void PrivNumberTextChartedEnd()
	{
		m_bUpdateChartNumber = false;
		List<int> pDigit = ExtractValuePrintDigit(m_iDestValue);
		ProtNumberTextInput(m_iDestValue, pDigit);
		OnNumberTextChartedEnd();
	}

	private void UpdateNumberTextCharted(float fDelta)
	{
		m_fCurrentTime += fDelta;
		if (m_fCurrentTime >= Duration)
		{
			PrivNumberTextChartedEnd();
		}
		else
		{
			float fCurveRate = m_fCurrentTime / Duration;
			float fCurveValue = ChartCurve.Evaluate(fCurveRate);
			float fChartedValue = (float)m_iChartedValue * fCurveValue;
			m_iCurrentValue = m_iOriginValue + (long)fChartedValue;

			List<int> pDigit = ExtractValuePrintDigit(m_iCurrentValue);
			ProtNumberTextInput(m_iCurrentValue, pDigit);
			OnNumberTextChartedUpdate(m_iCurrentValue, fCurveValue);
		}
	}

	//-------------------------------------------------------------
	protected virtual void OnNumberTextChartedStart() { }
	protected virtual void OnNumberTextChartedEnd() { }
	protected virtual void OnNumberTextChartedUpdate(long iValue, float fCurveValue) { }

}
