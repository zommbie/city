using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public abstract class CUIWidgetNumberTextBase : CUIWidgetNumberBase
{
	private const int c_CommaCount = 3;
	private const double c_PercentResolution = 10000f; //만분율 사용 10000 = 100%

	public enum ENumberTextType
    {
		None,
		Comma,
		Percent,
    }

	[SerializeField]
	private ENumberTextType TextType = ENumberTextType.None;
	
	protected Text m_pText = null;	
	protected StringBuilder m_pTextNote = new StringBuilder(64);
	//-------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		m_pText = GetComponent<Text>();
		m_pText.text = 0.ToString();
		base.OnUIEntryInitialize(pParentFrame);	
	}

	protected override void OnUIWidgetNumber(long iNumber, bool bForce, List<int> pListDigit)
	{
		ProtNumberTextInput(iNumber, pListDigit);
	}
	//--------------------------------------------------------
	protected void ProtNumberTextInput(long iNumber, List<int> pListDigit)
	{
		if (TextType == ENumberTextType.None)
		{
			m_pText.text = iNumber.ToString();
		}
		else if (TextType == ENumberTextType.Comma)
		{
			PrivNumberTextComma(pListDigit);
		}
		else if (TextType == ENumberTextType.Percent)
		{
			PrivNumberTextPercent(iNumber);
		}
	}

	//---------------------------------------------------------
	private void PrivNumberTextComma(List<int> pListDigit)
	{
		m_pTextNote.Clear();

		int iCount = c_CommaCount - (pListDigit.Count % c_CommaCount);
		for (int i = 0; i < pListDigit.Count; i++)
		{
			if (iCount++ % c_CommaCount == 0 && i != 0)
			{
				m_pTextNote.Append(',');
			}
			m_pTextNote.Append(pListDigit[i]);
		}
	
		m_pText.text = m_pTextNote.ToString();
	}

    private void PrivNumberTextPercent(long iNumber)
    {
        double fValue = iNumber / c_PercentResolution;
        m_pText.text = string.Format("{0:0.#}%", fValue);
    }
}
