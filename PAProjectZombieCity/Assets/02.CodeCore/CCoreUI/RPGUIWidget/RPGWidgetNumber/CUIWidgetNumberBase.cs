using System.Collections.Generic;
using UnityEngine;

public abstract class CUIWidgetNumberBase : CUIWidgetBase
{
	private long m_iNumberValue = -1;
    private List<int> m_listNumberNote = new List<int>(16);
    //-------------------------------------------------------------------
    private void Update()
    {
        OnUIWidgetNumberUpdate(Time.deltaTime);
    }
	//-------------------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitialize(pParentFrame);
	}

	protected override void OnUIEntryInitializePost(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitializePost(pParentFrame);
		DoTextNumber(0, true);
	}

	//-----------------------------------------------------------------
	public void DoTextNumber(long iTargetNumber, bool bForce = false)
	{
		if (m_iNumberValue == iTargetNumber) return;

		m_iNumberValue = iTargetNumber;

		List<int> pListDigit = ExtractValuePrintDigit(iTargetNumber);
		OnUIWidgetNumber(iTargetNumber, bForce, pListDigit);
	}

    //-------------------------------------------------------------------
    protected List<int> ExtractValuePrintDigit(long iValue)
    {
        m_listNumberNote.Clear();
        if (iValue == 0)
        {
            m_listNumberNote.Add(0);
            return m_listNumberNote;
        }

        int iDigitCount = ExtractValuePrintDigitCount(iValue);
        for (int i = iDigitCount; i >= 1; i--)
        {
            long iDigitNumber = ExtractValueSquare(10, i);
            int iDigitValue = (int)(iValue / iDigitNumber % 10);
            m_listNumberNote.Add(iDigitValue);
        }
        return m_listNumberNote;
    }

    //--------------------------------------------------------------------
    private int ExtractValuePrintDigitCount(long iValue)
	{
		int iCount = 0;
		long iNumber = iValue;
		while (true)
		{
			iNumber /= 10;
			iCount++;

			if (iNumber == 0) break;
		}
		return iCount;
	}

	private long ExtractValueSquare(long iValue, int iSquare)
	{
		long iResult = 1;
		for (int i = 1; i < iSquare; i++)
		{
			iResult *= iValue;
		}

		return iResult;
	}

	//-------------------------------------------------------
	protected virtual void OnUIWidgetNumber(long iTargetNumber, bool bForce, List<int> pListDigit) { }
    protected virtual void OnUIWidgetNumberUpdate(float fDelta) { }
}
